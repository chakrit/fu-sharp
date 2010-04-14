
using System;
using System.Net;
using System.Text;

namespace Fu.Services.Sessions
{
  public abstract class CookieSessionIdProvider : ISessionIdProvider
  {
    public const string DefaultCookieName = "s";
    public const int DefaultKeyLength = 128;


    public string CookieName { get; private set; }
    public int KeyLength { get; private set; }

    public CookieSessionIdProvider() :
      this(DefaultKeyLength, DefaultCookieName) { }

    public CookieSessionIdProvider(int keyLength) :
      this(keyLength, DefaultCookieName) { }

    public CookieSessionIdProvider(string cookieName) :
      this(DefaultKeyLength, cookieName) { }

    public CookieSessionIdProvider(int keyLength, string cookieName)
    {
      KeyLength = keyLength;
      CookieName = cookieName;
    }


    protected abstract string CreateKeyCore(IFuContext c);

    public string CreateId(IFuContext c)
    {
      // create a new session key
      var sessionId = CreateKeyCore(c);
      var settings = c.Settings.Session;

      // save the session into the response cookie
      var cookie = new Cookie(CookieName, sessionId,
        settings.CookiePath,
        settings.CookieDomain);

      cookie.HttpOnly = true;
      cookie.Expires = DateTime.Now.AddMonths(1);

      saveCookie(c, cookie);
      return sessionId;
    }


    public string GetId(IFuContext c)
    {
      var cookie = c.Response.Cookies[CookieName] ??
        c.Request.Cookies[CookieName];

      if (cookie == null || string.IsNullOrEmpty(cookie.Value))
        return null;

      return cookie.Value;
    }

    public void DeleteId(IFuContext c)
    {
      var cookie = c.Request.Cookies[CookieName];

      if (cookie == null || string.IsNullOrEmpty(cookie.Value))
        return;

      cookie.Expires = DateTime.Now.AddMonths(-1);

      saveCookie(c, cookie);
    }


    private void saveCookie(IFuContext c, Cookie cookie)
    {
      // NOTE: Manual cookie string building is required to allow Fu to
      //       stay with "client" framework subset because the
      //       System.Net.Cookie class doesn't actually use all the http fields
      var cookieString = new StringBuilder();
      cookieString.AppendFormat("{0}={1}; ", cookie.Name, cookie.Value);

      if (!cookie.Expired)
        cookieString.AppendFormat("expires={0}; ",
          cookie.Expires.ToUniversalTime().ToString("R"));

      if (!string.IsNullOrEmpty(cookie.Path))
        cookieString.AppendFormat("path={0}; ", cookie.Path);
      if (!string.IsNullOrEmpty(cookie.Domain))
        cookieString.AppendFormat("domain={0}; ", cookie.Domain);

      c.Response.Headers.Add(HttpResponseHeader.SetCookie,
        cookieString.ToString());
    }
  }
}
