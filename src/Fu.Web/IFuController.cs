
using System.Collections.Generic;

namespace Fu
{
  public interface IFuController
  {
    IDictionary<string, Continuation> Mappings { get; }

    void Initialize();
  }
}
