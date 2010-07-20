
using System.Linq;
using System.Text.RegularExpressions;

using Fu.Contexts;

using ContDict = System.Collections.Generic.IDictionary<string, Fu.Continuation>;
using ContRegexDict = System.Collections.Generic.IDictionary<System.Text.RegularExpressions.Regex, Fu.Continuation>;

namespace Fu.Steps
{
  public static partial class Map
  {
    public static Continuation Url(this IMapSteps _, string pattern)
    { return _.Url(pattern, null); }

    public static Continuation Url(this IMapSteps _, string pattern,
      Continuation on404)
    {
      on404 = on404 ?? fu.Http.NotFound();

      return step => ctx =>
      {
        var match = Regex.Match(ctx.Request.Url.AbsolutePath, pattern);

        (match.Success ? step : on404(step))
          (new UrlMappedContext(ctx, match));
      };
    }

    public static Continuation Url(this IMapSteps _, string pattern,
      Continuation urlStep, Continuation on404)
    {
      on404 = on404 ?? fu.Http.NotFound();

      return step => ctx =>
      {
        var match = Regex.Match(ctx.Request.Url.AbsolutePath, pattern);

        (match.Success ? urlStep : on404)(step)
          (new UrlMappedContext(ctx, match));
      };
    }


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
      return _.Custom(mappings, ctx => ctx.Request.Url.AbsolutePath, on404);
    }


    public static Continuation Hosts(this IMapSteps _,
      ContDict mappings)
    {
      return _.Hosts(mappings, null);
    }

    public static Continuation Hosts(this IMapSteps _,
      ContDict mappings, Continuation on404)
    {
      var dict = mappings.ToDictionary(
        kv => new Regex(kv.Key),
        kv => kv.Value);

      return _.Hosts(dict, on404);
    }

    public static Continuation Hosts(this IMapSteps _,
      ContRegexDict mappings)
    {
      return _.Hosts(mappings);
    }

    public static Continuation Hosts(this IMapSteps _,
      ContRegexDict mappings, Continuation on404)
    {
      return _.Custom(mappings, ctx => ctx.Request.Headers["Host"], on404);
    }


    public static Continuation Custom(this IMapSteps _,
      ContDict mappings, Reduce<string> pathReducer)
    {
      return _.Custom(mappings, pathReducer, null);
    }

    public static Continuation Custom(this IMapSteps _,
      ContDict mappings, Reduce<string> pathReducer, Continuation on404)
    {
      var dict = mappings.ToDictionary(
        kv => new Regex(kv.Key),
        kv => kv.Value);

      return _.Custom(dict, pathReducer, on404);
    }

    public static Continuation Custom(this IMapSteps _,
      ContRegexDict mappings, Reduce<string> pathReducer)
    {
      return _.Custom(mappings, pathReducer, null);
    }

    public static Continuation Custom(this IMapSteps _,
      ContRegexDict mappings,
      Reduce<string> pathReducer, Continuation on404)
    {
      on404 = on404 ?? fu.Http.NotFound();

      return step => ctx =>
      {
        var path = pathReducer(ctx);
        var result = mappings
          .Select(kv => new { Match = kv.Key.Match(path), Cont = kv.Value })
          .FirstOrDefault(m => m.Match.Success);

        if (result == null)
          on404(step)(ctx);
        else
          result.Cont(step)(new UrlMappedContext(ctx, result.Match));
      };

    }
  }
}
