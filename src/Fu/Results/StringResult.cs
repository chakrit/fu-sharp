
using System.IO;
using System.Net.Mime;
using System.Text;

using Fu.Contexts;

namespace Fu.Results
{
  public class StringResult : ResultBase
  {
    public string Text { get; protected set; }


    // TODO: Should we hard-code UTF8 here?
    //       Should we provide a configurable option for that?
    public StringResult(string text)
    {
      ContentType.MediaType = Mime.TextPlain;
      ContentType.CharSet = Encoding.UTF8.WebName;

      Text = text;
    }


    public override long Render(IFuContext c, Stream output)
    {
      var charSet = Encoding.GetEncoding(this.ContentType.CharSet);
      var bytes = Encoding.UTF8.GetBytes(Text);

      var buffer = Encoding.Convert(Encoding.UTF8, charSet, bytes);
      output.Write(buffer, 0, buffer.Length);

      return buffer.Length;
    }


    public static ResultContext From(IFuContext input, string text)
    { return new ResultContext(input, new StringResult(text)); }
  }
}
