
using System.Threading;

namespace Fu.Util
{
  // A modified version of CountDownLatch taken from:
  // http://msdn.microsoft.com/en-us/magazine/cc163427.aspx
  public class Counter
  {
    private Mutex _mutex;
    private int _remain;
    private EventWaitHandle _event;

    public Counter(int count)
    {
      _remain = count;
      _mutex = new Mutex();
      _event = new ManualResetEvent(count == 0);
    }

    public void Increment()
    {
      _mutex.WaitOne();
      if (++_remain > 0)
        _event.Reset();
      _mutex.ReleaseMutex();
    }

    public void Decrement()
    {
      _mutex.WaitOne();
      if (--_remain == 0)
        _event.Set();
      _mutex.ReleaseMutex();
    }

    public void WaitForZero()
    {
      _event.WaitOne();
    }
  }
}
