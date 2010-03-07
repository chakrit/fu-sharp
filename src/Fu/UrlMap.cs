
using System.Text.RegularExpressions;

namespace Fu
{
    public class UrlMap
    {
        public Regex Pattern { get; private set; }
        public Step Step { get; private set; }

        public UrlMap(string pattern, Step step) :
            this(new Regex(pattern, StrComp.FastRx), step) { }

        public UrlMap(string pattern, params Step[] steps) :
            this(new Regex(pattern, StrComp.FastRx), fu.Compose(steps)) { }

        public UrlMap(Regex pattern, params Step[] steps) :
            this(pattern, fu.Compose(steps)) { }

        public UrlMap(Regex pattern, Step step)
        {
            this.Pattern = pattern;
            this.Step = step;
        }
    }
}
