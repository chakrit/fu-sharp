
using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using System.IO;

namespace Fu
{
  [Serializable]
  public partial class FuSettings
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


  public partial class FuSettings
  {
    private static XmlSerializerFactory _factory =
      new XmlSerializerFactory();


    public void SaveTo(string filename)
    {
      var serializer = _factory.CreateSerializer(typeof(FuSettings));
      var fs = File.OpenWrite(filename);

      serializer.Serialize(fs, this);
      fs.Flush();
      fs.Close();
      fs.Dispose();
    }

    public static FuSettings LoadFrom(string filename)
    {
      var serializer = _factory.CreateSerializer(typeof(FuSettings));
      var fs = File.OpenRead(filename);

      var result = serializer.Deserialize(fs);
      fs.Close();
      fs.Dispose();

      return (FuSettings)result;
    }
  }
}
