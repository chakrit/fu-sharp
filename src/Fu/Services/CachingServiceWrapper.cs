
using System;

namespace Fu.Services
{
  // a special version of CachingServiceBase that lets you
  // caches another service
  public class CachingServiceWrapper<T> : CachingServiceBase<T>, IService<T>
  {
    private IService<T> _service;


    public IService<T> UnderlyingService { get { return _service; } }


    public CachingServiceWrapper(IService<T> service)
    {
      _service = service;
    }

    public static CachingServiceWrapper<TX> CreateFrom<TX>(IService<TX> service)
    { return new CachingServiceWrapper<TX>(service); }


    protected override bool CanGetServiceObjectCore(IFuContext input)
    {
      return _service.CanGetServiceObject(input);
    }

    protected override T GetServiceObjectCore(IFuContext input)
    {
      return _service.GetServiceObject(input);
    }
  }
}
