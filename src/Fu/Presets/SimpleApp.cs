
using System;
using System.Diagnostics;
using System.Linq;

using Fu.Services;
using Fu.Steps;

namespace Fu.Presets
{
    // basic app which adds logging and the result framework support
    public class SimpleApp : App
    {
        private TraceListener _listener;


        public SimpleApp(params Step[] steps) : this(null, steps) { }

        public SimpleApp(FuSettings settings, params Step[] steps) :
            base(settings, null, steps.Concat(new[] { fu.Result.Render() }).ToArray())
        {
            _listener = new TextWriterTraceListener(Console.Out);
            EnableConsoleOutput();
        }


        public void EnableConsoleOutput()
        {
            if (!Trace.Listeners.Contains(_listener))
                Trace.Listeners.Add(_listener);
        }

        public void DisableConsoleOutput()
        {
            if (Trace.Listeners.Contains(_listener))
                Trace.Listeners.Remove(_listener);
        }
    }
}
