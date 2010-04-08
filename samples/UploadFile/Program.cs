
using System;
using System.Drawing;
using System.Drawing.Imaging;

using Fu;
using Fu.Services.Web;
using Fu.Steps;

namespace UploadFile
{
  class Program
  {
    private static void Main(string[] args)
    {
      var services = new[] { new MultipartFormDataParser() };
      var steps = new[] {
        fu.Map.Urls(new ContMap {
          { "^/$", fu.Compose(fu.Static.File("index.html"), fu.Result.Render()) },
          { "^/sepia$", makeSepia() },
        })
      };

      var app = new App(null, services, steps);
      app.Start();
    }

    private static Continuation makeSepia()
    {
      return step => ctx =>
      {
        // gets the uploaded image
        var file = ctx.Get<IFormData>().Files["image"];

        var fs = file.OpenRead();
        var img = Image.FromStream(fs);
        fs.Close();
        fs.Dispose();

        // convert to sepia
        var bmp = new Bitmap(img);
        for (var i = 0; i < bmp.Width; i++)
          for (var j = 0; j < bmp.Height; j++) {
            var pixel = bmp.GetPixel(i, j);
            pixel = Color.FromArgb(
              (int)Math.Min(255, pixel.R * .393 + pixel.G * .769 + pixel.B * .189),
              (int)Math.Min(255, pixel.R * .349 + pixel.G * .686 + pixel.B * .168),
              (int)Math.Min(255, pixel.R * .272 + pixel.G * .534 + pixel.B * .131)
            );

            bmp.SetPixel(i, j, pixel);
          }

        // write result to response stream
        var output = ctx.Response.OutputStream;
        bmp.Save(output, ImageFormat.Png);
        bmp.Dispose();

        ctx.Response.ContentType = Mime.ImagePng;
        ctx.Response.Close();

        step(ctx);
      };
    }
  }
}
