
using System;
using System.IO;

using Fu.Contexts;

namespace Fu.Results
{
  public class FileResult : BytesResult
  {
    public string Filename { get; protected set; }

    // TODO: Null-check
    public FileResult(string filename) :
      this(filename, Mime.FromFilename(filename)) { }

    public FileResult(string filename, string contentType)
    {
      if (string.IsNullOrEmpty(filename))
        throw new ArgumentNullException("filename");

      if (string.IsNullOrEmpty(filename))
        throw new ArgumentNullException("contentType");

      ContentType.MediaType = contentType;
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


    public override byte[] RenderBytes(IFuContext c)
    {
      var filePath = c.ResolvePath(Filename, true);
      return File.ReadAllBytes(filePath);
    }


  }
}
