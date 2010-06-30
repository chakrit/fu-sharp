
using System;

using Sider;

namespace Fu.Services.Sessions
{
  public class RedisSessionService<T> : SessionService<T>
    where T : class
  {
    private IClientsPool _pool;


    public RedisSessionService() :
      this(new ThreadwisePool()) { }

    public RedisSessionService(IClientsPool clientsPool) :
      this(new SHA1SessionIdProvider(), clientsPool) { }

    public RedisSessionService(ISessionIdProvider idProvider) :
      this(idProvider, new ThreadwisePool()) { }

    public RedisSessionService(ISessionIdProvider idProvider, IClientsPool clientsPool) :
      base(idProvider, new RedisSessionStore(clientsPool))
    {
      if (idProvider == null) throw new ArgumentNullException("idProvider");
      if (clientsPool == null) throw new ArgumentNullException("pool");

      _pool = clientsPool;
    }
  }
}
