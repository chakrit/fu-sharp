
using System.IO;

using Fu.Results;

namespace Fu.Steps
{
  public static partial class Static
  {
    public static Continuation Text(this IStaticSteps _, string text)
    {
      return _.Text(c => text);
    }

    public static Continuation Text(this IStaticSteps _, Reduce<string> textReducer)
    {
      return step => ctx => step(StringResult.From(ctx, textReducer(ctx)));
    }

    // TODO: Convert this to *not* use overloads to allow better
    //       composition
    // TODO: Add automatic caching, pre-execute the result
    //       or maybe wraps it in a memorizing step
    public static Continuation File(this IStaticSteps _, string filename)
    {
      var mime = Mime.FromFilename(filename);

      return _.File(c => filename, (c, s) => mime);
    }

    public static Continuation File(this IStaticSteps _,
      Reduce<string> filenameReducer)
    {
      return _.File(filenameReducer, (c, filename) => Mime.FromFilename(filename));
    }

    public static Continuation File(this IStaticSteps _,
      Reduce<string> filenameReducer, string contentType)
    {
      return _.File(filenameReducer, (c, filename) => Mime.FromFilename(filename));
    }

    public static Continuation File(this IStaticSteps _,
      Reduce<string> filenameReducer, Filter<string> contentTypeFilter)
    {
      return step => ctx =>
      {
        var filename = filenameReducer(ctx);
        var contentType = contentTypeFilter(ctx, filename);

        step(FileResult.From(ctx, filename, contentType));
      };
    }


    public static Continuation Folder(this IStaticSteps _,
      string urlPrefix, string folder)
    {
      return _.Folder(urlPrefix, folder, null);
    }

    public static Continuation Folder(this IStaticSteps _,
      string urlPrefix, string folder, Continuation on404)
    {
      on404 = on404 ?? fu.Http.NotFound();

      string folderPath = null;

      return step => ctx =>
      {
        var rawUrl = ctx.Request.Url.AbsolutePath;
        folderPath = folderPath ?? ctx.ResolvePath(folder);

        if (!rawUrl.StartsWith(urlPrefix)) {
          on404(step)(ctx);
          return;
        }

        var url = rawUrl.Substring(urlPrefix.Length);
        var filePath = Path.Combine(folderPath, url);

        if (!System.IO.File.Exists(filePath)) {
          on404(step)(ctx);
          return;
        }

        step(FileResult.From(ctx, filePath));
      };
    }
  }
}
