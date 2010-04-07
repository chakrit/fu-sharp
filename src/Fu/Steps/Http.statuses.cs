
namespace Fu.Steps
{
	public static partial class Http
	{
		public static Continuation OK(this IHttpSteps _)
		{ return _.Status(200, "OK"); }
		
		public static Continuation MovedPermanently(this IHttpSteps _)
		{ return _.Status(301, "MovedPermanently"); }
		
		public static Continuation Found(this IHttpSteps _)
		{ return _.Status(302, "Found"); }
		
		public static Continuation NotModified(this IHttpSteps _)
		{ return _.Status(304, "NotModified"); }
		
	}
}