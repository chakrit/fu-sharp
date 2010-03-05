
using System.IO;
using System.Linq;

using Fu;
using Fu.Presets;
using Fu.Results;
using Fu.Steps;

namespace FileServer
{
    class Program
    {
        static void Main(string[] args)
        {
            var step = fu.Map.DefaultDoc(listFiles, fu.Static.Folder("/", "Content"));

            var app = new SimpleApp(step);
            app.Start();
        }

        static IFuContext listFiles(IFuContext c)
        {
            var folder = c.ResolvePath("Content");
            var files = Directory
                .GetFiles(folder, "*.*", SearchOption.AllDirectories)
                .Select(f => f.Substring(folder.Length))
                .Select(f => string.Format("<li><a href=\"{0}\">{0}</a></li>", f))
                .ToArray();

            var template = @"
                <html>
                    <head><title>Files on this server</title></head>
                    <body><h1>Files</h1><ul>{0}</ul></body>
                </html>";

            var html = string.Format(template, string.Join("", files));

            var result = StringResult.From(c, html);
            result.Result.ContentType.MediaType = Mime.TextHtml;

            return result;
        }
    }
}
