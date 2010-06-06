
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
      if (input.Items.ContainsKey(this, CanGetKey))
        return input.Items.Get<bool>(this, CanGetKey);

      var result = CanGetServiceObjectCore(input);
      input.Items.Set(this, CanGetKey, result);

      return result;
    }

    public T GetServiceObject(IFuContext input)
    {
      if (input.Items.ContainsKey(this, ServiceObjectKey))
        return input.Items.Get<T>(this, ServiceObjectKey);

      var result = GetServiceObjectCore(input);
      input.Items.Set(this, ServiceObjectKey, result);

      return result;
    }


    protected abstract bool CanGetServiceObjectCore(IFuContext input);
    protected abstract T GetServiceObjectCore(IFuContext input);
  }
}
