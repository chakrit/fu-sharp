
using System.Collections.Generic;
using System.Net;

using Fu.Services;

namespace Fu
{
  public interface IFuContext
  {
    HttpListenerRequest Request { get; }
    HttpListenerResponse Response { get; }

    IServiceBroker Services { get; }
    IItemsStore Items { get; }
    FuSettings Settings { get; }


    // conversion helper
    T As<T>() where T : class, IFuContext;


    // gets service object of type T
    T Get<T>();
    bool CanGet<T>();
  }
}
