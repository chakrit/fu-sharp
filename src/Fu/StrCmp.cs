
using System;
using System.Text.RegularExpressions;

namespace Fu
{
    internal static class StrCmp
    {
        public static StringComparison Fast = StringComparison.OrdinalIgnoreCase;
        public static StringComparison UI = StringComparison.CurrentCulture;
        public static StringComparison IgnoreCase = StringComparison.CurrentCultureIgnoreCase;

        public static RegexOptions FastRx = RegexOptions.CultureInvariant | RegexOptions.IgnoreCase;
    }
}
