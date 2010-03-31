
namespace Fu.Services
{
  public interface IService { }

  public interface IService<T> : IService
  {
    bool CanGetServiceObject(IFuContext input);
    T GetServiceObject(IFuContext input);
  }
}
