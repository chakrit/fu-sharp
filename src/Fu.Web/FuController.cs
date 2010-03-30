
using System.Collections.Generic;
using System.Linq;

namespace Fu
{
    public abstract class FuController : IFuController
    {
        public IList<Step> PreSteps { get; protected set; }
        public IList<Step> PostSteps { get; protected set; }

        public IDictionary<string, Step> Mappings { get; protected set; }

        public FuController()
        {
            PreSteps = new List<Step>();
            PostSteps = new List<Step>();

            Mappings = new Dictionary<string, Step>();
        }


        protected void Handle(string urlRegex, params Step[] steps)
        { Handle(urlRegex, fu.Compose(steps)); }

        protected void Handle(string urlRegex, Step step)
        {
            Mappings.Add(urlRegex, step);
        }


        public abstract void Initialize();
    }
}
