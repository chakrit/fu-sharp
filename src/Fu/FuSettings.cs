
using System;
using System.Collections.Generic;

namespace Fu
{
  [Serializable]
  public class FuSettings
  {
    public string[] Hosts = new[] { "localhost:80" };
    public string BasePath = "";

    public string Encoding = "utf-8";

    public bool EnableStats = false;
    public int ListenerThreads = 25;

    public readonly SessionSettings Session = new SessionSettings();
    public readonly ThreadPoolSettings ThreadPool = new ThreadPoolSettings();
  }

  [Serializable]
  public class SessionSettings
  {
    public int SessionIdLength = 128;
    public TimeSpan Timeout = TimeSpan.FromHours(4.0);

    public string CookiePath = "/";
    public string CookieDomain = null;
  }

  [Serializable]
  public class ThreadPoolSettings
  {
    public int MaxThreads = 25;
    public int MinThreads = 10;

    public int MaxIOThreads = 1000;
    public int MinIOThreads = 25;
  }
}
