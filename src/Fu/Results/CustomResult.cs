
using System.IO;

namespace Fu.Results
{
  public delegate long RenderDelegate(IFuContext c, Stream output);

  public class CustomResult : ResultBase
  {
    public RenderDelegate _render;

    public CustomResult(string contentType, RenderDelegate renderAction)
    {
      // TODO: Null-check
      _render = renderAction;
      MediaType = contentType;
    }


    public override long Render(IFuContext c, Stream output)
    {
      return _render(c, output);
    }
  }
}
