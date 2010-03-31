
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
      var app = new SimpleApp(fu.Map.Urls(
        new UrlMap("^/$", fu.Static.File("index.html")),
        new UrlMap("^/greet$", fu.Http.Post(c =>
        {
          var model = c.Get<MyModel>();
          var greets = string.Format(
            "Hello, {0}! You're {1} years old!",
            model.Name, model.Age);

          c.WalkPath.InsertNext(fu.Static.Text(greets));
          return c;
        }))));

      app.Services.Add(new ModelBinder<MyModel>());
      app.Services.Add(new FormDataParser());
      app.Start();
    }
  }
}
