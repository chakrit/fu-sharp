
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Fu.Services.Web
{
  public class UploadedFilesCollection : IUploadedFilesCollection
  {
    private IDictionary<string, UploadedFile> _src;
    private UploadedFile[] _values;


    public UploadedFile this[string key] { get { return Get(key); } }
    public UploadedFile this[int index] { get { return Get(index); } }

    public UploadedFilesCollection(IDictionary<string, UploadedFile> src)
    {
      _src = src;
      _values = (src != null) ?
        src.Values.ToArray() :
        new UploadedFile[] { };
    }


    public UploadedFile Get(string key) { return _src[key]; }
    public UploadedFile Get(int index) { return _values[index]; }


    IEnumerator IEnumerable.GetEnumerator()
    { return (IEnumerator)GetEnumerator(); }

    public IEnumerator<KeyValuePair<string, UploadedFile>> GetEnumerator()
    { return _src.GetEnumerator(); }
  }
}
