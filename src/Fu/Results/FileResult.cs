
using System;
using System.IO;

using Fu.Contexts;

namespace Fu.Results
{
  public class FileResult : BytesResult
  {
    public string Filename { get; protected set; }

    // TODO: Null-check
    public FileResult(string filename)
    {
      if (string.IsNullOrEmpty(filename))
        throw new ArgumentNullException("filename");

      ContentType.MediaType = Mime.FromFilename(filename);
      Filename = filename;
    }


    public override byte[] RenderBytes(IFuContext c)
    {
      var filePath = c.ResolvePath(Filename, true);
      return File.ReadAllBytes(filePath);
    }


    public static ResultContext From(IFuContext input, string filename)
    { return new ResultContext(input, new FileResult(filename)); }
  }
}
