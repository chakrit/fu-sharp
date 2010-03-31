
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
      IEnumerable<Step> steps) :
      base(settings ?? FuSettings.Default,
        services ?? new IService[] { },
        steps ?? new Step[] { }) { }


    protected override IWalker CreateWalkerCore()
    {
      return new Walker(Settings, Services, Steps);
    }

    protected override IServer CreateServerCore(IWalker walker)
    {
      return new Server(Settings, walker);
    }
  }
}
