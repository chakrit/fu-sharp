
using System;

using Fu;
using Fu.Contexts;
using Fu.Steps;

namespace RestStyle
{
    public abstract class RestController : FuController
    {
        public void Get(string url, Step step)
        { map(url, step404 => fu.Http.Get(step, step404)); }

        public void Put(string url, Step step)
        { map(url, step404 => fu.Http.Put(step, step404)); }

        public void Post(string url, Step step)
        { map(url, step404 => fu.Http.Post(step, step404)); }

        public void Delete(string url, Step step)
        { map(url, step404 => fu.Http.Delete(step, step404)); }


        // user-supplied step is passed over via closures in Get/Put/Post/Delete
        // so we only need the step404
        private void map(string url, Func<Step, Step> wrapper)
        {
            if (Mappings.ContainsKey(url))
                Mappings[url] = wrapper(Mappings[url]);

            else
                Mappings[url] = wrapper(fu.Http.NotFound());
        }
    }
}
