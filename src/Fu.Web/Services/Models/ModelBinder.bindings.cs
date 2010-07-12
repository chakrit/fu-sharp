
using System;
using System.ComponentModel;
using System.Linq;
using System.Reflection;

using Fu.Services.Web;

namespace Fu.Services.Models
{
  // TODO: Array binders ... prop.array[0].anoterProp.anotherArray[1].prop
  public partial class ModelBinder<T>
    where T : class
  {
    private delegate void BinderAction(object model, IFormData form, string prefix);


    private const string FieldSeparator = ".";
    private const string ArrayFieldRx =
      @"(?<name>[a-zA-Z0-9_]+)\[(?<index>\d+)?\]" + // property name and indexer
      @"(?<suffix>\..+)?"; // optional prop[].suffix

    private static object[] _noIndex = new object[0];


    private static Func<T> createDefaultFactory()
    {
      return () => Activator.CreateInstance<T>();
    }

    private static Filter<T> createDefaultBinder()
    {
      var binder = createComplexBinder(null);

      return (c, model) =>
      {
        binder(model, c.Get<IFormData>(), null);
        return model;
      };
    }

    // TODO: Cache on PropertyInfo
    private static BinderAction createComplexBinder(PropertyInfo prop)
    {
      var type = prop == null ? typeof(T) : prop.PropertyType;
      var props = type
        .GetProperties(BindingFlags.Public | BindingFlags.Instance);

      // categorize properties
      var arrayProps = props.Where(p => p.PropertyType.IsArray);

      var simpleProps = props
        .Where(p => !p.PropertyType.IsArray &&
          p.PropertyType.Namespace == "System");

      var complexProps = props
        .Except(arrayProps)
        .Except(simpleProps);

      // make binders for each type of property (arrays not supported yet)
      var simpleBinders = simpleProps
        .Select(p => createSimpleBinder(p))
        .ToArray();

      var complexBinders = complexProps
        .Select(p => new {
          Prop = p,
          Binder = new Lazy<BinderAction>(() => createComplexBinder(p))
        })
        .ToArray();


      // return a function which make uses of the binders to bind the model
      // if property == null (we're at root), 
      return (model, form, prefix) =>
      {
        // invoke simple binders on root
        Array.ForEach(simpleBinders, b => b(model, form, prefix));

        // recursively bind complex objects by adding the appropriate prefixes
        Array.ForEach(complexBinders, cb =>
        {
          var innerModel = cb.Prop.GetValue(model, _noIndex);
          var newPrefix = prefix + cb.Prop.Name + FieldSeparator;

          // halt if no more values to recurse and bind
          if (!form.Any(kv => kv.Key.StartsWith(newPrefix, StrComp.Fast)))
            return;

          if (innerModel == null) {
            innerModel = Activator.CreateInstance(cb.Prop.PropertyType);
            cb.Prop.SetValue(model, innerModel, _noIndex);
          }

          cb.Binder.Value(innerModel, form, newPrefix);
        });
      };
    }


    // TODO: Convert to Expression builders
    private static BinderAction createSimpleBinder
      (PropertyInfo prop)
    {
      // TODO: add .Parse and .TryParse converter support

      // create simple convert-and-assign binder
      return (model, form, prefix) =>
      {
        var rawValue = form[prefix + prop.Name];
        if (string.IsNullOrEmpty(rawValue)) return;

        var value = convertTo(rawValue, prop.PropertyType);
        prop.SetValue(model, value, _noIndex);
      };
    }


    private static object convertTo(object value, Type targetType)
    {
      // check for nullables
      if (targetType.IsGenericType &&
        targetType.GetGenericTypeDefinition() == typeof(Nullable<>)) {

        if (value == null)
          return null;

        // we have a Nullable<T> that has a value
        NullableConverter converter = new NullableConverter(targetType);
        targetType = converter.UnderlyingType;
      }

      // we have a type that isn't Nullable<> by this point
      if (targetType != typeof(string) && value == null)
        return Activator.CreateInstance(targetType);

      try {
        return Convert.ChangeType(value, targetType);
      }
      catch (FormatException) {
        return Activator.CreateInstance(targetType);
      }
    }
  }
}
