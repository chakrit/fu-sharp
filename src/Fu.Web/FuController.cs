
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


    protected void Handle(string urlRegex, params Continuation[] steps)
    {
      Handle(urlRegex, fu.Compose(steps));
    }


    public abstract void Initialize();
  }
}
