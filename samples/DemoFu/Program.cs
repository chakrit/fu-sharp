
using System.Linq;
using System.Text;

using Fu;
using Fu.Results;
using Fu.Services;
using Fu.Services.Sessions;
using Fu.Services.Web;
using Fu.Steps;

using FuDemo;

namespace DemoFu
{
    internal partial class Program
    {
        static void Main(string[] args)
        {
            var app = new SimpleWebApp(fu.Map.Urls(
                new UrlMap("^/$", fu.Static.File("Upload.html")),
                new UrlMap("^/echo$", echo),
                new UrlMap("^/generator$", testGenerator)));

            var sessionService = app.Services
                .OfType<IService<ISession>>()
                .FirstOrDefault();

            app.Services.Remove(sessionService);
            app.Services.Add(new SessionService<IMySession>(
                new SHA1SessionIdProvider(),
                new DictionarySessionStore()));

            app.Settings.Host = "*";
            app.Start();
        }

        private static IFuContext testGenerator(IFuContext c)
        {
            var session = c.Get<IMySession>();

            var set = c.Request.QueryString["set"];
            if (!string.IsNullOrEmpty(set))
                session.MyValue = set;

            return StringResult.From(c, session.MyValue);
        }

        private static IFuContext echo(IFuContext c)
        {
            var data = c.Get<IFormData>();
            var sb = new StringBuilder();

            foreach (var item in data)
                sb.AppendFormat("{0}: {1}\n", item.Key, item.Value);

            sb.AppendFormat("\n\nFiles:\n");
            foreach (var item in data.Files)
                sb.AppendFormat("{0}: data stored in {1}\n", item.Key, item.Value.Filename);

            return StringResult.From(c, sb.ToString());
        }
    }
}
