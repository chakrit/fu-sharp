
using System.IO;

using Fu;

namespace WildcardSubdomains
{
    class Program
    {
        static void Main(string[] args)
        {
            var app = new App(null, null, new Step[] { EchoSubdomain });

            // accept all port 80 connection, regardless of domain
            app.Settings.Host = "*";
            app.Start();
        }

        static IFuContext EchoSubdomain(IFuContext c)
        {
            var msg = c.Request.Headers["host"];
            msg = "You are browsing from: " + msg;

            var sw = new StreamWriter(c.Response.OutputStream);
            sw.Write(msg);
            sw.Flush();

            return c;
        }
    }
}
