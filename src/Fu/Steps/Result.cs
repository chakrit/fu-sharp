
using System;
using System.IO;
using System.IO.Compression;

using Fu.Contexts;
using Fu.Results;

namespace Fu.Steps
{
    public static partial class Result
    {
        public static Step Compress(this IResultSteps _,
            string extension, Filter<string> compressor)
        {
            return fu.If<IResultContext>(
                c => c.Request.Url.AbsolutePath.EndsWith(extension),
                _.Compress(compressor));
        }

        public static Step Compress(this IResultSteps _,
            string extension, Filter<byte[]> compressor)
        {
            return fu.If<IResultContext>(
                c => c.Request.Url.AbsolutePath.EndsWith(extension),
                _.Compress(compressor));
        }

        public static Step Compress(this IResultSteps _,
            Filter<string> compressor)
        {
            return fu.Results<IResultContext>(c =>
                new CompressedResult(c.Result, compressor));
        }

        public static Step Compress(this IResultSteps _,
            Filter<byte[]> compressor)
        {
            return fu.Results<IResultContext>(c =>
                new CompressedResult(c.Result, compressor));
        }


        // NOTE: GZip is OFF by default... it should be opt-in to avoid
        //       unknowingly slowing down many static resources serving
        public static Step Render(this IResultSteps _)
        { return _.Render(false); }

        public static Step Render(this IResultSteps _, bool allowHttpCompression)
        {
            if (!allowHttpCompression)
                return _.Render(null);

            // TODO: Should this logic be as complicated as the true "Accept-Encoding" spec?
            //       http://www.w3.org/Protocols/rfc2616/rfc2616-sec14.html?
            return _.Render((c, s) =>
            {
                var acceptEncoding = c.Request.Headers["Accept-Encoding"];
                if (string.IsNullOrEmpty(acceptEncoding)) return s;

                // Prefer deflate over gzip
                if (acceptEncoding.Contains("deflate"))
                {
                    c.Response.AddHeader("Content-Encoding", "deflate");
                    return new DeflateStream(s, CompressionMode.Compress, false);
                }
                else if (acceptEncoding.Contains("gzip"))
                {
                    c.Response.AddHeader("Content-Encoding", "gzip");
                    return new GZipStream(s, CompressionMode.Compress, false);
                }

                // no supported encoding found, assume "identity" encoding
                return s;
            });
        }

        public static Step Render(this IResultSteps _, Filter<Stream> filter)
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
