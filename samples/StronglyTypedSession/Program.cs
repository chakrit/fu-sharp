
using Fu;
using Fu.Presets;
using Fu.Services.Sessions;
using Fu.Services.Web;
using Fu.Steps;

namespace StronglyTypedSession
{
  public class Program
  {
    internal static void Main() { (new Program()).Run(); }

    public void Run()
    {
      var app = new SimpleApp(fu.Map.Controller(new MyController()));

      app.Services.Add(new InMemorySessionService<IMySession>());
      app.Services.Add(new FormDataParser());

      app.Start();
    }
  }
}
