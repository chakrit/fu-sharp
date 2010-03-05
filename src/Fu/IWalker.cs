
using System.Collections.Generic;
using System.Net;

using Fu.Services;

namespace Fu
{
    public interface IWalker
    {
        FuSettings Settings { get; }

        IEnumerable<Step> Steps { get; }
        IEnumerable<IService> Services { get; }

        void Walk(HttpListenerContext httpContext);
    }
}
