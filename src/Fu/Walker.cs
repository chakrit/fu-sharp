
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

            foreach (var serv in Services)
                serv.BeginWalk(context);

            foreach (var step in path)
            {
                // TODO: Break this one so each is non-blocking and stoppable
                //       then proceeds to make a proper evented I/O
                // TODO: Add step compatibility checking
                try { context = step(context); }
                catch (SkipStepException) { /* absorbed */ }
            }

            // the first service to gets the BeginWalk call should be the last
            // to gets the EndWalk call so there're less interferance with each other
            foreach (var serv in Services.Reverse())
                serv.EndWalk(context);
        }
    }
}
