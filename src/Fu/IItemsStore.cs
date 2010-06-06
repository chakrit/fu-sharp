
namespace Fu
{
  public interface IItemsStore
  {
    object Get(object scope, string key);
    T Get<T>(object scope, string key);

    void Set(object scope, string key, object value);
    void Set<T>(object scope, string key, T value);

    object this[object scope, string key] { get; set; }

    bool ContainsKey(object scope, string key);

    void Clear(object scope);
    void Clear();
  }
}
