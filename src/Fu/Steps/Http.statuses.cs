
using System.Collections.Generic;

using Fu.Contexts;
using Fu.Results;

namespace Fu.Steps
{
	public static partial class Http
	{
		public static Step OK(this IHttpSteps _)
		{ return _.Status(200, "OK"); }
		
		public static Step MovedPermanently(this IHttpSteps _)
		{ return _.Status(301, "MovedPermanently"); }
		
		public static Step Found(this IHttpSteps _)
		{ return _.Status(302, "Found"); }
		
		public static Step NotModified(this IHttpSteps _)
		{ return _.Status(304, "NotModified"); }
		
	}
}