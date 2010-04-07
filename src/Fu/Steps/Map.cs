
using System.Linq;
using System.Text.RegularExpressions;

using Fu.Contexts;

using ContDict = System.Collections.Generic.IDictionary<string, Fu.Continuation>;
using ContRegexDict = System.Collections.Generic.IDictionary<
  System.Text.RegularExpressions.Regex, Fu.Continuation>;

namespace Fu.Steps
{
  public static partial class Map
  {
    public static Continuation Urls(this IMapSteps _,
      ContDict mappings)
    {
      return _.Urls(mappings, null);
    }

    public static Continuation Urls(this IMapSteps _,
      ContDict mappings, Continuation on404)
    {
      var dict = mappings.ToDictionary(
        kv => new Regex(kv.Key),
        kv => kv.Value);

      return _.Urls(dict, on404);
    }

    public static Continuation Urls(this IMapSteps _,
      ContRegexDict mappings)
    {
      return _.Urls(mappings, null);
    }

    public static Continuation Urls(this IMapSteps _,
      ContRegexDict mappings, Continuation on404)
    {
      on404 = on404 ?? fu.Http.NotFound();

      return step => ctx =>
      {
        var path = ctx.Request.Url.AbsolutePath;
        var result = mappings
          .Select(kv => new { Match = kv.Key.Match(path), Cont = kv.Value })
          .FirstOrDefault(m => m.Match.Success);

        (result == null ? on404 : result.Cont)
          (step)
          (new UrlMappedContext(ctx, result.Match));
      };
    }
  }
}
