
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

        IEnumerable<IService> Services { get; }
        IWalkPath WalkPath { get; }

        // gets service object of type T
        T Get<T>();
        bool CanGet<T>();
    }
}
