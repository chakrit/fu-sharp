
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;

using Fu.Exceptions;
using Fu.Services;

namespace Fu
{
  public class FuContext : IFuContext, IDisposable
  {
    private IServiceBroker _services;

    // TODO: Abstract out HttpListenerRequest
    public HttpListenerRequest Request { get; private set; }
    public HttpListenerResponse Response { get; private set; }

    public FuSettings Settings { get; private set; }

    public FuContext(IFuContext c) :
      this(c.Settings, c.Services, c.Request, c.Response, c.WalkPath) { }

    public FuContext(FuSettings settings,
      IServiceBroker services,
      HttpListenerContext httpContext) :
      this(settings, services, httpContext.Request, httpContext.Response) { }

    public FuContext(FuSettings settings,
      IServiceBroker services,
      HttpListenerRequest request,
      HttpListenerResponse response)
    {
      this.Request = request;
      this.Response = response;

      this.Settings = settings;

      _services = services;
    }


    public T Get<T>()
    {
      return _services.Get<T>(this);
    }

    public bool CanGet<T>()
    {
      return _services.CanGet<T>(this);
    }


    public void Dispose()
    { Response.Close(); }
  }
}
