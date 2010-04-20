
using System.Collections.Generic;

using Fu;
using Fu.Results;
using Fu.Services.Web;
using Fu.Steps;

namespace RestStyle
{
  public class NotesController : RestStyleController
  {
    private IList<string> _notes;
    private object _lock;


    public override void Initialize()
    {
      _notes = new List<string>();
      _lock = new object();

      // homepage
      Get("/", fu.Static.File("index.html"));
      Get("/site.js", fu.Static.File("site.js"));

      // notes API
      Get("/notes", c => new JsonResult(_notes));

      Put("/notes", c =>
      {
        var note = c.Get<IFormData>()["note"];

        _notes.Add(note);
        return new JsonResult(note);
      });

      Delete("/notes/(.+)", c =>
      {
        var note = c.Match.Groups[1].Value;
        _notes.Remove(note);

        return new JsonResult(new { ok = true });
      });
    }
  }
}
