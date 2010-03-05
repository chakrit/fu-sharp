
using System;
using System.Collections.Generic;

using Fu.Services;

namespace Fu
{
    public interface IApp : IDisposable
    {
        FuSettings Settings { get; }
        Stats Stats { get; }

        IList<Step> Steps { get; }
        IList<IService> Services { get; }

        IServer Server { get; }

        void Start();
        void Stop();
    }
}
