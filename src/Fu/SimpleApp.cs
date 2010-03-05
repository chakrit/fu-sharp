
using System;
using System.Diagnostics;

using Fu.Services;
using Fu.Steps;

namespace Fu
{
    // basic app which adds logging and the result framework support
    public class SimpleApp : App
    {
        public SimpleApp(params Step[] steps) : this(null, steps) { }

        public SimpleApp(FuSettings settings, params Step[] steps) :
            base(settings, new[] {
                new LoggingService()
            }, new[] {
                fu.Compose(steps),
                fu.Result.Render(),
            })
        {
            Trace.Listeners.Add(new TextWriterTraceListener(Console.Out));
        }
    }
}
