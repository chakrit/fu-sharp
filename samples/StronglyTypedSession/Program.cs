
using Fu;
using Fu.Steps;
using Fu.Services.Sessions;
using Fu.Presets;
using Fu.Services.Web;

namespace StronglyTypedSession.cs
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
