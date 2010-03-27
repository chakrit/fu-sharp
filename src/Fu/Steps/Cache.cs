
using System;
using System.Globalization;
using System.Linq;
using Fu.Contexts;
using Fu.Results;

namespace Fu.Steps
{
    // TODO: Implement Cache-Control support
    // TODO: Still needs tests in the wild to make sure these work properly
    public static class Cache
    {
        public static Step Expires(this ICacheSteps _, TimeSpan time)
        { return _.Expires(c => DateTime.Now.Add(time)); }

        public static Step Expires(this ICacheSteps _, DateTime date)
        { return _.Expires(c => date); }

        public static Step Expires(this ICacheSteps _, Filter<DateTime> dateFilter)
        { return _.Expires(c => dateFilter(c, DateTime.Now)); }

        public static Step Expires(this ICacheSteps _, Reduce<DateTime> dateReducer)
        { return fu.Void(c => c.Request.Headers["Expires"] = dateReducer(c).ToString("R")); }


        public static Step LastModified(this ICacheSteps _, DateTime lastModified)
        { return _.LastModified((c, d) => lastModified, fu.Identity); }

        public static Step LastModified(this ICacheSteps _, Reduce<DateTime> lastModifiedReducer)
        { return _.LastModified((c, d) => lastModifiedReducer(c), fu.Identity); }

        public static Step LastModified(this ICacheSteps _, Filter<DateTime> lastModifiedFilter)
        { return _.LastModified(lastModifiedFilter, fu.Identity); }

        public static Step LastModified(this ICacheSteps _, DateTime lastModified, Step step)
        { return _.LastModified((c, d) => lastModified, step); }

        public static Step LastModified(this ICacheSteps _,
            Filter<DateTime> lastModifiedFilter, Step step)
        {
            return fu.Branch(c =>
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
                    if (roughCompare(lastModified.ToUniversalTime(), d.ToUniversalTime()) <= 0)
                        return fu.Compose(
                            fu.Http.Header("Last-Modified", lastModified.ToString("R")),
                            fu.Http.NotModified(),
                            fu.Walk.Stop());
                }

                // else just proceeds with the content step normally
                lastModified = lastModifiedFilter(c, DateTime.MinValue);

                return fu.Compose(
                    fu.Http.Header("Last-Modified", lastModified.ToString("R")),
                    step);
            });
        }


        public static Step ETag(this ICacheSteps _, string etag)
        { return _.ETag(c => etag, fu.Identity); }

        public static Step ETag(this ICacheSteps _, string etag, Step step)
        { return _.ETag(c => etag, step); }

        public static Step ETag(this ICacheSteps _, Reduce<string> etagReducer)
        { return _.ETag(etagReducer, fu.Identity); }

        public static Step ETag(this ICacheSteps _, Reduce<string> etagReducer, Step step)
        {
            return fu.Branch(c =>
            {
                var etag = etagReducer(c);

                // always sends the etag regardless wether the validator works or not
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
                        return fu.Compose(
                            fu.Http.NotModified(),
                            fu.Walk.Stop());
                    }
                }

                // if not, proceeds with the normal rendering
                return step;
            });
        }


        private static long roughCompare(DateTime d1, DateTime d2)
        {
            const long oneSecond = 10000 * 1000 /* 10,000 ticks = 1 ms */;

            // eliminate differences smaller than 1 second interval
            var result = (d1 - d2).Ticks;
            result -= result % oneSecond;

            return result;
        }
    }
}
