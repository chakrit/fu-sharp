
using System;

using Fu;
using Fu.Contexts;
using Fu.Results;
using Fu.Steps;

namespace RestStyle
{
  public abstract class RestController : FuController
  {
    public void Get(string url, Func<IUrlMappedContext, IResult> handler)
    { map(url, fu.Map.Get, handler); }

    public void Get(string url, Continuation handler)
    { map(url, fu.Map.Get, handler); }

    public void Put(string url, Func<IUrlMappedContext, IResult> handler)
    { map(url, fu.Map.Put, handler); }

    public void Put(string url, Continuation handler)
    { map(url, fu.Map.Put, handler); }

    public void Post(string url, Func<IUrlMappedContext, IResult> handler)
    { map(url, fu.Map.Post, handler); }

    public void Post(string url, Continuation handler)
    { map(url, fu.Map.Post, handler); }

    public void Delete(string url, Func<IUrlMappedContext, IResult> handler)
    { map(url, fu.Map.Delete, handler); }

    public void Delete(string url, Continuation handler)
    { map(url, fu.Map.Delete, handler); }


    private void map(string url,
      Func<Continuation, Continuation, Continuation> wrapper,
      Func<IUrlMappedContext, IResult> handler)
    {
      // converts context to ResultContext using result from the handler
      // and wraps in a simple Continuation
      map(url, wrapper, step => ctx =>
      {
        var urlMapped = ctx.As<IUrlMappedContext>();
        var result = handler(urlMapped);

        step(new ResultContext(ctx, result));
      });
    }

    private void map(string url,
      Func<Continuation, Continuation, Continuation> wrapper,
      Continuation handler)
    {
      // safeguard the url
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
