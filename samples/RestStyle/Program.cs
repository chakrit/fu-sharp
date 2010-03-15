
using Fu;
using Fu.Presets;
using Fu.Steps;
using Fu.Services.Sessions;

namespace RestStyle
{
    class Program
    {
        static void Main(string[] args)
        {
            var step = fu.Map.Controller(new AppController());

            var app = new SimpleWebApp(step);
            app.Start();
        }
    }
}
