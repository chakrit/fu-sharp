
using System;
using System.Collections.Generic;

using MbUnit.Framework;
using Fu.Presets;

namespace Fu.Tests
{
    [TestFixture]
    public abstract class AppTestBase
    {
        private Dictionary<int, App> _portMap = new Dictionary<int, App>();
        private Random _random = new Random();


        public App CreateApp() { return CreateApp(null, new Step[] { }); }
        public App CreateApp(params Step[] steps) { return CreateApp(null, steps); }

        public virtual App CreateApp(string basePath, params Step[] steps)
        {
            var port = findUnusedPort();
            var settings = new FuSettings
            {
                Host = "localhost",
                Port = port,

                // we don't need a lot of threads for functionality testing
                // this helps speeds up the test runs
                ThreadPoolMaxThreads = 3,
                ThreadPoolMinThreads = 1,
            };

            if (!string.IsNullOrEmpty(basePath))
                settings.BasePath = basePath;

            var app = new SimpleApp(settings, steps);
            _portMap.Add(port, app);

            return app;
        }

        public void CleanupApp(App app)
        {
            app.Stop();
            _portMap.Remove(app.Settings.Port);

            app.Dispose();
        }

        [TearDown]
        public void CleanupApps()
        {
            foreach (var app in _portMap.Values)
            {
                app.Stop();
                app.Dispose();
            }

            _portMap.Clear();
        }


        protected CookieEnabledWebClient GetClient()
        { return new CookieEnabledWebClient(); }


        private int findUnusedPort()
        {
            while (true)
            {
                var port = _random.Next(1025, 65535);
                if (!_portMap.ContainsKey(port))
                    return port;
            }
        }
    }
}
