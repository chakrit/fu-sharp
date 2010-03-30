
using System;
using System.Diagnostics;
using System.Threading;

namespace Fu
{
    public static class FuTrace
    {
        [Conditional("TRACE")]
        public static void Exception(Exception ex)
        {
            output(TraceLevel.Error, @"E - {0}: {1}",
                ex.StackTrace.Split('\n')[0].Trim(),
                ex.Message);
        }

        [Conditional("TRACE")]
        public static void Step(Delegate step)
        {
            output(TraceLevel.Verbose, @"  - {0}: {1}",
                step.Method.Module, step.Method.Name);
        }

        [Conditional("TRACE")]
        public static void Session(string action, string sessionId)
        {
            output(TraceLevel.Info, @"S - {0}: {1}",
                action, sessionId);
        }

        [Conditional("TRACE")]
        public static void Request(IFuContext c)
        {
            output(TraceLevel.Info, @"> {0} {1}",
                c.Request.HttpMethod,
                c.Request.Url.PathAndQuery);
        }

        [Conditional("TRACE")]
        public static void Response(IFuContext c)
        {
            output(TraceLevel.Info, @"< {0} - {1} bytes of {2}",
                c.Response.StatusCode,
                c.Response.ContentLength64,
                c.Response.ContentType);
        }

        [Conditional("DEBUG"), Conditional("TRACE")]
        public static void Debug(string dbgMessage)
        {
            Trace.WriteLine(dbgMessage, "FU-DEBUG");
        }

        [Conditional("TRACE")]
        public static void Fail(string failMessage)
        {
            Trace.Fail(failMessage);
        }

        [Conditional("TRACE")]
        private static void output(TraceLevel level, string format, params object[] args)
        {
            var threadId = Thread.CurrentThread.ManagedThreadId;
            var levelName = Enum.GetName(typeof(TraceLevel), level).PadRight(7);
            var timestamp = DateTime.Now.ToString("hh:MM:ss");
            var msg = string.Format(format, args);

            var line = string.Format("T#{0:000} {1,7} {2,8} {3}",
                threadId, levelName, timestamp, msg);

            Trace.WriteLine(line, "Fu#");
        }
    }
}
