
using Fu.Results;

using NHaml;

namespace Fu.Services.Templating
{
    public class HamlTemplateBase : Template
    {
        public HamlResultBase Result { get; set; }
        public IFuContext Context { get; set; }
    }
}
