
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
      var pipeline = fu.If(isFolder, listFiles(),
        fu.Static.Folder("/", "Content"));

      var app = new SimpleApp(pipeline);
      app.Start();
    }


    private static bool isFolder(IFuContext c)
    {
      return c.Request.Url.AbsolutePath.EndsWith("/");
    }


    private static Continuation listFiles()
    {
      var on404 = fu.Http.NotFound();

      return step => ctx =>
      {
        var relPath = ctx.Request.Url.AbsolutePath.Substring(1);
        var folder = ctx.ResolvePath(Path.Combine("Content", relPath));

        if (!Directory.Exists(folder)) {
          on404(step)(ctx);
          return;
        }

        var files = Directory
          .GetFiles(folder, "*.*", SearchOption.AllDirectories)
          .Select(f => f.Substring(folder.Length))
          .Select(f => string.Format("<li><a href=\"{0}\">{0}</a></li>", f))
          .ToArray();

        var template = @"
        <html>
          <head><title>Files on this server</title></head>
          <body><h1>Files - /{0}</h1><ul>{1}</ul></body>
        </html>";

        var html = string.Format(template,
          relPath,
          string.Join("", files));

        var result = StringResult.From(ctx, html);
        result.Result.ContentType.MediaType = Mime.TextHtml;

        step(result);
      };
    }
  }
}
