
using System;

using ISessionDict = System.Collections.Generic.IDictionary<string, Fu.Services.Sessions.ISession>;
using SessionDict = System.Collections.Concurrent.ConcurrentDictionary<string, Fu.Services.Sessions.ISession>;

namespace Fu.Services.Sessions
{
  public class DictionarySessionStore : ISessionStore
  {
    private ISessionDict _sessions;

    public DictionarySessionStore()
    {
      _sessions = new SessionDict();
    }


    public ISession CreateSession(string sessionId)
    {
      try {
        // reserve a session id
        // this line will throw an exception if the id already exists
        _sessions.Add(sessionId, null);
        return _sessions[sessionId] = new DictionarySession(sessionId);
      }
      catch (ArgumentException e) {
        throw new InvalidOperationException(string.Format(
          @"DictionarySessionStore.CreateNew: Session Id #{0} already exists",
          sessionId), e);
      }
    }

    public ISession GetSession(string sessionId)
    {
      ISession result;
      if (_sessions.TryGetValue(sessionId, out result))
        return result;

      return null;
    }


    public void DeleteSession(string sessionId)
    {
      if (_sessions.ContainsKey(sessionId))
        _sessions.Remove(sessionId);
    }

    public void DeleteSession(ISession session)
    { DeleteSession(session.SessionId); }
  }
}
