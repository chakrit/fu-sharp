
using System;
using System.Diagnostics;
using System.Linq;

using Fu.Steps;

namespace Fu.Presets
{
  // basic app which adds logging and the result framework support
  public class SimpleApp : App
  {
    private TraceListener _listener;


    public SimpleApp(params Continuation[] pipeline) : this(null, pipeline) { }

    public SimpleApp(FuSettings settings, params Continuation[] pipeline) :
      base(settings, null, fu.Compose(pipeline, fu.Result.Render()))
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
