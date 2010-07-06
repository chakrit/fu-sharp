
using Fu;
using Fu.Presets;
using Fu.Services.Sessions;
using Fu.Services.Web;
using Fu.Steps;

namespace StronglyTypedSession
{
  class Program
  {
    static void Main(string[] args)
    {
      var app = new SimpleApp(fu.Map.Controller(new MyController()));

      app.Services.Add(new InMemorySessionService<IMySession>());
      app.Services.Add(new FormDataParser());

      app.Start();
    }
  }
}
