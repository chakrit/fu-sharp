
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
    // TODO: Abstract out HttpListenerRequest
    public HttpListenerRequest Request { get; private set; }
    public HttpListenerResponse Response { get; private set; }

    public IServiceBroker Services { get; private set; }
    public FuSettings Settings { get; private set; }

    public FuContext(IFuContext c) :
      this(c.Settings, c.Services, c.Request, c.Response) { }

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

      this.Services = services;
      this.Settings = settings;
    }


    public T As<T>() where T : class, IFuContext
    {
      var result = this as T;
      if (result == null)
        throw new MismatchedContextTypeException(
          typeof(this), typeof(T));

      return result;
    }


    public T Get<T>()
    {
      return Services.Get<T>(this);
    }

    public bool CanGet<T>()
    {
      return Services.CanGet<T>(this);
    }


    public void Dispose()
    { Response.Close(); }
  }
}
