
using Fu;
using Fu.Presets;
using Fu.Services.Models;
using Fu.Services.Web;
using Fu.Steps;

namespace ModelBinder
{
  class Program
  {
    static void Main(string[] args)
    {
      var app = new SimpleApp(fu.Map.Urls(new ContMap() {
        { "^/$", fu.Static.File("index.html") },
        { "^/greet$", fu.Map.Post().Then(showGreet()) },
      }));

      app.Services.Add(new ModelBinder<MyModel>());
      app.Services.Add(new FormDataParser());
      app.Start();
    }

    private static Continuation showGreet()
    {
      return step => ctx =>
      {
        var model = ctx.Get<MyModel>();
        var greets = string.Format(
          "Hello, {0}! You're {1} years old!",
          model.Name, model.Age);

        fu.Static.Text(greets)(step)(ctx);
      };
    }
  }
}
