
using System;
using System.Net;
using System.Net.Cache;

namespace Fu.Tests
{
    public class CookieEnabledWebClient : WebClient
    {
        CookieContainer _cookieBox = new CookieContainer();
        HttpWebRequest _request;


        public CookieEnabledWebClient() :
            base()
        {
            // turns off caching by default for tests
            // tests which requires caching should enable them manually
            this.CachePolicy = new RequestCachePolicy(
                RequestCacheLevel.NoCacheNoStore);
        }


        public void ClearCookies()
        { _cookieBox = new CookieContainer(); }


        protected override WebRequest GetWebRequest(Uri address)
        {
            var cookies = _cookieBox.GetCookies(address);
            for (var i = 0; i < cookies.Count; i++)
            {
                FuTrace.Debug(cookies[i].Path + " - " + cookies[i].ToString());
            }

            var req = base.GetWebRequest(address);
            _request = (HttpWebRequest)req;
            _request.CookieContainer = _cookieBox;

            return _request;
        }

        protected override WebResponse GetWebResponse(WebRequest request)
        {
            var response = (HttpWebResponse)_request.GetResponse();
            _cookieBox.Add(response.Cookies);

            return response;
        }

        protected override WebResponse GetWebResponse(WebRequest request, IAsyncResult result)
        {
            var response = (HttpWebResponse)_request.EndGetResponse(result);
            _cookieBox.Add(response.Cookies);

            return response;
        }

    }
}
