
namespace Fu.Services
{
    public class LoggingService : IService
    {
        public void BeginWalk(IFuContext input)
        { FuTrace.Request(input); }

        public void EndWalk(IFuContext input)
        { FuTrace.Response(input); }
    }
}
