
using System;

namespace Fu
{
  public interface IServer : IDisposable
  {
    FuSettings Settings { get; }
    Stats Stats { get; }
    string Url { get; }

    IWalker Walker { get; }
    bool IsServing { get; }

    void Start();
    void Stop();
  }
}
