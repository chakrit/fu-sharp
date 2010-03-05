
namespace Fu.Services.Sessions
{
    public interface ISessionStore
    {
        ISession CreateSession(string sessionId);
        ISession GetSession(string sessionId);

        void DeleteSession(string sessionId);
        void DeleteSession(ISession session);
    }
}
