
using System;
using System.Net.Mime;
using System.Text;

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

    public CompressedResult(IResult input, Filter<string> compressFunc)
    {
      InnerResult = input;
      _compressor = (c, bytes) =>
      {
        // TODO: Is UTF8 the right one? Do we need to make this configurable?
        var str = Encoding.UTF8.GetString(bytes);

        // HACK: Fixes many unicode/ansi wonderbugs
        str = str.Trim();

        var result = compressFunc(c, str);
        return Encoding.UTF8.GetBytes(result);
      };
    }

    public CompressedResult(IResult input, Filter<byte[]> compressFunc)
    {
      InnerResult = input;
      _compressor = compressFunc;
    }


    public byte[] RenderBytes(IFuContext c)
    {
      return _compressor(c, InnerResult.RenderBytes(c));
    }
  }
}
