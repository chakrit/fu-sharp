
using System;
using System.Net.Mime;
using System.Text;

using Fu.Contexts;
using System.IO;

namespace Fu.Results
{
  public class CompressedResult : IResult
  {
    private Filter<byte[]> _compressor;


    public IResult InnerResult { get; protected set; }

    public ContentType ContentType
    {
      get { return InnerResult.ContentType; }
      protected set { throw new NotSupportedException(); }
    }

    public CompressedResult(IResult input, Filter<string> compressFilter)
    {
      InnerResult = input;
      _compressor = (c, bytes) =>
      {
        // Encoding-safe way to get a string from arbitary bytes
        var ms = new MemoryStream(bytes);
        var sr = new StreamReader(ms);
        var str = sr.ReadToEnd();

        sr.Dispose();
        ms.Dispose();

        // run the compression function on the string
        var result = compressFilter(c, str);
        return Encoding.UTF8.GetBytes(result);
      };
    }

    public CompressedResult(IResult input, Filter<byte[]> compressFilter)
    {
      InnerResult = input;
      _compressor = compressFilter;
    }

    public static ResultContext From(IFuContext c, Filter<string> compressFilter)
    {
      var prev = c.As<IResultContext>();
      var result = new CompressedResult(prev.Result, compressFilter);

      return new ResultContext(c, result);
    }

    public static ResultContext From(IFuContext c, Filter<byte[]> compressFilter)
    {
      var prev = c.As<IResultContext>();
      var result = new CompressedResult(prev.Result, compressFilter);

      return new ResultContext(c, result);
    }


    public byte[] RenderBytes(IFuContext c)
    {
      return _compressor(c, InnerResult.RenderBytes(c));
    }
  }
}
