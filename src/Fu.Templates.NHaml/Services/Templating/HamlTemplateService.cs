
using System;

using NHaml;
using NHaml.TemplateResolution;

namespace Fu.Services.Templating
{
    public class HamlTemplateService : IService<TemplateEngine>
    {
        private TemplateEngine _engine;


        public HamlTemplateService() : this(new TemplateEngine()) { }

        public HamlTemplateService(string templateBasePath)
            : this(new TemplateEngine())
        {
            var provider = new FileTemplateContentProvider();

            provider.PathSources.Clear();
            provider.AddPathSource(templateBasePath);
        }

        public HamlTemplateService(string[] templatePaths)
        {
            var provider = new FileTemplateContentProvider();

            provider.PathSources.Clear();
            Array.ForEach(templatePaths, provider.AddPathSource);
        }

        public HamlTemplateService(TemplateOptions options)
            : this(new TemplateEngine(options)) { }

        public HamlTemplateService(TemplateEngine engine)
        {
            if (engine == null)
                throw new ArgumentNullException("engine");

            _engine = engine;
        }


        public bool CanGetServiceObject(IFuContext input) { return true; }

        public TemplateEngine GetServiceObject(IFuContext input) { return _engine; }


        public void BeginWalk(IFuContext input) { /* no-op */ }
        public void EndWalk(IFuContext input) { /* no-op */ }
    }
}
