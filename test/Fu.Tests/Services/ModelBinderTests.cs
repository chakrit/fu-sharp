
using System.Reflection;
using System.Linq;

using MbUnit.Framework;

using Moq;

using Fu.Services.Models;
using Fu.Services.Web;
using Fu.Tests.Dummy;
using System;
using System.Collections.Generic;

namespace Fu.Tests.Services
{
    [TestFixture]
    public class ModelBinderTests
    {
        [Test]
        public void BasicBinding()
        {
            // create a mock context/formdata
            var obj = new Foo()
            {
                String = "str",
                Int = 123,
                Float = 456F,
                DateTime = DateTime.MinValue,
                NullableInt = new Nullable<int>(789),
                NullableDateTime = new Nullable<DateTime>(DateTime.MinValue),
            };

            var form = mockFormData(obj);
            var ctx = new Mock<IFuContext>();
            ctx.Setup(c => c.Get<IFormData>())
                .Returns(form.Object);

            // bind the model
            var binder = getBinder();
            var result = binder.GetModel(ctx.Object);

            // verify that model value matches
            form.VerifyAll();
            Assert.IsTrue(obj.FieldsEqual(result));
        }


        private ModelBinder<Foo> getBinder()
        {
            return new ModelBinder<Foo>();
        }

        private Mock<IFormData> mockFormData<T>(T obj)
        {
            var props = typeof(T)
                .GetProperties(BindingFlags.Public | BindingFlags.Instance);

            var mock = new Mock<IFormData>();
            var dud = new object[0];

            // mock each property to return value from the object when get-ed
            Array.ForEach(props, p => mock
                .SetupGet(d => d[It.Is<string>(arg => arg == p.Name)])
                .Returns((p.GetValue(obj, dud) ?? "").ToString())
                .Verifiable());

            // setup enumerating support
            var keys = props.Select(p => new KeyValuePair<string, string>(p.Name, null));
            mock.Setup(d => d.GetEnumerator()).Returns(keys.GetEnumerator());

            return mock;
        }
    }
}
