
using System;

namespace Fu.Tests.Dummy
{
    public class Foo
    {
        public string String { get; set; }
        public int Int { get; set; }
        public float Float { get; set; }

        public int? NullableInt { get; set; }
        public DateTime DateTime { get; set; }
        public DateTime? NullableDateTime { get; set; }


        public bool FieldsEqual(Foo another)
        {
            return this.String == another.String &&
                this.Int == another.Int &&
                this.Float == another.Float &&
                this.NullableInt == another.NullableInt &&
                this.DateTime == another.DateTime &&
                this.NullableDateTime == another.NullableDateTime;
        }
    }
}
