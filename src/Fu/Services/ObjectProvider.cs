
namespace Fu.Services
{
    // simple service to provide a static singleton object
    // such as settings throughout the app
    public class ObjectProvider<T> : IService<T>
    {
        private T _obj;

        public ObjectProvider(T obj) { _obj = obj; }


        public bool CanGetServiceObject(IFuContext input) { return _obj != null; }
        public T GetServiceObject(IFuContext input) { return _obj; }

        public void BeginWalk(IFuContext input) { /* no-op */ }
        public void EndWalk(IFuContext input) { /* no-op */ }
    }
}
