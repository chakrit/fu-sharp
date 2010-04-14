
using System.IO;

using Fu;

namespace WildcardSubdomains
{
  class Program
  {
    static void Main(string[] args)
    {
      var app = new App(null, null, echoSubDomain());

      // accept all port 80 connection, regardless of domain
      app.Settings.Hosts = new[] { "*:80" };
      app.Start();
    }

    private static Continuation echoSubDomain()
    {
      return step => ctx =>
      {
        var msg = ctx.Request.Headers["host"];
        msg = "You are browsing from: " + msg;

        var sw = new StreamWriter(ctx.Response.OutputStream);
        sw.Write(msg);
        sw.Flush();

        step(ctx);
      };
    }
  }
}
