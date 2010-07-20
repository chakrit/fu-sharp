
using System.Collections.Generic;

using Fu;
using Fu.Presets;
using Fu.Services.Web;
using Fu.Steps;

namespace FormData
{
  public class Program
  {
    public static void Main(string[] args)
    {
      var app = new SimpleApp(fu.Map.Urls(new Dictionary<string, Continuation>
      {
        { "^/$", fu.Static.File("index.html") },
        { "^/save$", echoName() }
      }));

      app.Services.Add(new FormDataParser());
      app.Start();
    }

    private static Continuation echoName()
    {
      return step => ctx =>
      {
        var forms = ctx.Get<IFormData>();
        var name = forms["name"];

        fu.Static.Text("Hello, " + name + "!")(step)(ctx);
      };
    }
  }
}
