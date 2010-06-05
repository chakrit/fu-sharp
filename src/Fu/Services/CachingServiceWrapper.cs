
using System;

namespace Fu.Services
{
  // Provides a caching mechanism for services which can only
  // be activated once per request (or which is costly to activate)
  // so the service object can be re-used multiple times inside
  // a single request
  public class CachingServiceWrapper<T> : IService<T>
  {
    // TODO: We could just use a Lazy<T> but the type seems to be
    //       conflicted between FSharp.Core and mscorlib;
    private IService<T> _service;
    private bool _activated;
    private T _servObj;


    public IService<T> UnderlyingService { get { return _service; } }


    public CachingServiceWrapper(IService<T> service)
    {
      _service = service;
      _activated = false;
    }

    public static CachingServiceWrapper<TX> CreateFrom<TX>(IService<TX> service)
    { return new CachingServiceWrapper<TX>(service); }


    public bool CanGetServiceObject(IFuContext input)
    { return _service.CanGetServiceObject(input); }

    public T GetServiceObject(IFuContext input)
    {
      if (_activated) return _servObj;
      return _servObj = _service.GetServiceObject(input);
    }
  }
}
