
using System.IO;
using System.IO.Compression;

using Fu.Contexts;
using Fu.Results;

namespace Fu.Steps
{
  public static partial class Result
  {
    public static Continuation Compress(this IResultSteps _,
      Filter<string> compressFilter)
    {
      return step => ctx => step(CompressedResult.From(ctx, compressFilter));
    }

    public static Continuation Compress(this IResultSteps _,
      Filter<byte[]> compressFilter)
    {
      return step => ctx => step(CompressedResult.From(ctx, compressFilter));
    }


    // NOTE: GZip is OFF by default... it should be opt-in to avoid
    //       unknowingly slowing down many static resources serving
    public static Continuation Render(this IResultSteps _)
    { return _.Render(false); }

    public static Continuation Render(this IResultSteps _, bool enableHttpCompression)
    {
      if (!enableHttpCompression)
        return _.Render(null);

      // TODO: Should this logic be as complicated as the true "Accept-Encoding" spec?
      //       http://www.w3.org/Protocols/rfc2616/rfc2616-sec14.html?
      return _.Render((c, s) =>
      {
        var acceptEncoding = c.Request.Headers["Accept-Encoding"];
        if (string.IsNullOrEmpty(acceptEncoding))
          return s;

        // Prefer deflate over gzip
        if (acceptEncoding.Contains("deflate")) {
          c.Response.AddHeader("Content-Encoding", "deflate");
          return new DeflateStream(s, CompressionMode.Compress, false);
        }
        else if (acceptEncoding.Contains("gzip")) {
          c.Response.AddHeader("Content-Encoding", "gzip");
          return new GZipStream(s, CompressionMode.Compress, false);
        }

        // no supported encoding found, assume "identity" encoding
        return s;
      });
    }

    public static Continuation Render(this IResultSteps _, Filter<Stream> filter)
    {
      return step => ctx =>
      {
        var resp = ctx.Response;
        var result = ctx.As<IResultContext>().Result;
        var outStream = ctx.Response.OutputStream;

        var bytes = result.RenderBytes(ctx);

        // skip rendering of empty stream
        if (bytes.LongLength == 0L) {
          outStream.Close();
          return;
        }

        // write to a temporary stream so we can have Content-Length
        // even when we need to run it through a filter
        if (filter != null) {
          var ms = new MemoryStream();
          var temp = filter(ctx, ms);
          var bw = new BinaryWriter(temp);

          bw.Write(bytes);
          bw.Close();

          bytes = ms.ToArray();
          temp.Dispose();
          ms.Dispose();
          bw.Dispose();
        }

        // write out the response
        if (!string.IsNullOrEmpty(result.ContentType.MediaType))
          resp.ContentType = result.ContentType.MediaType;

        resp.ContentLength64 = bytes.LongLength;

        using (var bw = new BinaryWriter(outStream)) {
          bw.Write(bytes);
          bw.Close();
        }

        outStream.Close();

        step(ctx);
      };
    }
  }
}
