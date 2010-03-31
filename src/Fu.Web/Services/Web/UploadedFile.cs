
using System;
using System.IO;
using System.Net.Mime;

namespace Fu.Services.Web
{
  public class UploadedFile : IDisposable
  {
    private string _tempFilename;


    public string Filename { get; protected set; }

    public ContentType ContentType { get; protected set; }
    public ContentDisposition ContentDisposition { get; protected set; }

    public UploadedFile(ContentType contentType,
        ContentDisposition disposition, string tempFilename) :
      this(disposition.FileName, contentType, disposition, tempFilename) { }

    public UploadedFile(string filename,
        ContentType contentType,
        ContentDisposition contentDisposition,
        string tempFilename)
    {
      ContentDisposition = contentDisposition;
      ContentType = contentType;

      Filename = filename;
      _tempFilename = tempFilename;
    }


    public void SaveAs(string filename)
    {
      File.Delete(filename);
      File.Move(_tempFilename, filename);
    }

    public FileStream OpenRead()
    { return new FileStream(_tempFilename, FileMode.Open); }


    public void Dispose()
    {
      if (File.Exists(_tempFilename))
        File.Delete(_tempFilename);
    }
  }
}
