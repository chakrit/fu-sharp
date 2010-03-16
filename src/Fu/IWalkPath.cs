
using System.Collections.Generic;

namespace Fu
{
    public interface IWalkPath : IEnumerable<Step>
    {
        IEnumerable<Step> Steps { get; }

        void InsertNext(Step step);
        void InsertNextRange(params Step[] steps);
        void InsertNextRange(IEnumerable<Step> steps);

        Step DeleteNext();

        void Stop();
    }
}
