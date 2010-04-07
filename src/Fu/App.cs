
using System;
using System.Collections.Generic;
using System.Linq;

using Fu.Services;

namespace Fu
{
  public class App : AppBase
  {
    public App(FuSettings settings,
      IEnumerable<IService> services,
      params Continuation[] pipeline) :
      base(settings, services, fu.Compose(pipeline)(fu.EndAct)) { }


    protected override RequestHandler CreateHandlerCore()
    {
      var broker = new ServiceBroker(Services);

      return c =>
      {
        var context = new FuContext(Settings, broker, c);
        Pipeline(context);
      };
    }

    protected override IServer CreateServerCore(RequestHandler handler)
    {
      return new Server(Settings, handler);
    }
  }
}
