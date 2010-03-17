
using System.Collections.Generic;

namespace Fu
{
    public interface IWalkPath : IEnumerable<Step>
    {
        IEnumerable<Step> Steps { get; }

        void InsertNext(Step step);
        void InsertNext(params Step[] steps);
        void InsertNext(IEnumerable<Step> steps);

        Step DeleteNext();

        void Stop();
    }
}
