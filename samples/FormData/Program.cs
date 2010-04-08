
using Fu;
using Fu.Presets;
using Fu.Services.Web;
using Fu.Steps;
using System.Collections.Generic;

namespace FormData
{
  class Program
  {
    static void Main(string[] args)
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
