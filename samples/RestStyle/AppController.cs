
using Fu;
using Fu.Steps;
using Fu.Services.Web;
using Fu.Results;
using Fu.Contexts;
using System.Collections.Generic;

namespace RestStyle
{
    public class AppController : RestController
    {
        private IList<string> _notes;
        private object _lock;


        public AppController()
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
