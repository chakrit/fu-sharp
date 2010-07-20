
using Fu;
using Fu.Presets;
using Fu.Steps;

namespace HelloWorld
{
  public class Program
  {
    public static void Main(string[] args)
    {
      var app = new SimpleApp(fu.Static.Text("Hello World!!"));
      app.Start();
    }
  }
}
