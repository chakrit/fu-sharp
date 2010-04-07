
using System.Collections.Generic;
using System.Linq;

using Fu.Contexts;

namespace Fu.Steps
{
  public static partial class Map
  {
    public static Step Url(this IMapSteps _, string pattern, Step step)
    { return _.Url(new UrlMap(pattern, step), null); }

    public static Step Url(this IMapSteps _, string pattern, Step step, Step step404)
    { return _.Url(new UrlMap(pattern, step), step404); }

    public static Step Url(this IMapSteps _, UrlMap mapping, Step step404)
    {
      step404 = step404 ?? fu.Http.NotFound();

      return fu.Branch(c => mapping.Pattern.IsMatch(c.Request.Url.AbsolutePath) ?
          mapping.Step : step404);
    }


    public static Step Urls(this IMapSteps _, params UrlMap[] mappings)
    { return _.Urls((IEnumerable<UrlMap>)mappings, null); }

    public static Step Urls(this IMapSteps _, IDictionary<string, Step> mappings)
    { return _.Urls(mappings.Select(kv => new UrlMap(kv.Key, kv.Value)), null); }

    public static Step Urls(this IMapSteps _, IDictionary<string, Step> mappings, Step step404)
    { return _.Urls(mappings.Select(kv => new UrlMap(kv.Key, kv.Value)), step404); }

    public static Step Urls(this IMapSteps _, IEnumerable<UrlMap> mappings, Step step404)
    {
      step404 = step404 ?? fu.Http.NotFound();

      return fu.Branch(c =>
      {
        var path = c.Request.Url.AbsolutePath;
        var urlMatch = mappings
          .Select(m => new { Mapping = m, Result = m.Pattern.Match(path) })
          .FirstOrDefault(m => m.Result.Success);

        if (urlMatch != null)
          return fu.Compose(
            cx => new UrlMappedContext(cx, urlMatch.Mapping, urlMatch.Result),
            urlMatch.Mapping.Step);

        // no mappings found
        return step404;
      });
    }
  }
}
