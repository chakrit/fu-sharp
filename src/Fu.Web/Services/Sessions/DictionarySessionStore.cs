
using System;
using System.Threading;

using ISessionDict = System.Collections.Generic.IDictionary<string, Fu.Services.Sessions.ISession>;
using SessionDict = System.Collections.Generic.Dictionary<string, Fu.Services.Sessions.ISession>;

namespace Fu.Services.Sessions
{
  public class DictionarySessionStore : ISessionStore
  {
    // TODO: Eliminate locks
    ReaderWriterLockSlim _lock;
    ISessionDict _sessions;

    public DictionarySessionStore()
    {
      _lock = new ReaderWriterLockSlim();
      _sessions = new SessionDict();
    }


    public ISession CreateSession(string sessionId)
    {
      try {
        _lock.EnterWriteLock();
        if (_sessions.ContainsKey(sessionId))
          throw new InvalidOperationException(string.Format(
            @"DictionarySessionStore.CreateNew: Session Id #{0} already exists",
            sessionId));

        var session = new DictionarySession(sessionId);
        _sessions.Add(sessionId, session);

        return session;
      }
      finally { _lock.ExitWriteLock(); }
    }

    public ISession GetSession(string sessionId)
    {
      try {
        _lock.EnterReadLock();
        ISession result;
        if (_sessions.TryGetValue(sessionId, out result))
          return result;

        return null;
      }
      finally { _lock.ExitReadLock(); }
    }


    public void DeleteSession(string sessionId)
    {
      try {
        _lock.EnterWriteLock();
        if (_sessions.ContainsKey(sessionId))
          _sessions.Remove(sessionId);

      }
      finally { _lock.ExitWriteLock(); }
    }

    public void DeleteSession(ISession session)
    { DeleteSession(session.SessionId); }
  }
}
