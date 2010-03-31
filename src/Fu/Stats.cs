
using System;

namespace Fu
{
  public class Stats
  {
    private long _totalRequests, _totalResponses;
    private object _reqLock, _respLock;

    public DateTime StartTime { get; set; }
    public DateTime StopTime { get; set; }

    public long TotalRequests { get { return _totalRequests; } }
    public long TotalResponses { get { return _totalResponses; } }

    public Stats()
    {
      _reqLock = new object();
      _respLock = new object();
      Reset();
    }


    public void Reset()
    {
      lock (_reqLock)
        lock (_respLock) {
          StartTime = StopTime = DateTime.Now;
          _totalRequests = _totalResponses = 0;
        }
    }


    public void IncrementRequest()
    { lock (_reqLock) { _totalRequests += 1; } }

    public void IncrementResponse()
    { lock (_respLock) { _totalResponses += 1; } }
  }
}
