
using System.Net;

namespace Fu
{
  public delegate void FuAction(IFuContext c);

  public delegate FuAction Continuation(FuAction action);
}
