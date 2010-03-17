
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;

using Fu;
using Fu.Steps;

namespace UrlForwarder
{
    public static class Program
    {
        // poor man's thread-safety, thread carefully
        [ThreadStatic]
        private static IDictionary<string, string> _mappings;

        [ThreadStatic]
        private static WebClient _client;


        public static void Main(string[] args)
        {
            var app = new App(null, null, new Step[] { forwardRequest });
            app.Start();
        }


        private static IFuContext forwardRequest(IFuContext c)
        {
            _client = _client ?? new WebClient();
            _mappings = _mappings ?? buildMappings(c);

            var path = c.Request.Url.AbsolutePath;

            if (_mappings.ContainsKey(path))
            {
                // real-world use will, of course, be more complicated than this
                var result = _client.DownloadString(_mappings[path]);

                c.Response.ContentType = "text/html";

                // could be replaced with Fu's mini results framework for better simplicity
                var sw = new StreamWriter(c.Response.OutputStream);
                sw.Write(result);
                sw.Flush();
                sw.Close();

                c.Response.Close();
            }
            else
                // sets the status to 404 NotFound and stop processing
                c.WalkPath.InsertNext(fu.Http.NotFound());

            return c;
        }

        static IDictionary<string, string> buildMappings(IFuContext c)
        {
            return File.ReadAllLines(c.ResolvePath("~/UrlMap.txt"))
                .Select(line => line.Trim())
                .Where(line => !line.StartsWith("#"))
                .Select(line => line.Split(' '))
                .ToDictionary(arr => arr[0], arr => arr[1]);
        }
    }
}
