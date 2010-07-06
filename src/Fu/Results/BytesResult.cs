
using System.IO;
using System;
using System.Net.Mime;

namespace Fu.Results
{
  public class BytesResult : ResultBase
  {
    private byte[] _data;
    private int _offset;


    public BytesResult(byte[] data, int offset, int count) :
      base()
    {
      if (data == null)
        throw new ArgumentNullException("data");

      _data = data;
      _offset = offset;
      ContentLength64 = count;
    }


    public override long Render(IFuContext c, Stream output)
    {
      var count = (int)ContentLength64;
      output.Write(_data, _offset, count);

      return count;
    }
  }
}
