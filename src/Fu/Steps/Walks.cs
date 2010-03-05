
using System;

using Fu.Exceptions;

namespace Fu.Steps
{
    public static class Walks
    {
        public static Step Stop(this IWalksSteps _)
        { return c => { throw new StopWalkException(); }; }

        public static Step SkipNext(this IWalksSteps _)
        { return fu.Void(c => c.WalkPath.DeleteNext()); }

        public static Step SkipBy(this IWalksSteps _, int steps)
        {
            return fu.Void(c =>
            {
                for (var i = 0; i < steps; i++) c.WalkPath.DeleteNext();
            });
        }
    }
}
