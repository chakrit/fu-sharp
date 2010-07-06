
using System;
using System.IO;

using Fu.Contexts;

namespace Fu.Results
{
  public class FileResult : ResultBase
  {
    public string Filename { get; protected set; }


    // TODO: Null-check
    public FileResult(string filename) :
      this(filename, Mime.FromFilename(filename)) { }

    public FileResult(string filename, string contentType) :
      base()
    {
      if (string.IsNullOrEmpty(filename))
        throw new ArgumentNullException("filename");

      if (string.IsNullOrEmpty(filename))
        throw new ArgumentNullException("contentType");

      MediaType = contentType;
      Filename = filename;
    }

    public static ResultContext From(IFuContext c, string filename)
    {
      return new ResultContext(c, new FileResult(filename));
    }

    public static ResultContext From(IFuContext c, string filename, string contentType)
    {
      return new ResultContext(c, new FileResult(filename, contentType));
    }


    public override long Render(IFuContext c, Stream output)
    {
      var filePath = c.ResolvePath(Filename, allowUnsafePath: true);

      var fs = File.OpenRead(filePath);
      var length = fs.Length;

      fs.CopyTo(output);
      fs.Close();
      fs.Dispose();

      return length;
    }
  }
}
