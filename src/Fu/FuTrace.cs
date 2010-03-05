
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
        public static void Step(string step, string msg)
        {
            output(TraceLevel.Verbose, @"  - {0}: {1}",
                step, msg);
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

            var line = string.Format("Fu: T#{0:000} {1,8} {2}",
                threadId, DateTime.Now.ToString("hh:MM:ss"), string.Format(format, args));

            switch (level)
            {
                case TraceLevel.Error: Trace.TraceError(line); break;
                case TraceLevel.Warning: Trace.TraceWarning(line); break;
                case TraceLevel.Info: Trace.TraceInformation(line); break;
                case TraceLevel.Verbose: Trace.WriteLine(line); break;

                case TraceLevel.Off:
                default: /* absorbed */
                    break;
            }
        }
    }
}
