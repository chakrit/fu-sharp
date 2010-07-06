
using System.IO;
using System.Text;
using System.Net.Mime;

using Fu.Contexts;

namespace Fu.Results
{
  public class FilteredResult : ResultBase
  {
    private Filter<Stream> _compressor;


    public IResult ActualResult { get; protected set; }

    public override long ContentLength64 { get { return ActualResult.ContentLength64; } }
    public override ContentType ContentType { get { return ActualResult.ContentType; } }


    public FilteredResult(IResult result, Filter<string> filter) :
      this(result, convertToStreamFilter(filter)) { }

    public FilteredResult(IResult result, Filter<byte[]> filter) :
      this(result, convertToStreamFilter(filter)) { }

    public FilteredResult(IResult result, Filter<Stream> filter)
    {
      ActualResult = result;
      _compressor = filter;
    }


    public static ResultContext From(IFuContext c, Filter<string> filter)
    {
      return new ResultContext(c,
        new FilteredResult(c.As<IResultContext>().Result, filter));
    }

    public static ResultContext From(IFuContext c, Filter<byte[]> filter)
    {
      return new ResultContext(c,
        new FilteredResult(c.As<IResultContext>().Result, filter));
    }

    public static ResultContext From(IFuContext c, Filter<Stream> filter)
    {
      return new ResultContext(c,
        new FilteredResult(c.As<IResultContext>().Result, filter));
    }


    public override long Render(IFuContext c, Stream output)
    {
      // TODO: Eliminate this buffer?
      // Renders actual result to a buffer first
      Stream ms = new MemoryStream();
      ActualResult.Render(c, ms);

      // then runs it through the compressor
      ms.Seek(0, SeekOrigin.Begin);
      var buffer = _compressor(c, ms);

      // then render the result to the output
      buffer.CopyTo(output);

      // checks wether we can have the content length
      var length = -1L;
      if (buffer.CanSeek)
        length = buffer.Length;

      buffer.Close();
      buffer.Dispose();

      if (ms != buffer) {
        ms.Close();
        ms.Dispose();
      }

      return length;
    }


    private static Filter<Stream> convertToStreamFilter(Filter<string> strFilter)
    {
      return (c, stream) =>
      {
        // TODO: Eliminate this buffer? StreamReader is required to correctly
        //       decodes the stream without encoding issues
        var sr = new StreamReader(stream);
        var str = sr.ReadToEnd();

        // runs the string filter
        str = strFilter(c, str);

        // convert back to stream
        var bytes = Encoding.UTF8.GetBytes(str);
        return new MemoryStream(bytes);
      };
    }

    private static Filter<Stream> convertToStreamFilter(Filter<byte[]> bytesFilter)
    {
      return (c, stream) =>
      {
        // TODO: Eliminate this buffer?
        // Renders to the buffer first
        var ms = new MemoryStream();
        stream.CopyTo(ms);

        // Converts to byte[] and runs the filter
        var buffer = ms.ToArray();
        buffer = bytesFilter(c, buffer);

        // put the resulting array back to the stream
        ms.Seek(0, SeekOrigin.Begin);
        ms.Write(buffer, 0, buffer.Length);
        ms.SetLength(buffer.Length);

        return ms;
      };
    }
  }
}
