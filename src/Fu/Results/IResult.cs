
using System.IO;
using System.Net.Mime;

namespace Fu.Results
{
  public interface IResult
  {
    ContentType ContentType { get; }
    long ContentLength64 { get; }

    long Render(IFuContext c, Stream output);
  }
}
