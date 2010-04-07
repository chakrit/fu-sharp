
using System.Text.RegularExpressions;

namespace Fu
{
  public class UrlMap
  {
    public Regex Pattern { get; private set; }
    public Continuation Continuation { get; private set; }

    public UrlMap(string pattern, Continuation cont) :
      this(new Regex(pattern, StrComp.FastRx), cont) { }

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
