
using System.Collections.Generic;
using System.Linq;
using System.Net;

using Fu.Exceptions;
using Fu.Services;

namespace Fu
{
    public class Walker : IWalker
    {
        public FuSettings Settings { get; private set; }

        public IEnumerable<Step> Steps { get; private set; }
        public IEnumerable<IService> Services { get; private set; }

        public Walker(FuSettings settings, IEnumerable<IService> services, IEnumerable<Step> steps)
        {
            this.Settings = settings;

            this.Steps = steps;
            this.Services = services;
        }


        public void Walk(HttpListenerContext httpContext)
        {
            IWalkPath path = new WalkPath(Steps);
            IFuContext context = new FuContext(Settings, Services, httpContext, path);

            FuTrace.Request(context);

            foreach (var step in path)
            {
                // TODO: Evented I/O non-block style?
                // TODO: Add step compatibility checking
                try
                {
                    FuTrace.Step(step);
                    context = step(context);
                }
                catch (SkipStepException) { /* absorbed */ }
            }

            FuTrace.Response(context);
        }
    }
}
