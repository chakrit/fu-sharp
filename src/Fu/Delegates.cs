
using System.Net;

namespace Fu
{
  // required to make Server and IServer usable outside of Fu
  public delegate void RequestHandler(HttpListenerContext c);

  // continuation stuffs for all the libraries in Fu
  public delegate void FuAction(IFuContext c);

  public delegate FuAction Continuation(FuAction action);

  public delegate void FuAction<T>(T c)
    where T : IFuContext;

  public delegate FuAction Continuation<T>(FuAction<T> action)
    where T : IFuContext;

  // meta-func stuffs
  public delegate T Reduce<T>(IFuContext c);
  public delegate T Filter<T>(IFuContext c, T input);
}
