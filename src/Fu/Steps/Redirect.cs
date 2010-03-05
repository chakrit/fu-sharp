
using System;
using System.Net;

namespace Fu.Steps
{
    public static class Redirect
    {
        public static Step To(this IRedirectSteps _, string target)
        { return _.To((c, s) => target); }

        public static Step To(this IRedirectSteps _, FilterStep<string> urlStep)
        {
            return c =>
            {
                var target = urlStep(c, c.Request.Url.AbsolutePath);

                c.Response.StatusCode = (int)HttpStatusCode.Moved;
                c.Response.Redirect(target);
                c.Response.Close();

                c.WalkPath.InsertNext(fu.Walk.Stop());
                return c;
            };
        }


        public static Step PermanentlyTo(this IRedirectSteps _, string target)
        { return _.PermanentlyTo((c, s) => target); }

        public static Step PermanentlyTo(this IRedirectSteps _, FilterStep<string> urlStep)
        {
            return c =>
            {
                var target = urlStep(c, c.Request.Url.AbsolutePath);

                c.Response.StatusCode = (int)HttpStatusCode.MovedPermanently;
                c.Response.Redirect(target);
                c.Response.Close();

                c.WalkPath.InsertNext(fu.Walk.Stop());
                return c;
            };
        }
    }
}
