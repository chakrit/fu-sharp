
using System;
using System.Collections.Generic;
using System.Linq;

using Fu.Services;

namespace Fu
{
  public abstract class AppBase : IApp
  {
    private IServer _server;


    public FuSettings Settings { get; private set; }
    public Stats Stats { get { return _server.Stats; } }

    public IList<IService> Services { get; private set; }
    public FuAction Pipeline { get; private set; }

    public IServer Server { get { return _server; } }


    public AppBase(FuSettings settings, IEnumerable<IService> services, FuAction pipeline)
    {
      services = services ?? new IService[] { };
      settings = settings ?? FuSettings.Default;

      pipeline = pipeline ?? fu.End;

      this.Services = services.ToList();
      this.Settings = settings;
    }


    public void Start()
    {
      // validate settings and fill in any defaults
      if (string.IsNullOrEmpty(Settings.BasePath))
        Settings.BasePath = Environment.CurrentDirectory;

      var handler = CreateHandlerCore();
      _server = CreateServerCore(handler);

      _server.Start();
    }

    protected abstract RequestHandler CreateHandlerCore();
    protected abstract IServer CreateServerCore(RequestHandler handler);


    public void Stop()
    {
      _server.Stop();
    }


    public void Dispose()
    {
      if (_server != null)
        _server.Dispose();
    }
  }
}
