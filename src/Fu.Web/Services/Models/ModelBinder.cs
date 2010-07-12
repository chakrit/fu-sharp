
using System;
using System.ComponentModel;
using System.Linq;
using System.Reflection;

using Fu.Services.Web;

namespace Fu.Services.Models
{
  public partial class ModelBinder<T> : IService<T>
      where T : class
  {
    public Filter<T> Binder { get; protected set; }
    public Func<T> Factory { get; protected set; }


    public ModelBinder() : this(null, null) { }
    public ModelBinder(Func<T> factory) : this(factory, null) { }
    public ModelBinder(Filter<T> binder) : this(null, binder) { }

    public ModelBinder(Func<T> factory, Filter<T> binder)
    {
      Factory = factory ?? createDefaultFactory();
      Binder = binder ?? createDefaultBinder();
    }


    public T GetModel(IFuContext input)
    {
      return Binder(input, Factory());
    }


    bool IService<T>.CanGetServiceObject(IFuContext input)
    {
      return input.Request.HttpMethod == "POST" &&
        input.CanGet<IFormData>();
    }

    T IService<T>.GetServiceObject(IFuContext input) { return GetModel(input); }
  }
}
