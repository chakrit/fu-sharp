// <auto-generated />
using Fu.Steps;

namespace Fu
{
	namespace Steps
	{
		public interface IHttpSteps { }
		public interface ICacheSteps { }
		public interface IMapSteps { }
		public interface IResultSteps { }
		public interface IRedirectSteps { }
		public interface IStaticSteps { }
		public interface IWalkSteps { }
	}

	public static partial class fu
    {
		// Extension points to "park" the extension methods in
		// on the fu class, these interfaces serve no other puposes
		// and should and always will be left null
		public static IHttpSteps Http { get { return null; } }
		public static ICacheSteps Cache { get { return null; } }
		public static IMapSteps Map { get { return null; } }
		public static IResultSteps Result { get { return null; } }
		public static IRedirectSteps Redirect { get { return null; } }
		public static IStaticSteps Static { get { return null; } }
		public static IWalkSteps Walk { get { return null; } }
    }
}
