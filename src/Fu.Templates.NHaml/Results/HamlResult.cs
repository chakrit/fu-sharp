﻿
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Mime;
using System.Text;

using NHaml;

using Fu.Services.Templating;

namespace Fu.Results
{
    public abstract class HamlResult : IResult
    {
        private ContentType _contentType;


        public HamlResult()
        {
            _contentType = new ContentType { MediaType = Mime.TextHtml };
        }


        protected abstract IEnumerable<string> GetTemplateNames(IFuContext context);
        protected abstract Type GetTemplateType(IFuContext context);


        // implement IResult interface explicitly so they're not
        // directly accessible from Haml page
        ContentType IResult.ContentType { get { return _contentType; } }

        byte[] IResult.RenderBytes(IFuContext c)
        {
            var engine = c.Get<TemplateEngine>();

            var templateNames = GetTemplateNames(c).Reverse();
            var templateType = GetTemplateType(c);
            var template = GetTemplate(c, templateNames, templateType);

            // render the template
            var encoding = Encoding.GetEncoding(c.Settings.Encoding);

            var ms = new MemoryStream();
            var sw = new StreamWriter(ms, encoding);
            template.Render(sw);

            sw.Close();
            sw.Dispose();

            var result = ms.ToArray();
            ms.Close();
            ms.Dispose();

            return result;
        }


        protected Template GetTemplate(IFuContext context, IEnumerable<string> templateNames, Type templateType)
        {
            if (!typeof(HamlTemplateBase).IsAssignableFrom(templateType))
                throw new InvalidOperationException(
                    "Template types must derive from HamlTemplateBase");

            // NOTE: TemplateEngine already has caching mechanism
            var engine = context.Get<TemplateEngine>();
            var compiled = engine.Compile(templateNames.ToList(), templateType);

            var instance = (HamlTemplateBase)compiled.CreateInstance();
            instance.Context = context;
            instance.Result = this;

            return instance;
        }
    }
}
