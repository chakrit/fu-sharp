
using System.Collections.Generic;
using System.Linq;

namespace Fu
{
    public class FuController : IFuController
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


        // fluent mapping entry point
        protected HandlerSyntax Handle(params string[] urls)
        {
            return new HandlerSyntax
            {
                Parent = this,
                Urls = urls.ToList()
            };
        }


        protected class HandlerSyntax
        {
            public FuController Parent { get; set; }
            public IList<string> Urls { get; set; }


            // fluent mapping end point
            public void With(Step step)
            {
                foreach (var url in Urls)
                    Parent.Mappings.Add(url, step);
            }

            public void With(params Step[] steps)
            {
                var step = fu.Compose(steps);
                foreach (var url in Urls)
                    Parent.Mappings.Add(url, step);
            }
        }
    }
}
