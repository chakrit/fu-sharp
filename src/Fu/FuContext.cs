
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

    public FuSettings Settings { get; private set; }

    public IEnumerable<IService> Services { get; private set; }
    public IWalkPath WalkPath { get; private set; }

    public FuContext(IFuContext c) :
      this(c.Settings, c.Services, c.Request, c.Response, c.WalkPath) { }

    public FuContext(FuSettings settings,
      IEnumerable<IService> services,
      HttpListenerContext httpContext,
      IWalkPath walkPath) :
      this(settings, services, httpContext.Request, httpContext.Response, walkPath) { }

    public FuContext(FuSettings settings,
      IEnumerable<IService> services,
      HttpListenerRequest request,
      HttpListenerResponse response,
      IWalkPath walkPath)
    {
      this.Request = request;
      this.Response = response;

      this.Settings = settings;

      this.Services = services;
      this.WalkPath = walkPath;
    }


    public T Get<T>()
    {
      var service = Services
        .OfType<IService<T>>()
        .FirstOrDefault(t => t.CanGetServiceObject(this));

      if (service == null)
        throw new InvalidServiceTypeException(typeof(T));

      return service.GetServiceObject(this);
    }

    public bool CanGet<T>()
    {
      return Services
        .OfType<IService<T>>()
        .Any(t => t.CanGetServiceObject(this));
    }


    public void Dispose()
    { Response.Close(); }
  }
}
