
using System;
using System.Web;
using System.Web.Hosting;
using System.IO;

namespace Fu
{
    // TODO:
    public class AspNetHost : MarshalByRefObject
    {
        private IApp _app;


        public static AspNetHost Create(IApp app)
        {
            var host = (AspNetHost)ApplicationHost.CreateApplicationHost(
                typeof(AspNetHost), "/", app.Settings.BasePath);

            host.Initialize(app);
            return host;
        }

        private void Initialize(IApp app)
        {

        }


        public void ProcessRequest(string page, string query, string output)
        {
            var writer = (TextWriter)File.CreateText(output);
            //var request = new SimpleWorkerRequest(page, query, output);
            //HttpRuntime.ProcessRequest(request);
        }
    }
}
