
using System;
using System.Collections.Generic;

using Fu.Services;

namespace Fu
{
  public interface IApp : IDisposable
  {
    FuSettings Settings { get; }
    Stats Stats { get; }

    IList<IService> Services { get; }
    FuAction Pipeline { get; }

    IServer Server { get; }

    void Start();
    void Stop();
  }
}
