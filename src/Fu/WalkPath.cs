
using System.Collections;
using System.Collections.Generic;

namespace Fu
{
    // TODO: Add verification that each steps supply compatible objects
    //       for the next one
    // TODO: Make WalkPath work sensibly with fu.Compose and/or make
    //       parts of Fu that do (Compose) properly supports WalkPath
    //       i.e. FuController
    // TODO: Provide a way to STOP walk without needing to throw exceptions
    //       maybe by setting a special IWalkPath flag that would stop
    //       enumeration immediatly
    public class WalkPath : IWalkPath
    {
        private LinkedListNode<Step> _current;
        private LinkedListNode<Step> _appendPoint;

        private LinkedList<Step> _steps;


        public IEnumerable<Step> Steps { get { return this; } }


        public WalkPath(IEnumerable<Step> steps)
        {
            _steps = new LinkedList<Step>(steps);
        }


        public void InsertNext(Step step)
        {
            _appendPoint = _steps.AddAfter(_appendPoint, step);
        }

        public void InsertNextRange(IEnumerable<Step> steps)
        {
            foreach (var step in steps)
                _appendPoint = _steps.AddAfter(_appendPoint, step);
        }


        public Step DeleteNext()
        {
            var next = _current.Next;

            // advance the appendpoint, if all the steps
            // after _current and before _appendPoint have depleted
            if (_appendPoint == next)
                _appendPoint = next.Next;

            _steps.Remove(next);

            return (next != null) ?
                next.Value :
                null;
        }


        public void Stop()
        {
            // jump to last step
            _current = _appendPoint = _steps.Last;
        }


        public IEnumerator<Step> GetEnumerator()
        {
            _current = _steps.First;

            while (_current != null)
            {
                _appendPoint = _current;
                yield return _current.Value;

                _current = _current.Next;
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            foreach (var item in this)
                yield return item;
        }
    }
}
