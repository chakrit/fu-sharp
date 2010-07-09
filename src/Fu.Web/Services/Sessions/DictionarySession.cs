
using System;

using IStringDict = System.Collections.Generic.IDictionary<string, object>;
using StringDict = System.Collections.Concurrent.ConcurrentDictionary<string, object>;

namespace Fu.Services.Sessions
{
  public class DictionarySession : ISession
  {
    private IStringDict _dict;


    public DateTime Timestamp { get; private set; }
    public string SessionId { get; private set; }

    public object this[string key]
    { get { return Get(key); } set { Set(key, value); } }

    public DictionarySession(string sessionId)
    {
      SessionId = sessionId;
      Timestamp = DateTime.Now;

      _dict = new StringDict();
    }


    public object Get(string key)
    {
      object result;

      return _dict.TryGetValue(key, out result) ? result : null;
    }

    public TValue Get<TValue>(string key)
    {
      var result = Get(key);
      if (result != null && result is TValue)
        return (TValue)result;

      return default(TValue);
    }

    public void Set(string key, object value)
    {
      _dict[key] = value;
    }


    public void Destroy()
    {
      _dict.Clear();
    }
  }
}
