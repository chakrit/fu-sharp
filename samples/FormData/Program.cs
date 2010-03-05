
using Fu;
using Fu.Presets;
using Fu.Services.Web;
using Fu.Steps;

namespace FormData
{
    class Program
    {
        static void Main(string[] args)
        {
            var app = new SimpleApp(fu.Map.Urls(
                new UrlMap("^/$", fu.Static.File("index.html")),
                new UrlMap("^/save$", c =>
                {
                    var forms = c.Get<IFormData>();
                    var name = forms["name"];

                    return fu.Static.Text(name)(c);
                })
            ));

            app.Services.Add(new FormDataParser());
            app.Start();
        }
    }
}
