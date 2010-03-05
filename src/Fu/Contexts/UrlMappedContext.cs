
using System.Text.RegularExpressions;

namespace Fu.Contexts
{
    public class UrlMappedContext : FuContext, IUrlMappedContext
    {
        public Match Match { get; protected set; }
        public UrlMap Mapping { get; protected set; }

        public UrlMappedContext(IFuContext input, UrlMap mapping, Match result) :
            base(input)
        {
            Mapping = mapping;
            Match = result;
        }
    }
}
