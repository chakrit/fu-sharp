
using System;
using System.Collections.Generic;
using System.Linq;

using Fu.Exceptions;

namespace Fu.Services
{
  public class ServiceBroker : IServiceBroker
  {
    public IEnumerable<IService> Services { get; private set; }

    public ServiceBroker(IEnumerable<IService> services)
    {
      Services = services;
    }


    public T Get<T>(IFuContext c)
    {
      var service = getService<T>(c);
      if (service == null)
        throw new InvalidServiceTypeException(typeof(T));

      return service.GetServiceObject(c);
    }

    public bool CanGet<T>(IFuContext c)
    {
      var service = getService<T>(c);

      return service != null &&
        service.CanGetServiceObject(c);
    }


    // TODO: There's seem to be some GetHashCode optimization possible
    //       mentioned in Funq IoC source code... maybe we can use that?
    private IService<T> getService<T>(IFuContext c)
    {
      return Services
        .OfType<IService<T>>()
        .FirstOrDefault(srv => srv.CanGetServiceObject(c));
    }
  }
}
