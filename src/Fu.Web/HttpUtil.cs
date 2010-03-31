
using System.Web;

namespace Fu
{
  public static class HttpUtil
  {
    public static string UrlEncode(this IFuContext c, string text)
    { return HttpUtility.UrlEncode(text); }

    public static string UrlDecode(this IFuContext c, string text)
    { return HttpUtility.UrlDecode(text); }

    public static string HtmlEncode(this IFuContext c, string text)
    { return HttpUtility.HtmlEncode(text); }

    public static string HtmlDecode(this IFuContext c, string text)
    { return HttpUtility.HtmlDecode(text); }
  }
}
