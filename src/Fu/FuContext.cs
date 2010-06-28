
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;

using Fu.Exceptions;
using Fu.Services;

namespace Fu
{
  public class FuContext : IFuContext, IDisposable
  {
    private IFuContext _previous;


    // TODO: Abstract out HttpListenerRequest
    public HttpListenerRequest Request { get; private set; }
    public HttpListenerResponse Response { get; private set; }

    public IServiceBroker Services { get; private set; }
    public IItemsStore Items { get; private set; }
    public FuSettings Settings { get; private set; }


    public FuContext(IFuContext c) :
      this(c, c.Settings, c.Services, c.Request, c.Response) { }

    public FuContext(FuSettings settings,
      IServiceBroker services,
      HttpListenerContext httpContext) :
      this(null, settings, services, httpContext.Request, httpContext.Response) { }

    public FuContext(FuSettings settings,
      IServiceBroker services,
      HttpListenerRequest request,
      HttpListenerResponse response) :
      this(null, settings, services, request, response) { }

    protected FuContext(IFuContext previous,
      FuSettings settings,
      IServiceBroker broker,
      HttpListenerRequest request,
      HttpListenerResponse response)
    {
      this.Request = request;
      this.Response = response;

      this.Services = broker;
      this.Settings = settings;

      // TODO: Make this injectable and clean up the ctor parameters list as well.
      this.Items = new ContextItemsStore();

      _previous = previous;
    }


    public T As<T>() where T : class, IFuContext
    {
      // TODO: Is looking up the chain a good idea?
      //       since this will transparently transforms the context
      //       without the user acknowledgement
      var result = this as T;
      if (result == null) {
        if (_previous == null)
          throw new MismatchedContextTypeException(
            this.GetType(), typeof(T));

        result = _previous.As<T>();
        if (result == null)
          throw new MismatchedContextTypeException(
            this.GetType(), typeof(T));
      }

      return result;
    }


    public T Get<T>()
    {
      return Services.Get<T>(this);
    }

    public bool CanGet<T>()
    {
      return Services.CanGet<T>(this);
    }


    public void Dispose()
    {
      Items.Clear();
      Response.Close();
    }
  }
}
