
using System;

namespace Fu.Services.Sessions
{
    // basic version
    public class SessionService : IService<ISession>
    {
        private ISessionIdProvider _idMgr;
        private ISessionStore _store;


        public SessionService(ISessionIdProvider idProvider, ISessionStore store)
        {
            _idMgr = idProvider;
            _store = store;
        }


        public bool CanGetServiceObject(IFuContext input)
        { return true; }

        public ISession GetServiceObject(IFuContext input)
        { return getSession(input); }


        // gets existing session or create new ones
        private ISession getSession(IFuContext c)
        {
            var id = _idMgr.GetId(c);

            if (string.IsNullOrEmpty(id))
                return createNewSession(c);

            // try to get the session, if failed it means
            // we have an invalid session id or it has expired
            var session = _store.GetSession(id);
            if (session == null)
            {
                FuTrace.Session("INVALID", id);
                _idMgr.DeleteId(c);

                return createNewSession(c);
            }

            // enforce session expiration
            if ((DateTime.Now - session.Timestamp) > c.Settings.SessionTimeout)
            {
                FuTrace.Session("EXPIRED", id);
                _store.DeleteSession(session);
                _idMgr.DeleteId(c);

                return createNewSession(c);
            }

            return session;
        }

        private ISession createNewSession(IFuContext c)
        {
            _idMgr.DeleteId(c);
            var id = _idMgr.CreateId(c);

            FuTrace.Session("NEW", id);
            return _store.CreateSession(id);
        }
    }
}
