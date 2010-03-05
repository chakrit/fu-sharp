
using System;
using System.Net;
using System.Threading;

using Fu.Util;

namespace Fu
{
    /// <summary>
    /// Contains server initialization logic
    /// </summary>
    public class Server : IServer
    {
        private Thread _spawnThread;

        private HttpListener _listener;
        private Semaphore _listenerCounter;
        private Counter _processorCounter;
        private ManualResetEvent _stopEvent;

        public FuSettings Settings { get; private set; }
        public Stats Stats { get; private set; }
        public string Url { get; private set; }

        public IWalker Walker { get; private set; }
        public bool IsServing { get; private set; }

        public Server(FuSettings settings, IWalker walker)
        {
            this.Settings = settings;
            this.Walker = walker;

            Url = "http://" + Settings.Host + ":" +
                Settings.Port.ToString() + "/";

            ifStats(s => Stats = new Stats());
        }

        public void Start()
        {
            ifStats(s => s.Reset());

            // configure the ThreadPool
            // TODO: Numbers may need serious adjustment
            var maxThreads = Settings.ThreadPoolMaxThreads;
            var maxIo = Settings.ThreadPoolMaxIOThreads;
            var minThreads = Settings.ThreadPoolMinThreads;
            var minIo = Settings.ThreadPoolMinIOThreads;

            ThreadPool.SetMaxThreads(maxThreads, maxIo);
            ThreadPool.SetMinThreads(minThreads, minIo);

            // setup the request processing loop
            _stopEvent = new ManualResetEvent(false);

            _processorCounter = new Counter(0);
            _listenerCounter = new Semaphore(
                Settings.ListenerThreads,
                Settings.ListenerThreads);

            _spawnThread = new Thread(spawnLoop)
            {
                Priority = ThreadPriority.AboveNormal,

                // Foreground thread prevents application from terminating
                // we don't want the app to end until the server is stopped
                IsBackground = false,
            };

            _listener = new HttpListener();
            _listener.Prefixes.Add(Url);

            // fire up the server
            _spawnThread.Start();
            IsServing = true;
        }

        public void Stop()
        {
            if (!IsServing)
                return;

            // 1. Signal everything to begin shutting down
            _stopEvent.Set();

            // 2. Stop the main spawn thread first.
            _spawnThread.Join();

            // 3. Stops any future listener from starting.
            _listener.Abort();

            // 4. Wait until all started listeners stop
            for (var i = 0; i < Settings.ListenerThreads; i++)
                _listenerCounter.WaitOne();

            // 5. Wait until all request processors have stopped
            _processorCounter.WaitForZero();

            // All threads are terminated by this point
            _processorCounter = null;
            _listenerCounter = null;
            _spawnThread = null;
            _listener = null;

            IsServing = false;
        }


        // this should be the only part of Fu where it's multi-threaded
        // in order to keep various parts of fu free to be synchronous and
        // easy to use and program against.
        private void spawnLoop()
        {
            try
            { _listener.Start(); }
            catch (HttpListenerException ex)
            {
                // This will produces a dialog box, which is OK since
                // apparently we havn't started the server just yet.
                FuTrace.Fail("Server.Start: " + ex.Message);
                return;
            }

            // Fu'step main request processing loop
            var handles = new WaitHandle[] { _listenerCounter, _stopEvent };

            while (WaitHandle.WaitAny(handles) == 0 /* _listenerCounter */)
            {
                // spawn #(_listenerCounter) threads to wait for incoming requests
                _listener.BeginGetContext(ar =>
                {
                    _listenerCounter.Release();
                    if (_stopEvent.WaitOne(0, true))
                        return;

                    // fetch the Request/Response pair for current connection
                    HttpListenerContext context = null;

                    try
                    { context = _listener.EndGetContext(ar); }
                    catch (ObjectDisposedException ex)
                    {
                        FuTrace.Exception(ex);
                        throw;
                    }
                    catch (HttpListenerException ex)
                    {
                        FuTrace.Exception(ex);
                        throw;
                    }

                    // use threadpool thread to process the request
                    ifStats(s => s.IncrementRequest());
                    ThreadPool.QueueUserWorkItem(_ => requestProcessor(context));

                }, null);
            }

            // stop signal has been raised by this point
        }

        private void requestProcessor(HttpListenerContext context)
        {
            if (_stopEvent.WaitOne(0, true))
                return;

            _processorCounter.Increment();

            try
            {
                Walker.Walk(context);
                ifStats(s => s.IncrementResponse());
            }
            catch (Exception ex)
            {
                /* TODO: Properly handle this absorbtion */
                FuTrace.Exception(ex);

                // attempts to close any dangling connection
                // so we never hangs the browser
                try { context.Response.Close(); }
                catch { }

            }
            finally { _processorCounter.Decrement(); }
        }


        private void ifStats(Action<Stats> statsAction)
        { if (Settings.EnableStats) statsAction(Stats); }

        public void Dispose()
        { if (IsServing) Stop(); }
    }
}
