
using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

using Fu.Services.Sessions;

using ServiceStack.Redis;

namespace zNotes
{
  public class RedisSession : ISession
  {
    public const string RedisKeyFormat = "{0}-{1}";

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
      key = getItemKey(key);

      var bytes = _client.Get<byte[]>(key);
      if (bytes == null || bytes.Length == 0)
        return null;

      var ms = new MemoryStream(bytes);
      var result = _formatter.Deserialize(ms);

      ms.Dispose();
      return result;
    }

    public TValue Get<TValue>(string key)
    {
      var result = Get(key);

      return (result == null) ? default(TValue) : (TValue)result;
    }


    public void Set(string key, object value)
    {
      // update the set to indicate we have new value
      key = getItemKey(key);

      if (value != null) {
        var ms = new MemoryStream();
        _formatter.Serialize(ms, value);

        _client.Set(key, ms.ToArray());
        ms.Dispose();

      }
      else {
        _client.Remove(key);

      }
    }


    public void Clear()
    {
      // TODO: Remove dependence on TPL code
      throw new NotImplementedException(
        "I don't think this feature is needed... yet.");
    }


    private string getItemKey(string keyName)
    {
      return string.Format(RedisKeyFormat, SessionId, keyName);
    }
  }
}
