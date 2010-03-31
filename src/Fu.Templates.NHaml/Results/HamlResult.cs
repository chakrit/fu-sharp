
using System.Collections.Generic;
using System.Linq;

using Fu.Services.Templating;

namespace Fu.Results
{
  public class HamlResult : HamlResultBase
  {
    private static string[] noLayouts = new string[] { };

    public string HamlFilename { get; protected set; }
    public string[] LayoutFilenames { get; protected set; }


    public HamlResult(string hamlFilename) : this(hamlFilename, noLayouts) { }

    public HamlResult(string hamlFilename, params string[] layouts)
    {
      HamlFilename = hamlFilename;
      LayoutFilenames = layouts;
    }


    protected override IEnumerable<string> GetTemplateNames(IFuContext context)
    { return new[] { HamlFilename }.Concat(LayoutFilenames); }

    protected override System.Type GetTemplateType(IFuContext context)
    { return typeof(HamlTemplateBase); }

  }
}
