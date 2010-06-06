
using System;
namespace Fu.Services
{
  // Provides a caching mechanism for services which can only
  // be activated once per request (or which is costly to activate)
  // so the service object can be re-used multiple times inside
  // a single request
  public abstract class CachingServiceBase<T> : IService<T>
  {
    protected const string CanGetKey = "CanGet";
    protected const string ServiceObjectKey = "ServObj";


    public bool CanGetServiceObject(IFuContext input)
    {
      var lazyCanGet = input.Items.ContainsKey(this, CanGetKey) ?
        input.Items.Get<Lazy<bool>>(this, CanGetKey) :
        new Lazy<bool>(() => CanGetServiceObjectCore(input));

      return lazyCanGet.Value;
    }

    public T GetServiceObject(IFuContext input)
    {
      var lazyServObj = input.Items.ContainsKey(this, ServiceObjectKey) ?
        input.Items.Get<Lazy<T>>(this, ServiceObjectKey) :
        new Lazy<T>(() => GetServiceObjectCore(input));

      return lazyServObj.Value;
    }


    protected abstract bool CanGetServiceObjectCore(IFuContext input);
    protected abstract T GetServiceObjectCore(IFuContext input);
  }
}
