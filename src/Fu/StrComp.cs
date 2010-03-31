
using System;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;

namespace Fu
{
  internal static class StrComp
  {
    public static StringComparison Fast = StringComparison.OrdinalIgnoreCase;
    public static StringComparison UI = StringComparison.CurrentCulture;
    public static StringComparison IgnoreCase = StringComparison.CurrentCultureIgnoreCase;

    public static StringComparer FastComp = StringComparer.OrdinalIgnoreCase;
    public static StringComparer UIComp = StringComparer.CurrentCulture;
    public static StringComparer IgnoreCaseComp = StringComparer.CurrentCultureIgnoreCase;

    public static RegexOptions FastRx = RegexOptions.CultureInvariant | RegexOptions.IgnoreCase;
  }
}
