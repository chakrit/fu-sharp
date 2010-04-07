
namespace Fu.Services
{
  public interface IService { }

  public interface IService<T> : IService
  {
    // TODO: Convert this to use continuations
    //       i.e. void GetServiceObject(IFuContext input, Action<T> callback)
    bool CanGetServiceObject(IFuContext input);
    T GetServiceObject(IFuContext input);
  }
}
