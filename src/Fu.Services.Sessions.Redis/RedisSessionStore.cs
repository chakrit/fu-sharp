
using System;

using Sider;

namespace Fu.Services.Sessions
{
  public class RedisSessionStore : ISessionStore
  {
    private IClientsPool _pool;

    protected IRedisClient Client
    {
      get { return _pool.GetClient(); }
    }


    public RedisSessionStore(IClientsPool clientsPool)
    { _pool = clientsPool; }


    public ISession CreateSession(string sessionId)
    { return new RedisSession(Client, sessionId); }

    public ISession GetSession(string sessionId)
    { return new RedisSession(Client, sessionId); }


    public void DeleteSession(string sessionId)
    { GetSession(sessionId).Destroy(); }

    public void DeleteSession(ISession session)
    { session.Destroy(); }
  }
}
