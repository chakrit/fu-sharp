
using System;
using System.Net.Mime;
using System.Text;

namespace Fu.Results
{
    public class CompressedResult : IResult
    {
        private Func<byte[], byte[]> _compressor;


        public IResult InnerResult { get; protected set; }

        public ContentType ContentType
        {
            get { return InnerResult.ContentType; }
            protected set { throw new NotSupportedException(); }
        }

        public CompressedResult(IResult input, Func<string, string> compressFunc)
        {
            InnerResult = input;
            _compressor = bytes =>
            {
                // TODO: Is UTF8 the right one? Do we need to make this configurable?
                var str = Encoding.UTF8.GetString(bytes);

                // HACK: Fixes many unicode/ansi wonderbugs
                str = str.Trim();

                var result = compressFunc(str);

                return Encoding.UTF8.GetBytes(result);
            };
        }

        public CompressedResult(IResult input, Func<byte[], byte[]> compressFunc)
        {
            InnerResult = input;
            _compressor = compressFunc;
        }


        public byte[] RenderBytes(IFuContext c)
        {
            return _compressor(InnerResult.RenderBytes(c));
        }
    }
}
