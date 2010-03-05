
using MbUnit.Framework;

using Fu.Results;
using Fu.Services.Sessions;
using Fu.Steps;

namespace Fu.Tests.Services
{
    [TestFixture]
    public class SessionsTest : AppTestBase
    {
        // include the session service in this test
        public override App CreateApp(string basePath, params Step[] steps)
        {
            var app = base.CreateApp(basePath, steps);
            app.Services.Add(new InMemorySessionService());

            return app;
        }


        [Test]
        public void BasicSessionTest()
        {
            var keyName = "TEST";
            var testValue = "SDFLIJLEISJFLISJELKDFJLSIEJFLIJ";

            // build an app with session getter/setter
            var map = fu.Map.Urls(

                new UrlMap("^/get$", c => StringResult.From(c,
                    (c.Get<ISession>().Get(keyName) ?? "").ToString())),

                new UrlMap("^/set$", fu.Compose(
                    fu.Void(c => c.Get<ISession>().Set(keyName, c.Request.QueryString[keyName])),
                    fu.Static.Text("OK"))));

            var app = CreateApp(map);
            app.Start();

            var client = GetClient();

            // first get should results in nothing
            var result = client.DownloadString(app.Server.Url + "get");
            Assert.IsTrue(string.IsNullOrEmpty(result));

            // then lets set some values
            result = client.DownloadString(string.Format(
                app.Server.Url + "set?{0}={1}", keyName, testValue));
            Assert.AreEqual("OK", result);

            // check if the value in the last returnStep is session-saved
            result = client.DownloadString(app.Server.Url + "get");
            Assert.AreEqual(testValue, result);

            client.Dispose();
            CleanupApp(app);
        }
    }
}
