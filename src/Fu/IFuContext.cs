
using System.Collections.Generic;
using System.Net;

using Fu.Services;

namespace Fu
{
  public interface IFuContext
  {
    HttpListenerRequest Request { get; }
    HttpListenerResponse Response { get; }

    FuSettings Settings { get; }


    // gets service object of type T
    T Get<T>();
    bool CanGet<T>();
  }
}
