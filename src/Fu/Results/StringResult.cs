
using System.Net.Mime;
using System.Text;

using Fu.Contexts;

namespace Fu.Results
{
    public class StringResult : BytesResult
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


        public override byte[] RenderBytes(IFuContext c)
        {
            var charSet = Encoding.GetEncoding(this.ContentType.CharSet);
            var bytes = Encoding.UTF8.GetBytes(Text);

            return Encoding.Convert(Encoding.UTF8, charSet, bytes);
        }


        public static ResultContext From(IFuContext input, string text)
        { return new ResultContext(input, new StringResult(text)); }
    }
}
