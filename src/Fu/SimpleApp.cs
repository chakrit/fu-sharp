﻿
using System;
using System.Diagnostics;

using Fu.Steps;
using Fu.Services;

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
                fu.Render.Result()
            })
        {
            Trace.Listeners.Add(new TextWriterTraceListener(Console.Out));
        }
    }
}
