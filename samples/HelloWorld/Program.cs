
using Fu;
using Fu.Presets;
using Fu.Steps;

namespace HelloWorld
{
    class Program
    {
        static void Main(string[] args)
        {
            var app = new SimpleApp(fu.Static.Text("Hello World!!"));
            app.Start();
        }
    }
}
