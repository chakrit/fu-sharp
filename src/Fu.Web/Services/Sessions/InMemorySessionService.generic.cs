
namespace Fu.Services.Sessions
{
  public class InMemorySessionService<T> : SessionService<T>
      where T : class
  {
    public InMemorySessionService() :
      base(new SHA1SessionIdProvider(), new DictionarySessionStore())
    { }
  }
}
