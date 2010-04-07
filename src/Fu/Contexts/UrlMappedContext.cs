
using System.Text.RegularExpressions;

namespace Fu.Contexts
{
  public class UrlMappedContext : FuContext, IUrlMappedContext
  {
    public Match Match { get; protected set; }

    public UrlMappedContext(IFuContext input, Match result) :
      base(input)
    {
      Match = result;
    }
  }
}
