
using System.ComponentModel;

namespace Fu.Services.Sessions
{
    public class InMemorySessionService : SessionService
    {
        public InMemorySessionService() :
            base(new SHA1SessionIdProvider(), new DictionarySessionStore())
        { }
    }
}
