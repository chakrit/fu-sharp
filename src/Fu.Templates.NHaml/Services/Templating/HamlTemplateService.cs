
using System;

using NHaml;

namespace Fu.Services.Templating
{
    public class HamlTemplateService : IService<TemplateEngine>
    {
        private TemplateEngine _engine;


        public HamlTemplateService() : this(new TemplateEngine()) { }

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
