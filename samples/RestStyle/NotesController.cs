
using System.Collections.Generic;

using Fu;
using Fu.Contexts;
using Fu.Results;
using Fu.Services.Web;
using Fu.Steps;

namespace RestStyle
{
    public class NotesController : RestController
    {
        private IList<string> _notes;
        private object _lock;


        public override void Initialize()
        {
            _notes = new List<string>();
            _lock = new object();


            // homepage
            Get("^/$", fu.Static.File("index.html"));

            // notes API
            Get("^/notes$", c => JsonResult.From(c, _notes));

            Put("^/notes$", c =>
            {
                var note = c.Get<IFormData>()["note"];

                _notes.Add(note);
                return JsonResult.From(c, note);
            });

            Delete("^/notes/(.+)$", fu.Step<IUrlMappedContext>(c =>
            {
                var note = c.Match.Groups[1].Value;
                _notes.Remove(note);

                return JsonResult.From(c, new { ok = true });
            }));
        }
    }
}
