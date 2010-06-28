
using Fu;
using Fu.Services.Web;
using Fu.Steps;

namespace StronglyTypedSession.cs
{
  public class MyController : RestStyleController
  {
    public override void Initialize()
    {
      Get("/",
        fu.Static.File("index.html"),
        fu.Result.Compress(replaceSessionVariables));

      Post("/",
        fu.Action(c =>
        {
          var form = c.Get<IFormData>();
          var session = c.Get<IMySession>();

          session.Username = form["Username"];
          session.UserID = int.Parse(form["UserID"]);
        }),
        fu.Redirect.To("/"));
    }


    private static string replaceSessionVariables(IFuContext context, string source)
    {
      var session = context.Get<IMySession>();

      source = source.Replace("{%USER_ID%}", session.UserID.ToString());
      source = source.Replace("{%USERNAME%}", session.Username);

      return source;
    }
  }
}
