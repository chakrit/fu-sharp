
using System;
using System.Globalization;
using System.Linq;

namespace Fu.Steps
{
    // TODO: Implement proper Cache-Control/Expires support
    //       which is a bit trickier to be done right
    // TODO: Still needs tests in the wild to make sure these work properly
    public static class Cache
    {
        public static Step LastModified(this ICacheSteps _, DateTime lastModified, Step step)
        { return _.LastModified((c, d) => lastModified, step); }

        public static Step LastModified(this ICacheSteps _,
            Filter<DateTime> lastModifiedFilter, Step step)
        {
            return fu.Void(c =>
            {
                DateTime lastModified, d;

                var ifModifiedHeader = c.Request.Headers["If-Modified-Since"];

                // if a valid date has been given as a validator
                if (!string.IsNullOrEmpty(ifModifiedHeader) &&
                    DateTime.TryParseExact(ifModifiedHeader, "R", CultureInfo.CurrentCulture,
                    DateTimeStyles.RoundtripKind, out d))
                {
                    // run the filter and check the modified date,
                    lastModified = lastModifiedFilter(c, d);

                    // automatically returning 304 Not Modified if date indicates not modified
                    if (lastModified.ToUniversalTime() <= d.ToUniversalTime())
                    {
                        c.WalkPath.InsertNext(
                            fu.Http.Header("Last-Modified", lastModified.ToString("R")),
                            fu.Http.NotModified(),
                            fu.Walk.Stop());
                        return;
                    }
                }

                // else just proceeds with the content step normally
                lastModified = lastModifiedFilter(c, DateTime.MinValue);

                c.WalkPath.InsertNext(
                    fu.Http.Header("Last-Modified", lastModified.ToString("R")),
                    step);
            });
        }

        public static Step ETag(this ICacheSteps _, Reduce<string> etagReducer, Step step)
        {
            return fu.Void(c =>
            {
                // always sends the etag regardless wether the validator works or not
                var etag = etagReducer(c);

                if (!string.IsNullOrEmpty(etag))
                    c.WalkPath.InsertNext(fu.Http.Header("ETag", "\"" + etag + "\""));

                // if the validator validates, send back a 304 Not Modified
                var ifNoneMatchHeader = c.Request.Headers["If-None-Match"];

                if (!string.IsNullOrEmpty(ifNoneMatchHeader))
                {
                    // note that the Select part is lazy-evaluated
                    var existingETags = ifNoneMatchHeader
                        .Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                        .Select(h => h.Trim())
                        .Where(h => !string.IsNullOrEmpty(h) && h.Length > 2)
                        .Select(h => h.Substring(1, h.Length - 2));

                    // check wether there's meat (!= null) to match a wildcard
                    // or that wether the client have a representation with the same etag
                    if ((ifNoneMatchHeader == "*" && !string.IsNullOrEmpty(etag)) ||
                        existingETags.Contains(etag))
                    {
                        c.WalkPath.InsertNext(
                            fu.Http.NotModified(),
                            fu.Walk.Stop());
                        return;
                    }
                }

                // if not, proceeds with the normal rendering
                c.WalkPath.InsertNext(step);
            });
        }
    }
}
