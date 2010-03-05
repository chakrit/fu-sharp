
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;

namespace Fu.Services.Web
{
    internal class FormDataImpl : IFormData
    {
        private NameValueCollection _values;
        private IUploadedFilesCollection _files;


        public string this[string key] { get { return Get(key); } }
        public string this[int index] { get { return Get(index); } }

        public IUploadedFilesCollection Files { get { return _files; } }

        public FormDataImpl(NameValueCollection values,
            IDictionary<string, UploadedFile> files)
        {
            _values = values;
            _files = new UploadedFilesCollection(files);
        }


        public string Get(string key) { return _values[key]; }
        public string Get(int index) { return _values[index]; }

        public UploadedFile GetFile(string key) { return _files[key]; }
        public UploadedFile GetFile(int index) { return _files[index]; }


        public IEnumerator<KeyValuePair<string, string>> GetEnumerator()
        {
            foreach (var key in _values.AllKeys)
                yield return new KeyValuePair<string, string>(key, _values[key]);
        }

        IEnumerator IEnumerable.GetEnumerator()
        { return (IEnumerator)GetEnumerator(); }
    }
}
