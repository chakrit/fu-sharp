
using System;
using System.Collections.Generic;

namespace Fu
{
  public abstract class FuController : IFuController
  {
    public IDictionary<string, Continuation> Mappings { get; protected set; }

    public FuController()
    {
      Mappings = new Dictionary<string, Continuation>();
    }


    [Obsolete("RestStyleController is a better alternative for most cases. " +
      "Otherwise you can roll your own controller style or manually add " +
      "Continuation(s) to FuController.Mappings on Initialize().")]
    protected virtual void Handle(string urlRegex, params Continuation[] steps)
    {
      Mappings.Add(urlRegex, fu.Compose(steps));
    }


    public abstract void Initialize();
  }
}
