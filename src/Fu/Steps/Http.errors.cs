
using System.Collections.Generic;

namespace Fu.Steps
{
	public static partial class Http
	{
		public static IDictionary<int, string> Statuses
    {
      get
      {
        return new Dictionary<int, string>()
        {
					{ 400, "BadRequest" },
					{ 401, "Unauthorized" },
					{ 403, "Forbidden" },
					{ 404, "NotFound" },
					{ 405, "MethodNotAllowed" },
					{ 500, "ServerError" },
					{ 503, "ServiceUnavailable" },
        };
      }
    }
		
		public static Continuation BadRequest(this IHttpSteps _)
		{
			return fu.Compose(
				_.Status(400, "BadRequest"),
				fu.Static.Text("BadRequest"),
				fu.Result.Render(),
				fu.End);
		}
		
		public static Continuation Unauthorized(this IHttpSteps _)
		{
			return fu.Compose(
				_.Status(401, "Unauthorized"),
				fu.Static.Text("Unauthorized"),
				fu.Result.Render(),
				fu.End);
		}
		
		public static Continuation Forbidden(this IHttpSteps _)
		{
			return fu.Compose(
				_.Status(403, "Forbidden"),
				fu.Static.Text("Forbidden"),
				fu.Result.Render(),
				fu.End);
		}
		
		public static Continuation NotFound(this IHttpSteps _)
		{
			return fu.Compose(
				_.Status(404, "NotFound"),
				fu.Static.Text("NotFound"),
				fu.Result.Render(),
				fu.End);
		}
		
		public static Continuation MethodNotAllowed(this IHttpSteps _)
		{
			return fu.Compose(
				_.Status(405, "MethodNotAllowed"),
				fu.Static.Text("MethodNotAllowed"),
				fu.Result.Render(),
				fu.End);
		}
		
		public static Continuation ServerError(this IHttpSteps _)
		{
			return fu.Compose(
				_.Status(500, "ServerError"),
				fu.Static.Text("ServerError"),
				fu.Result.Render(),
				fu.End);
		}
		
		public static Continuation ServiceUnavailable(this IHttpSteps _)
		{
			return fu.Compose(
				_.Status(503, "ServiceUnavailable"),
				fu.Static.Text("ServiceUnavailable"),
				fu.Result.Render(),
				fu.End);
		}
		
	}
}