
using System.Net;

namespace Fu.Steps
{
  public static class Redirect
  {
    public static Continuation To(this IRedirectSteps _, string target)
    { return _.To((c, s) => target); }

    public static Continuation To(this IRedirectSteps _, Reduce<string> urlReducer)
    { return _.To((c, s) => urlReducer(c)); }

    public static Continuation To(this IRedirectSteps _, Filter<string> urlFilter)
    { return redirectStep(fu.Http.Found(), urlFilter); }


    public static Continuation PermanentlyTo(this IRedirectSteps _, string target)
    { return _.PermanentlyTo((c, s) => target); }

    public static Continuation PermanentlyTo(this IRedirectSteps _, Reduce<string> urlReducer)
    { return _.PermanentlyTo((c, s) => urlReducer(c)); }

    public static Continuation PermanentlyTo(this IRedirectSteps _, Filter<string> urlFilter)
    {
      return redirectStep(fu.Http.MovedPermanently(), urlFilter);
    }


    private static Continuation redirectStep(Continuation header, Filter<string> urlFilter)
    {
      return step => outerCtx =>
      {
        header(ctx =>
        {
          var targetUrl = urlFilter(ctx, ctx.Request.Url.AbsolutePath);
          ctx.Response.Redirect(targetUrl);
        })(outerCtx);

        // step is discarded
        // (we're redirecting, so no more processing should take place)
      };
    }
  }
}
