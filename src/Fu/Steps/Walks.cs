
using Fu.Exceptions;

namespace Fu.Steps
{
    public static class Walks
    {
        public static Step Stop(this IWalkSteps _)
        { return fu.Void(c => c.WalkPath.Stop()); }

        public static Step SkipNext(this IWalkSteps _)
        { return fu.Void(c => c.WalkPath.DeleteNext()); }

        public static Step SkipBy(this IWalkSteps _, int steps)
        {
            return fu.Void(c =>
            {
                for (var i = 0; i < steps; i++) c.WalkPath.DeleteNext();
            });
        }
    }
}
