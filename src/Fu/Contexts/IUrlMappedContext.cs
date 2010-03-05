
using System.Text.RegularExpressions;

using Fu.Steps;

namespace Fu.Contexts
{
    public interface IUrlMappedContext : IFuContext
    {
        Match Match { get; }
        UrlMap Mapping { get; }
    }
}
