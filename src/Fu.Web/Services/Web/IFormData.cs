
using System.IO;
using System.Collections.Generic;

namespace Fu.Services.Web
{
    public interface IFormData : IEnumerable<KeyValuePair<string, string>>
    {
        string this[string key] { get; }
        string this[int index] { get; }

        IUploadedFilesCollection Files { get; }

        string Get(string key);
        string Get(int index);

        UploadedFile GetFile(string key);
        UploadedFile GetFile(int index);
    }
}
