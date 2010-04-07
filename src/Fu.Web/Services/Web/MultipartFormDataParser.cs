
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
    private readonly FuAction _badRequest = fu.Http.BadRequest()(fu.EndAct);

    public bool CanGetServiceObject(IFuContext input)
    {
      // only support PUT/POST/DELETE
      var method = input.Request.HttpMethod;
      if (method != "POST" &&
        method != "PUT" &&
        method != "DELETE")
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
        _badRequest;

      // parse the data
      var parser = new MultipartParser(
        input.Request.InputStream,
        Encoding.GetEncoding(input.Settings.Encoding),
        Encoding.Default.GetBytes(boundary));

      // build up a formdata representation
      try { parser.Parse(); }
      catch (Exception ex) { /* TODO: Properly absorbs this*/ BadRequest(input, ex); }

      // build an IFormData implementation
      var nv = new NameValueCollection();
      foreach (var pair in parser.Values)
        nv.Add(pair.Key, pair.Value);

      return new FormDataImpl(nv, parser.Files);
    }


    private void BadRequest(IFuContext input, Exception ex)
    {
      FuTrace.Exception(ex);
      _badRequest(input);

      throw new BadRequestDataException("Bad request", ex);
    }
  }
}
