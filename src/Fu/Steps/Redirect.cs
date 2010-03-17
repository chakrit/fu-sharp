
using System.Net;

namespace Fu.Steps
{
    public static class Redirect
    {
        public static Step To(this IRedirectSteps _, string target)
        { return _.To((c, s) => target); }

        public static Step To(this IRedirectSteps _, Filter<string> urlFilter)
        { return redirectStep(fu.Http.Found(), urlFilter); }


        public static Step PermanentlyTo(this IRedirectSteps _, string target)
        { return _.PermanentlyTo((c, s) => target); }

        public static Step PermanentlyTo(this IRedirectSteps _, Filter<string> urlFilter)
        { return redirectStep(fu.Http.MovedPermanently(), urlFilter); }


        private static Step redirectStep(Step headerStep, Filter<string> urlFilter)
        {
            var stopWalk = fu.Walk.Stop();

            return fu.Void(c =>
            {
                var targetUrl = urlFilter(c, c.Request.Url.AbsolutePath);

                c.Response.Redirect(targetUrl);
                c.WalkPath.InsertNext(
                    headerStep,
                    stopWalk);
            });
        }
    }
}
