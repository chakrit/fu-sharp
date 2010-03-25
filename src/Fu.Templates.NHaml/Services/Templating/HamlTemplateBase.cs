
using Fu.Results;

using NHaml;

namespace Fu.Services.Templating
{
    public abstract class HamlTemplateBase : Template
    {
        public HamlResult Result { get; set; }
        public IFuContext Context { get; set; }
    }
}
