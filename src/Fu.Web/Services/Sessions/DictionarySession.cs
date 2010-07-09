
using System;
using System.Threading;

using IStringDict = System.Collections.Generic.IDictionary<string, object>;
using StringDict = System.Collections.Generic.Dictionary<string, object>;

namespace Fu.Services.Sessions
{
  public class DictionarySession : ISession
  {
    // TODO: Eliminate locks
    private ReaderWriterLockSlim _lock;
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
      _lock = new ReaderWriterLockSlim();
    }


    public object Get(string key)
    {
      try {
        _lock.EnterReadLock();
        object result;

        return _dict.TryGetValue(key, out result) ? result : null;
      }
      finally { _lock.ExitReadLock(); }
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
      try {
        _lock.EnterWriteLock();
        _dict[key] = value;
      }
      finally { _lock.ExitWriteLock(); }
    }


    public void Destroy()
    {
      try {
        _lock.EnterWriteLock();
        _dict.Clear();
      }
      finally { _lock.ExitWriteLock(); }
    }
  }
}
