
using Fu.Services;
using Fu.Services.Sessions;
using Fu.Services.Web;

namespace Fu.Presets
{
    public class SimpleWebApp : SimpleApp
    {
        private IService _sessionService;


        public SimpleWebApp(params Step[] steps) : this(null, steps) { }

        public SimpleWebApp(FuSettings settings, params Step[] steps) :
            base(settings, steps)
        {
            this.Services.Add(_sessionService = new InMemorySessionService());
            this.Services.Add(new FormDataParser());
            this.Services.Add(new MultipartFormDataParser());
        }


        public void SetSessionType<T>() where T : class
        {
            this.Services.Remove(_sessionService);
            this.Services.Add(new InMemorySessionService<T>());
        }
    }
}
