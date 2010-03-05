
using System.IO;
using System.IO.Compression;

using Fu.Contexts;

namespace Fu.Steps
{
    public static partial class Render
    {
        // NOTE: GZip is OFF by default... it should be opt-in to avoid
        //       unknowingly slowing down many static resources serving
        public static Step Result(this IRenderSteps _)
        { return _.Result(false); }

        public static Step Result(this IRenderSteps _, bool autoGZip)
        {
            if (!autoGZip)
                return _.Result(null);

            return _.Result((c, s) =>
            {
                if (!c.Request.Headers["Accept-Encoding"].Contains("gzip"))
                    return s;

                c.Response.AddHeader("Content-Encoding", "gzip");
                return new GZipStream(s, CompressionMode.Compress, false);
            });
        }

        public static Step Result(this IRenderSteps _, FilterStep<Stream> filter)
        {
            // TODO: Concise-sify this
            if (filter == null)
                return fu.Void<IResultContext>(c =>
                {
                    var resp = c.Response;
                    var result = c.Result;
                    var bytes = result.RenderBytes(c);

                    resp.ContentType = result.ContentType.ToString();
                    resp.ContentLength64 = bytes.LongLength;

                    var bw = new BinaryWriter(resp.OutputStream);
                    bw.Write(bytes);
                    bw.Close();

                    resp.OutputStream.Close();
                });
            else
                return fu.Void<IResultContext>(c =>
                {
                    var resp = c.Response;
                    var result = c.Result;
                    var bytes = result.RenderBytes(c);

                    resp.ContentType = result.ContentType.ToString();
                    // omits ContentLength64 because a filter could modify the length

                    var outStream = filter(c, resp.OutputStream);
                    var bw = new BinaryWriter(outStream);
                    bw.Write(bytes);
                    bw.Close();

                    if (outStream != resp.OutputStream)
                        outStream.Close();

                    resp.OutputStream.Close();
                });
        }
    }
}
