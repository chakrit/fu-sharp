
using System.Net.Mime;

namespace Fu.Results
{
    public interface IResult
    {
        ContentType ContentType { get; }

        byte[] RenderBytes(IFuContext c);
    }
}
