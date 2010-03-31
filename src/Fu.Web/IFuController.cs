
using System.Collections.Generic;

namespace Fu
{
  public interface IFuController
  {
    IList<Step> PreSteps { get; }
    IList<Step> PostSteps { get; }

    IDictionary<string, Step> Mappings { get; }

    void Initialize();
  }
}
