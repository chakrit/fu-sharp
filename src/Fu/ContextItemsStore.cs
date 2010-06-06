
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace Fu
{
  public class ContextItemsStore : IItemsStore
  {
    private IDictionary<Tuple<object, string>, object> _dict;


    public object this[object scope, string key]
    {
      get { return Get(scope, key); }
      set { Set(scope, key, value); }
    }

    public ContextItemsStore()
    {
      _dict = new ConcurrentDictionary<Tuple<object, string>, object>();
    }


    public object Get(object scope, string key)
    { return _dict[Tuple.Create(scope, key)]; }

    public T Get<T>(object scope, string key)
    { return (T)Get(scope, key); }


    public void Set(object scope, string key, object value)
    { _dict[Tuple.Create(scope, key)] = value; }

    public void Set<T>(object scope, string key, T value)
    { Set(scope, key, value); }


    public bool ContainsKey(object scope, string key)
    {
      return _dict.ContainsKey(Tuple.Create(scope, key));
    }


    public void Clear(object scope)
    {
      var itemsInScope = _dict.Keys
        .Where(key => key.Item1 == scope)
        .ToArray();

      Array.ForEach(itemsInScope, s => _dict.Remove(s));
    }
  }
}
