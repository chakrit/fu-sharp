
using System;
using System.IO;
using System.Net;
using System.Diagnostics;
using System.Threading;

using Timer = System.Timers.Timer;

namespace Fu.Loader
{
    class Program
    {
        private const string TestUrl = "http://192.168.192.17/load/helloworld";
        private const int TestPort = 80;

        private const int LoadDelay = 0;
        private const int LoadThreads = 300;

        private static DateTime _startTime;
        private static int _requests;
        private static int _responses;


        //[MTAThread]
        static void Main(string[] args)
        {
            // ensure the reporting thread itself are not swamped by the load
            Thread.CurrentThread.Priority = ThreadPriority.Highest;

            // starts loading
            _requests = _responses = 0;
            startLoader(TestUrl);
            _startTime = DateTime.Now;

            // setup stats reporting
            var tmpl =
                "  --------------------------------  \n" +
                "         Requests  :  {0,10:0.00}   \n" +
                "        Responses  :  {1,10:0.00}   \n" +
                "         Underway  :  {2,10:0.00}   \n" +
                "     Time elapsed  :  {3,10:0.00}   \n" +
                "  --------------------------------  \n" +
                "  Requests/second  :  {4,10:0.00}   \n" +
                "  --------------------------------  \n" +
                "";

            while (true)
            {
                Thread.Sleep(1000);

                var requests = _requests;
                var responses = _responses;

                var underway = requests - responses;
                var seconds = (DateTime.Now - _startTime).TotalSeconds;

                var rate = responses / seconds;

                Console.Clear();
                Console.WriteLine(tmpl, _requests, _responses, underway,
                    seconds, rate);

                if (Console.KeyAvailable)
                    break;
            };
        }

        private static void startLoader(string url)
        {
            new Thread(() =>
            {
                for (var i = 0; i < LoadThreads; i++)
                    new Thread(() => innerLoader(url))
                    {
                        // keep the main reporting thread alive
                        Priority = ThreadPriority.BelowNormal,
                        IsBackground = true,
                    }.Start();
            })
            {
                // ditch out all load threads as fast as possible
                Priority = ThreadPriority.Highest
            }.Start();
        }

        private static void innerLoader(string url)
        {
            while (true)
            {
                if (LoadDelay > 0)
                    Thread.Sleep(LoadDelay);

                try
                {
                    var request = HttpWebRequest.CreateDefault(new Uri(url));

                    Interlocked.Increment(ref _requests);
                    var response = request.GetResponse();

                    var sr = new StreamReader(response.GetResponseStream());
                    var str = sr.ReadToEnd();
                    response.Close();
                    Interlocked.Increment(ref _responses);

                    sr.Dispose();
                }
                catch
                {
                    // absorbed
                }
            }
        }

    }
}
