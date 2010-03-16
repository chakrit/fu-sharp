
using System.Collections.Generic;

namespace Fu
{
    public interface IWalkPath : IEnumerable<Step>
    {
        IEnumerable<Step> Steps { get; }

        void InsertNext(Step step);
        void InsertNextRange(IEnumerable<Step> step);

        Step DeleteNext();

        void Stop();
    }
}
