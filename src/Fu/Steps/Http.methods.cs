
namespace Fu.Steps
{
    public static partial class Http
    {
        public static Step Method(this IHttpSteps _, string method, Step step)
        { return _.Method(method, step, null); }

        public static Step Method(this IHttpSteps _, string method, Step step, Step step404)
        {
            step404 = step404 ?? _.MethodNotAllowed();
            method = method.ToUpper();

            // TODO: What happens with lowercase HTTP methods?
            //       What would be the proper behavior in such cases?
            return c => (c.Request.HttpMethod == method) ?
                step(c) :
                step404(c);
        }


        #region GET POST PUT DELETE overloads

        public static Step Get(this IHttpSteps _, Step step)
        { return _.Get(step, null); }

        public static Step Get(this IHttpSteps _, Step step, Step step404)
        { return _.Method("GET", step, step404); }

        public static Step Post(this IHttpSteps _, Step step)
        { return _.Post(step, null); }

        public static Step Post(this IHttpSteps _, Step step, Step step404)
        { return _.Method("POST", step, step404); }

        public static Step Put(this IHttpSteps _, Step step)
        { return _.Method("PUT", step, null); }

        public static Step Put(this IHttpSteps _, Step step, Step step404)
        { return _.Method("PUT", step, step404); }

        public static Step Delete(this IHttpSteps _, Step step)
        { return _.Method("DELETE", step, null); }

        public static Step Delete(this IHttpSteps _, Step step, Step step404)
        { return _.Method("DELETE", step, step404); }

        #endregion


        public static Step Methods(this IHttpSteps _, Step get, Step post)
        { return _.Methods(get, post, null); }

        public static Step Methods(this IHttpSteps _, Step get, Step post, Step step404)
        { return _.Method("GET", get, _.Method("POST", post, step404)); }

        public static Step Methods(this IHttpSteps _, Step get, Step put, Step post, Step delete)
        { return _.Methods(get, put, post, delete, null); }

        public static Step Methods(this IHttpSteps _, Step get, Step put, Step post, Step delete, Step step404)
        {
            return _.Method("GET", get,
                _.Method("POST", post,
                _.Method("PUT", put,
                _.Method("DELETE", delete, step404))));
        }
    }
}
