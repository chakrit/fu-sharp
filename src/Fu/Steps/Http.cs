
using System.Net;

namespace Fu.Steps
{
  public static partial class Http
  {
    public static Step Status(this IHttpSteps _, HttpStatusCode statusCode)
    { return _.Status((int)statusCode); }

    public static Step Status(this IHttpSteps _, HttpStatusCode statusCode, string statusDesc)
    { return _.Status((int)statusCode, statusDesc); }

    public static Step Status(this IHttpSteps _, int statusCode)
    { return _.Status(statusCode, Statuses[statusCode]); }

    public static Step Status(this IHttpSteps _, int statusCode, string statusDesc)
    {
      return fu.Void(c =>
      {
        c.Response.StatusCode = statusCode;
        c.Response.StatusDescription = statusDesc;
      });
    }


    public static Step Header(this IHttpSteps _, string header, string value)
    {
      return fu.Void(c => c.Response.Headers[header] = value);
    }

    public static Step Header(this IHttpSteps _, string header, Filter<string> valueFilter)
    {
      return fu.Void(c =>
      {
        var value = c.Request.Headers[header];
        value = valueFilter(c, value);

        c.Response.Headers[header] = value;
      });
    }
  }
}
