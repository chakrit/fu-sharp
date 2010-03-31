
using System.Collections.Specialized;
using System.IO;

using MbUnit.Framework;

using Fu.Services.Web;

namespace Fu.Tests
{
  // disabled temporarily until multipart implementation is complete
  [TestFixture]
  public class FormDataTests : AppTestBase
  {
    [Test]
    public void BasicFormDataTest()
    {
      // define some test data
      var testFoo = "asdf";
      var testBar = "a\r\n\r\txda&&&&===";

      var resultFoo = "";
      var resultBar = "";

      // create an app that accepts the values
      var app = CreateApp(fu.Void(c =>
      {
        var data = c.Get<IFormData>();

        resultFoo = data["foo"];
        resultBar = data["bar"];
      }));

      app.Services.Add(new FormDataParser());
      app.Start();

      // try submitting the form
      var values = new NameValueCollection();

      values.Add("foo", testFoo);
      values.Add("bar", testBar);

      var client = this.GetClient();
      client.UploadValues(app.Server.Url, values);

      Assert.AreEqual(testFoo, resultFoo);
      Assert.AreEqual(testBar, resultBar);

      client.Dispose();
      CleanupApp(app);
    }

    [Test]
    public void BasicFileUploadTest()
    {
      // prepare test content
      var content = "HELLO----@#$%^&*\r\n\n\r\n\r\n\n\r\n(#*&$(@*#SDLKJ";
      var tempFilename = Path.GetTempFileName();

      File.WriteAllText(tempFilename, content);
      var contentBytes = File.ReadAllBytes(tempFilename);
      byte[] resultBytes = new byte[contentBytes.Length];

      // make an uploader app
      var app = CreateApp(c =>
      {
        var data = c.Get<IFormData>();

        // shouldn't need chunking since our test data is small enough
        var stream = data.Files[0].OpenRead();

        stream.Read(resultBytes, 0, contentBytes.Length);
        stream.Close();
        stream.Dispose();

        return c;
      });

      app.Services.Add(new MultipartFormDataParser());
      app.Start();

      // attemp the upload
      var client = GetClient();
      client.UploadFile(app.Server.Url, tempFilename);

      Assert.AreElementsEqual(contentBytes, resultBytes);

      // cleanup
      File.Delete(tempFilename);

      client.Dispose();
      CleanupApp(app);
    }
  }
}
