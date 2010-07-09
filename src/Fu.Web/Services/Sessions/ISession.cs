
using System;

namespace Fu.Services.Sessions
{
  public interface ISession
  {
    DateTime Timestamp { get; }
    string SessionId { get; }

    object this[string key] { get; set; }

    object Get(string key);
    TValue Get<TValue>(string key);
    void Set(string key, object value);

    void Destroy();
  }
}
