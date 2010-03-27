
using System;
using System.Web.Script.Serialization;

using Fu.Contexts;

namespace Fu.Results
{
    public class JsonResult : StringResult
    {
        public object Object { get; protected set; }

        public JsonResult(object o) :
            base(new JavaScriptSerializer().Serialize(o))
        {
            MediaType = Mime.AppJson;
            Object = o;
        }


        public static ResultContext From(IFuContext input, object o)
        { return new ResultContext(input, new JsonResult(o)); }

        public static ResultContext From(IFuContext input,
            Reduce<object> jsonStep)
        { return new ResultContext(input, new JsonResult(jsonStep(input))); }
    }
}
