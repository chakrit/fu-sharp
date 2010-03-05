
using System;
using System.IO;
using System.Linq;
using System.Net;

using MbUnit.Framework;

using Fu.Results;
using Fu.Steps;

namespace Fu.Tests
{
    [TestFixture]
    public class SimpleApps : AppTestBase
    {
        [Test]
        public void BasicAppTest()
        {
            var text = "Hello, World!";

            var app = CreateApp(fu.Static.Text(text));
            app.Start();

            var client = GetClient();
            var str = client.DownloadString(app.Server.Url);
            Assert.AreEqual(text, str);

            client.Dispose();
            CleanupApp(app);
        }

        [Test]
        public void EchoAppTest()
        {
            var template = "REQUEST: {0}";

            var app = CreateApp(c => StringResult.From(c,
                string.Format(template, c.Request.Url.AbsolutePath)));

            app.Start();

            var client = GetClient();
            var str = client.DownloadString(app.Server.Url + "test1111");
            Assert.Contains(str, "test1111");

            str = client.DownloadString(app.Server.Url + "test2222");
            Assert.Contains(str, "test2222");

            client.Dispose();
            CleanupApp(app);
        }

        [Test]
        public void ErrorAppTest()
        {
            var app = CreateApp(
                fu.Void(c =>
                {
                    var code = int.Parse(c.Request.Url.AbsolutePath.Substring(1));
                    var desc = c.Request.QueryString["msg"];

                    c.WalkPath.InsertNext(fu.Http.Status(code, desc));
                }));

            app.Start();

            var client = GetClient();

            try
            {
                client.DownloadString(app.Server.Url + "404?msg=Not-Found");
                Assert.Fail("Exception expected.");
            }
            catch (WebException ex)
            {
                Assert.AreEqual(404, (int)((HttpWebResponse)ex.Response).StatusCode);
            }

            try
            {
                client.DownloadString(app.Server.Url + "503?msg=Unavailable");
                Assert.Fail("Exception expected.");
            }
            catch (WebException ex)
            {
                Assert.AreEqual(503, (int)((HttpWebResponse)ex.Response).StatusCode);
            }

            client.Dispose();
            CleanupApp(app);
        }

        [Test]
        public void StaticFileAppTest()
        {
            // create a temp file
            var fileContent = "THE QUICK BROWN DOG JUMPS OVER THE LAZY FOX";
            var tempFilename = Path.GetTempFileName() + ".txt";
            var tempPath = Path.GetTempPath();

            File.WriteAllText(tempFilename, fileContent);
            tempFilename = Path.GetFileName(tempFilename);

            // create an app that reads the temp file
            var app = CreateApp(tempPath, new[] {
                fu.Static.File(tempFilename)
            });

            app.Start();

            var client = GetClient();
            var str = client.DownloadString(app.Server.Url);
            Assert.AreEqual(fileContent, str);

            client.Dispose();
            CleanupApp(app);

            // cleanup temp file after app has closed
            // to ensure app'returnStep not locking the file
            File.Delete(Path.Combine(tempPath, tempFilename));
        }

        [Test]
        public void StaticFolderApp()
        {
            // create a few temp files
            var random = new Random();
            var tempPath = Path.GetTempPath();
            var tempFiles = Enumerable.Range(0, 3)
                .Select(i =>
                {
                    var filePath = Path.GetTempFileName();

                    return new
                    {
                        Content = "ASDAOEIJFOSIEJF" + random.Next().ToString(),
                        Path = filePath,
                        Filename = Path.GetFileName(filePath),
                    };
                })
                .ToArray();

            foreach (var tempFile in tempFiles)
            {
                File.WriteAllText(tempFile.Path, tempFile.Content);
                Assert.AreEqual(1, 1);
            }

            // create an app mapped to the temp folder
            var app = CreateApp(tempPath, new[] {
                fu.Static.Folder("/", Path.GetTempPath())
            });

            app.Start();

            // try to download the temp files
            var client = GetClient();

            foreach (var tempFile in tempFiles)
            {
                var str = client.DownloadString(app.Server.Url + tempFile.Filename);
                Assert.AreEqual(tempFile.Content, str);
            }

            // try to request a non-existent file
            try
            {
                var ghostFilename = "_N_E_V_E_R_EXISTS_cijfejedj.awedsc";
                var str = client.DownloadString(app.Server.Url + ghostFilename);

                Assert.Fail("Exception expected.");
            }
            catch (WebException ex)
            {
                Assert.AreEqual(404, (int)((HttpWebResponse)ex.Response).StatusCode);
            }

            client.Dispose();
            CleanupApp(app);
        }

        [Test]
        public void BasicUrlMappedAppTest()
        {
            var text1 = "Hello, World!";
            var text2 = "Another Url";

            var map = fu.Map.Urls(
                new UrlMap("^/_hello$", fu.Static.Text(text1)),
                new UrlMap("^/_txt$", fu.Static.Text(text2)));

            var app = CreateApp(map);

            app.Start();

            var client = GetClient();
            var str = client.DownloadString(app.Server.Url + "_hello");
            Assert.AreEqual(text1, str);

            str = client.DownloadString(app.Server.Url + "_txt");
            Assert.AreEqual(text2, str);

            client.Dispose();
            CleanupApp(app);
        }
    }
}
