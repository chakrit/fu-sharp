
using System;

using Fu.Steps;
using Fu.Contexts;

using ResultStep = System.Func<Fu.Contexts.IUrlMappedContext, Fu.Results.IResult>;

namespace Fu
{
  // Was tempted to name this SinatraStyleController
  // but not everyone meddle in the Ruby world...
  public abstract class RestStyleController : FuController
  {
    public void Get(string url, ResultStep handler)
    {
      mapResult(url, fu.Map.Get, handler);
    }

    public void Get(string url, Continuation handler)
    {
      Map(url, fu.Map.Get, handler);
    }

    public void Post(string url, ResultStep handler)
    {
      mapResult(url, fu.Map.Post, handler);
    }

    public void Post(string url, Continuation handler)
    {
      Map(url, fu.Map.Post, handler);
    }

    public void Put(string url, ResultStep handler)
    {
      mapResult(url, fu.Map.Put, handler);
    }

    public void Put(string url, Continuation handler)
    {
      Map(url, fu.Map.Put, handler);
    }

    public void Delete(string url, ResultStep handler)
    {
      mapResult(url, fu.Map.Delete, handler);
    }

    public void Delete(string url, Continuation handler)
    {
      Map(url, fu.Map.Delete, handler);
    }


    private void mapResult(string url,
      Func<Continuation, Continuation, Continuation> wrapper,
      ResultStep handler)
    {
      // wraps result step in a simple continuation
      Map(url, wrapper, step => ctx =>
      {
        var urlMapped = ctx.As<IUrlMappedContext>();
        var result = handler(urlMapped);

        step(new ResultContext(ctx, result));
      });
    }

    protected virtual void Map(string url,
      Func<Continuation, Continuation, Continuation> wrapper,
      Continuation handler)
    {
      // safeguard url against partial matches
      if (!url.StartsWith("^") && !url.EndsWith("$"))
        url = "^" + url + "$";

      if (!Mappings.ContainsKey(url))
        // if first mapped this url, wrong method = error
        Mappings[url] = wrapper(handler, fu.Http.MethodNotAllowed());
      else
        // if already mapped this url, wrong method = try the previous method
        Mappings[url] = wrapper(handler, Mappings[url]);
    }
  }
}
