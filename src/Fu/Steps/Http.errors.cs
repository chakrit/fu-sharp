
using System.Collections.Generic;

using Fu.Contexts;
using Fu.Results;

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
		
		public static Step BadRequest(this IHttpSteps _)
		{
			return fu.Compose(
				_.Status(400),
				c => StringResult.From(c, "BadRequest"),
				fu.Render.Result(),
				fu.Walks.Stop());
		}
		
		public static Step Unauthorized(this IHttpSteps _)
		{
			return fu.Compose(
				_.Status(401),
				c => StringResult.From(c, "Unauthorized"),
				fu.Render.Result(),
				fu.Walks.Stop());
		}
		
		public static Step Forbidden(this IHttpSteps _)
		{
			return fu.Compose(
				_.Status(403),
				c => StringResult.From(c, "Forbidden"),
				fu.Render.Result(),
				fu.Walks.Stop());
		}
		
		public static Step NotFound(this IHttpSteps _)
		{
			return fu.Compose(
				_.Status(404),
				c => StringResult.From(c, "NotFound"),
				fu.Render.Result(),
				fu.Walks.Stop());
		}
		
		public static Step MethodNotAllowed(this IHttpSteps _)
		{
			return fu.Compose(
				_.Status(405),
				c => StringResult.From(c, "MethodNotAllowed"),
				fu.Render.Result(),
				fu.Walks.Stop());
		}
		
		public static Step ServerError(this IHttpSteps _)
		{
			return fu.Compose(
				_.Status(500),
				c => StringResult.From(c, "ServerError"),
				fu.Render.Result(),
				fu.Walks.Stop());
		}
		
		public static Step ServiceUnavailable(this IHttpSteps _)
		{
			return fu.Compose(
				_.Status(503),
				c => StringResult.From(c, "ServiceUnavailable"),
				fu.Render.Result(),
				fu.Walks.Stop());
		}
		
	}
}