
using System.Collections.Generic;

namespace Fu.Services
{
  public interface IServiceBroker
  {
    IEnumerable<IService> Services { get; }

    T Get<T>(IFuContext c);
    bool CanGet<T>(IFuContext c);
  }
}
