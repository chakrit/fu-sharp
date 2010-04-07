
using System;
using System.Collections.Generic;
using System.Linq;

using Fu.Exceptions;

namespace Fu.Services
{
  public class ServiceBroker : IServiceBroker
  {
    private IDictionary<Type, IService> _serviceMap;


    public IEnumerable<IService> Services { get; private set; }

    public ServiceBroker(IEnumerable<IService> services)
    {
      Services = services;

      _serviceMap = new Dictionary<Type, IService>();
    }


    public T Get<T>(IFuContext c)
    {
      var service = getService<T>();
      if (service == null)
        throw new InvalidServiceTypeException(typeof(T));

      return service.GetServiceObject(c);
    }

    public bool CanGet<T>(IFuContext c)
    {
      var service = getService<T>();

      return service != null &&
        service.CanGetServiceObject(c);
    }


    // TODO: There's seem to be some GetHashCode optimization possible
    //       mentioned in Funq IoC source code... maybe we can use that?
    private IService<T> getService<T>()
    {
      var type = typeof(T);

      if (_serviceMap.ContainsKey(typeof(T)))
        return (IService<T>)_serviceMap[type];

      var service = Services
        .OfType<IService<T>>()
        .FirstOrDefault();

      _serviceMap[type] = service;
      return service;
    }
  }
}
