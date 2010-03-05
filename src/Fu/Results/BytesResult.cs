
using System;
using System.Net.Mime;

namespace Fu.Results
{
    public class BytesResult : IResult
    {
        private byte[] _data;


        public virtual ContentType ContentType { get; protected set; }
        public virtual string MediaType
        {
            get { return ContentType.MediaType; }
            set { ContentType.MediaType = value; }
        }

        protected BytesResult()
        {
            ContentType = ContentType = new ContentType(Mime.AppOctetStream);
        }

        public BytesResult(byte[] data) :
            this()
        {
            if (data == null)
                throw new ArgumentNullException("data");

            _data = data;
        }


        public virtual byte[] RenderBytes(IFuContext c)
        { return _data; }
    }
}
