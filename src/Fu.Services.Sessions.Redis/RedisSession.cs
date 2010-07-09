
using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

using Sider;

namespace Fu.Services.Sessions
{
  public class RedisSession : ISession
  {
    public const string RedisKeyFormat = "fu:s:{0}";

    private BinaryFormatter _formatter;
    private IRedisClient _client;


    public DateTime Timestamp { get; protected set; }
    public string SessionId { get; protected set; }

    public RedisSession(IRedisClient client, string sessionId)
    {
      Timestamp = DateTime.Now;
      SessionId = sessionId;

      _formatter = new BinaryFormatter();
      _client = client;
    }


    public object this[string key]
    {
      get { return Get(key); }
      set { Set(key, value); }
    }


    public object Get(string key)
    {
      var sessionKey = getSessionKey();

      using (var ms = new MemoryStream()) {
        var length = _client.HGetTo(sessionKey, key, ms);

        if (length < 0 || ms.Length < 0)
          return null;

        ms.Seek(0, SeekOrigin.Begin);
        return _formatter.Deserialize(ms);
      }
    }

    public TValue Get<TValue>(string key)
    {
      var result = Get(key);

      try { return (result == null) ? default(TValue) : (TValue)result; }
      catch (InvalidCastException) { return default(TValue); }
    }


    public void Set(string key, object value)
    {
      // update the set to indicate we have new value
      var sessionKey = getSessionKey();

      if (value != null) {
        using (var ms = new MemoryStream()) {
          _formatter.Serialize(ms, value);

          ms.Seek(0, SeekOrigin.Begin);
          _client.HSetFrom(sessionKey, key, ms, (int)ms.Length);
        }
      }
      else {
        _client.HDel(sessionKey, key);
      }
    }


    public void Destroy()
    {
      var sessionKey = getSessionKey();
      _client.Del(sessionKey);
    }


    private string getSessionKey()
    {
      return string.Format(RedisKeyFormat, SessionId);
    }
  }
}
