
using System;
using System.Linq;
using System.Reflection;

using Fu.Services.Web;
using System.ComponentModel;

namespace Fu.Services.Models
{
    public class ModelBinder<T> : IService<T>
        where T : class
    {
        public FilterStep<T> Binder { get; private set; }
        public Func<T> Factory { get; private set; }


        public ModelBinder() : this(null, null) { }
        public ModelBinder(Func<T> factory) : this(factory, null) { }
        public ModelBinder(FilterStep<T> binder) : this(null, binder) { }

        public ModelBinder(Func<T> factory, FilterStep<T> binder)
        {
            Factory = factory ?? defaultFactory;
            Binder = binder ?? createDefaultBinder();
        }


        public T GetModel(IFuContext input)
        {
            return Binder(input, Factory());
        }


        private T defaultFactory() { return Activator.CreateInstance<T>(); }

        private FilterStep<T> createDefaultBinder()
        {
            // build and cache property setter methods for the type
            var type = typeof(T);
            var noIndex = new object[0];

            var setters = type
                .GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Where(p => p.CanWrite)
                .ToDictionary(
                    p => p.Name,
                    p => new Action<T, object>((m, value) =>
                    {
                        value = convertTo(value, p.PropertyType);
                        p.SetValue(m, value, noIndex);
                    }));

            // then use it to load values from form data when the app starts
            return (c, model) =>
            {
                var data = c.Get<IFormData>();
                var availableKeys = data
                    .Select(kv => kv.Key)
                    .Intersect(setters.Keys);

                foreach (var key in availableKeys)
                    setters[key](model, data[key]);

                return model;
            };
        }

        private object convertTo(object value, Type targetType)
        {
            // check for nullables
            if (targetType.IsGenericType &&
                targetType.GetGenericTypeDefinition() == typeof(Nullable<>))
            {
                if (value == null) return null;

                // we have a Nullable<T> that has a value
                NullableConverter converter = new NullableConverter(targetType);
                targetType = converter.UnderlyingType;
            }

            // we have a type that isn't Nullable<> by this point
            if (targetType != typeof(string) && value == null)
                return Activator.CreateInstance(targetType);

            return Convert.ChangeType(value, targetType);
        }


        bool IService<T>.CanGetServiceObject(IFuContext input)
        {
            return input.Request.HttpMethod == "POST" &&
                input.CanGet<IFormData>();
        }

        T IService<T>.GetServiceObject(IFuContext input) { return GetModel(input); }

        void IService.BeginWalk(IFuContext input) { /* no-op */ }
        void IService.EndWalk(IFuContext input) { /* no-op */ }
    }
}
