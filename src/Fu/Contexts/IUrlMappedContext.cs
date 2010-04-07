
using System.Text.RegularExpressions;

namespace Fu.Contexts
{
  public interface IUrlMappedContext : IFuContext
  {
    Match Match { get; }
  }
}
