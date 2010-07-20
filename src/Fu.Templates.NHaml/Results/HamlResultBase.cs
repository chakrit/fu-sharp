
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

using Fu.Services.Templating;

using NHaml;

namespace Fu.Results
{
  public abstract class HamlResultBase : ResultBase
  {
    public HamlResultBase()
    {
      MediaType = Mime.TextHtml;
      ContentLength64 = -1;
    }


    // should return filenames relative to template base paths specified in HamlTemplateService
    protected abstract IEnumerable<string> GetTemplateNames(IFuContext context);

    // should return types derived from HamlTemplateBase
    protected abstract Type GetTemplateType(IFuContext context);

    // ran just before rending the template, configure any properties here
    protected virtual void OnBeforeRender(IFuContext context, Template template)
    { /* no-op */ }


    public override long Render(IFuContext c, Stream output)
    {
      // TODO: Check first wether TemplateEngine is supported
      //       throw an exception, if it's not
      // TODO: Implement a proper exception system that make senses
      var engine = c.Get<TemplateEngine>();

      var templateNames = GetTemplateNames(c).Reverse();
      var templateType = GetTemplateType(c);
      var template = GetTemplate(c, templateNames, templateType);

      OnBeforeRender(c, template);


      var encoding = Encoding.GetEncoding(c.Settings.Encoding);
      var sw = new StreamWriter(output, encoding);
      template.Render(sw);

      sw.Flush();
      sw.Close();

      // TODO: Support content-length, if possible
      return -1;
    }


    protected Template GetTemplate(IFuContext context,
      IEnumerable<string> templateNames, Type templateType)
    {
      if (!typeof(HamlTemplateBase).IsAssignableFrom(templateType))
        throw new InvalidOperationException(
          "Template types must derive from HamlTemplateBase");

      // NOTE: TemplateEngine already maintains a caching system
      var engine = context.Get<TemplateEngine>();
      var compiled = engine.Compile(templateNames.ToList(), templateType);

      var instance = (HamlTemplateBase)compiled.CreateInstance();
      instance.Context = context;
      instance.Result = this;

      return instance;
    }
  }
}
