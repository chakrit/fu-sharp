
using System;
using System.Collections.Specialized;
using System.Net.Mime;
using System.Text;

using Fu.Exceptions;
using Fu.Steps;

namespace Fu.Services.Web
{
    public class MultipartFormDataParser : IService<IFormData>
    {
        public bool CanGetServiceObject(IFuContext input)
        {
            // only support POST and PUT
            var method = input.Request.HttpMethod;
            if (method != "POST" &&
                method != "PUT")
                return false;

            // make sure we have something to parse out of
            if (!input.Request.HasEntityBody)
                return false;

            // this implementation only works when content-type
            // is set to multipart/form-data
            var header = input.Request.Headers["Content-Type"];
            if (string.IsNullOrEmpty(header) ||
                !header.StartsWith("multipart/form-data", StrComp.Fast))
                return false;

            // we should be fine parsing this request at this point in time
            return true;
        }

        public IFormData GetServiceObject(IFuContext input)
        {
            var ctHeader = input.Request.Headers["Content-Type"];
            var contentType = new ContentType(ctHeader);
            var boundary = contentType.Boundary;

            if (string.IsNullOrEmpty(boundary) ||
                boundary.Length > 70)
                BadRequest(input);

            // parse the data
            var parser = new MultipartParser(
                input.Request.InputStream,
                Encoding.GetEncoding(input.Settings.Encoding),
                Encoding.Default.GetBytes(boundary));

            // build up a formdata representation
            try { parser.Parse(); }
            catch (Exception) { /* TODO: Properly absorbs this*/ BadRequest(input); }

            // build an IFormData implementation
            var nv = new NameValueCollection();
            foreach (var pair in parser.Values)
                nv.Add(pair.Key, pair.Value);

            return new FormDataImpl(nv, parser.Files);
        }


        public void BeginWalk(IFuContext input) { /* no-op */ }
        public void EndWalk(IFuContext input) { /* no-op */ }


        private void BadRequest(IFuContext input)
        {
            input.WalkPath.InsertNext(fu.Http.BadRequest());
            throw new SkipStepException();
        }
    }
}
