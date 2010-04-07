
using System.Net;

namespace Fu.Steps
{
  public static partial class Http
  {
    public static Continuation Status(this IHttpSteps _,
      HttpStatusCode statusCode)
    { return _.Status((int)statusCode); }

    public static Continuation Status(this IHttpSteps _,
      HttpStatusCode statusCode, string statusDesc)
    { return _.Status((int)statusCode, statusDesc); }

    public static Continuation Status(this IHttpSteps _, int statusCode)
    { return _.Status(statusCode, Statuses[statusCode]); }

    public static Continuation Status(this IHttpSteps _, int statusCode, string statusDesc)
    {
      return step => ctx =>
      {
        ctx.Response.StatusCode = statusCode;
        ctx.Response.StatusDescription = statusDesc;

        step(ctx);
      };
    }


    public static Continuation Header(this IHttpSteps _, string header, string value)
    {
      return _.Header(header, (c, s) => value);
    }

    public static Continuation Header(this IHttpSteps _,
      string header, Reduce<string> valueReducer)
    {
      return _.Header(header, (ctx, s) => valueReducer(ctx));
    }

    public static Continuation Header(this IHttpSteps _,
      string header, Filter<string> valueFilter)
    {
      return step => ctx =>
      {
        var value = ctx.Request.Headers[header];
        value = valueFilter(ctx, value);

        ctx.Response.Headers[header] = value;

        step(ctx);
      };
    }
  }
}
