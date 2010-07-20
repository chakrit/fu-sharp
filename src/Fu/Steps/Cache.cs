
using System;
using System.Globalization;
using System.Linq;

namespace Fu.Steps
{
  // TODO: Implement Cache-Control support
  // TODO: Still needs tests in the wild to make sure these work properly
  public static class Cache
  {
    protected const long OneSecond = 10000 * 1000 /* 10,000 ticks = 1 ms */;


    public static Continuation Expires(this ICacheSteps _, TimeSpan time)
    { return _.Expires(c => DateTime.Now.Add(time)); }

    public static Continuation Expires(this ICacheSteps _, DateTime date)
    { return _.Expires(c => date); }

    public static Continuation Expires(this ICacheSteps _, Reduce<DateTime> dateReducer)
    {
      return fu.Http.Header("Expires", ctx => dateReducer(ctx).ToString("R"));
    }


    public static Continuation LastModified(this ICacheSteps _, Reduce<DateTime> lastModifiedReducer)
    { return _.LastModified((c, d) => lastModifiedReducer(c)); }

    public static Continuation LastModified(this ICacheSteps _, DateTime lastModified)
    { return _.LastModified((c, d) => lastModified); }

    public static Continuation LastModified(this ICacheSteps _,
        Filter<DateTime> lastModifiedFilter)
    {
      return step => ctx =>
      {
        DateTime lastModified, d;

        var ifModifiedHeader = ctx.Request.Headers["If-Modified-Since"];

        // if a valid date has been given as a validator
        if (!string.IsNullOrEmpty(ifModifiedHeader) &&
          DateTime.TryParseExact(ifModifiedHeader, "R", CultureInfo.CurrentCulture,
          DateTimeStyles.RoundtripKind, out d)) {

          // run the filter and check the modified date,
          lastModified = lastModifiedFilter(ctx, d);

          // automatically returning 304 Not Modified if date indicates not modified
          if (roughCompare(lastModified.ToUniversalTime(), d.ToUniversalTime()) <= 0)
            step = fu.Compose(
              fu.Http.Header("Last-Modified", lastModified.ToString("R")),
              fu.Http.NotModified())
              (fu.EndAct);
        }
        else {
          lastModified = lastModifiedFilter(ctx, DateTime.MinValue);
        }

        fu.Http.Header("Last-Modified", lastModified.ToString("R"))
          (step)(ctx);
      };
    }


    public static Continuation ETag(this ICacheSteps _, string etag)
    { return _.ETag(c => etag); }

    public static Continuation ETag(this ICacheSteps _,
      Reduce<string> etagReducer)
    {
      return step => ctx =>
      {
        var etag = etagReducer(ctx);

        // always sends the etag regardless wether the validator works or not
        if (!string.IsNullOrEmpty(etag))
          step = fu.Http.Header("ETag", "\"" + etag + "\"")(step);

        // if the validator validates, send back a 304 Not Modified
        var ifNoneMatchHeader = ctx.Request.Headers["If-None-Match"];

        if (!string.IsNullOrEmpty(ifNoneMatchHeader)) {
          // note that the Select part is lazy-evaluated
          var existingETags = ifNoneMatchHeader
            .Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
            .Select(h => h.Trim())
            .Where(h => !string.IsNullOrEmpty(h) && h.Length > 2)
            .Select(h => h.Substring(1, h.Length - 2));

          // check wether there's meat (!= null) to match a wildcard
          // or that wether the client have a representation with the same etag
          if ((ifNoneMatchHeader == "*" && !string.IsNullOrEmpty(etag)) ||
            existingETags.Contains(etag)) {

            step = fu.Http.NotModified()(fu.EndAct);
          }
        }

        step(ctx);
      };
    }


    private static long roughCompare(DateTime d1, DateTime d2)
    {
      // eliminate differences smaller than 1 second interval
      var result = (d1 - d2).Ticks;
      result -= result % OneSecond;

      return result;
    }
  }
}
