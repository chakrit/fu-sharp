
using System;

using ServiceStack.Redis;

using Fu.Services.Sessions;

namespace zNotes
{
  public class RedisSessionStore : ISessionStore
  {
    private Func<IRedisClient> _factory;

    // Redis implementation is not thread-safe
    // but it's not a good idea to re-build the client each time because of
    // socket connection so we reuse the client thread-wise using ThreadStatic
    [ThreadStatic]
    private IRedisClient _client;

    protected IRedisClient Client
    {
      get { return _client = (_client ?? _factory()); }
    }


    public RedisSessionStore(Func<IRedisClient> clientFactory)
    { _factory = clientFactory; }


    public ISession CreateSession(string sessionId)
    { return new RedisSession(Client, sessionId); }

    public ISession GetSession(string sessionId)
    { return new RedisSession(Client, sessionId); }


    public void DeleteSession(string sessionId)
    { GetSession(sessionId).Clear(); }

    public void DeleteSession(ISession session)
    { session.Clear(); }
  }
}
