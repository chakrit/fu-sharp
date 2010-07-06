
using System.IO;
using System.Net.Mime;

namespace Fu.Results
{
  public abstract class ResultBase : IResult
  {
    public virtual ContentType ContentType { get; protected set; }
    public virtual long ContentLength64 { get; protected set; }

    public string MediaType
    {
      get { return ContentType.MediaType; }
      set { ContentType.MediaType = value; }
    }

    protected ResultBase()
    {
      ContentType = new ContentType(Mime.AppOctetStream);
      ContentLength64 = -1;
    }


    public abstract long Render(IFuContext c, Stream output);
  }
}
