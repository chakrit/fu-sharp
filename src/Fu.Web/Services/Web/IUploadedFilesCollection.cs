
using System.Collections.Generic;

namespace Fu.Services.Web
{
  public interface IUploadedFilesCollection : IEnumerable<KeyValuePair<string, UploadedFile>>
  {
    UploadedFile this[string key] { get; }
    UploadedFile this[int index] { get; }

    UploadedFile Get(string key);
    UploadedFile Get(int index);
  }
}
