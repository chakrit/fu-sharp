
using System;
using System.IO;
using System.Web;
using System.Text;

namespace Fu.Services.Web
{
    public class FormDataParser : IService<IFormData>
    {
        public bool CanGetServiceObject(IFuContext input)
        {
            // only support POST and PUT
            var method = input.Request.HttpMethod.ToUpper();
            if (method != "POST" &&
                method != "PUT")
                return false;

            // make sure we have something to parse out of
            if (!input.Request.HasEntityBody)
                return false;

            // this implementation does not support multipart encoding
            var header = input.Request.Headers["Content-Type"];
            if (!string.IsNullOrEmpty(header) &&
                header.StartsWith("multipart/form-data",
                StringComparison.InvariantCultureIgnoreCase))
                return false;

            // everything else should be ok.
            return true;
        }

        public IFormData GetServiceObject(IFuContext c)
        {
            // force ANSI encoding (whatever the current system is set to)
            var encoding = Encoding.GetEncoding(c.Settings.Encoding);

            var input = c.Request.InputStream;
            var sr = new StreamReader(input, encoding);

            var data = sr.ReadToEnd();
            sr.Close();
            sr.Dispose();

            var nv = HttpUtility.ParseQueryString(data);
            return new FormDataImpl(nv, null);
        }

        public void BeginWalk(IFuContext input) { /* no-op */ }
        public void EndWalk(IFuContext input) { /* no-op */ }
    }
}
