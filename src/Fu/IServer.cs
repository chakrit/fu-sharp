﻿
using System;

using Fu.Services;

namespace Fu
{
  public interface IServer : IDisposable
  {
    FuSettings Settings { get; }
    Stats Stats { get; }

    RequestHandler Handler { get; }
    bool IsServing { get; }

    void Start();
    void Stop();
  }
}
