
using System;
using System.Collections.Generic;
using System.Linq;

using Fu.Services;

namespace Fu
{
    public class App : IApp
    {
        private IServer _server;


        public FuSettings Settings { get; private set; }
        public Stats Stats { get { return _server.Stats; } }

        public IList<Step> Steps { get; private set; }
        public IList<IService> Services { get; private set; }

        public IServer Server { get { return _server; } }


        public App(FuSettings settings, IEnumerable<IService> services, IEnumerable<Step> steps)
        {
            settings = settings ?? FuSettings.Default;
            steps = steps ?? new Step[] { };
            services = services ?? new IService[] { };

            this.Settings = settings;

            this.Steps = steps.ToList();
            this.Services = services.ToList();
        }


        public void Start()
        {
            // validate settings and fill in any defaults
            if (string.IsNullOrEmpty(Settings.BasePath))
                Settings.BasePath = Environment.CurrentDirectory;

            var walker = new Walker(Settings, Services, Steps);
            _server = new Server(Settings, walker);

            _server.Start();
        }

        public void Stop()
        {
            _server.Stop();
        }


        public void Dispose()
        {
            if (_server != null)
                _server.Dispose();
        }
    }
}
