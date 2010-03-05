
using System.Text.RegularExpressions;

namespace Fu
{
    public class UrlMap
    {
        public Regex Pattern { get; private set; }
        public Step Step { get; private set; }

        public UrlMap(string pattern, Step step)
            : this(new Regex(pattern, StrCmp.FastRx), step) { }

        public UrlMap(Regex pattern, Step step)
        {
            this.Pattern = pattern;
            this.Step = step;
        }
    }
}
