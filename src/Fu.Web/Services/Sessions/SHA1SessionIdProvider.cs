
using System;
using System.Security.Cryptography;

namespace Fu.Services.Sessions
{
  public class SHA1SessionIdProvider : CookieSessionIdProvider
  {
    private RNGCryptoServiceProvider _rng;
    private SHA1CryptoServiceProvider _sha1;


    public SHA1SessionIdProvider() :
      base() { init(); }

    public SHA1SessionIdProvider(int keyLength) :
      base(keyLength) { init(); }

    public SHA1SessionIdProvider(string cookieName) :
      base(cookieName) { init(); }

    public SHA1SessionIdProvider(int keyLength, string cookieName) :
      base(keyLength, cookieName) { init(); }

    private void init()
    {
      _rng = new RNGCryptoServiceProvider();
      _sha1 = new SHA1CryptoServiceProvider();
    }


    protected override string CreateKeyCore(IFuContext c)
    {
      var buffer = new byte[KeyLength];
      _rng.GetBytes(buffer);

      buffer = _sha1.ComputeHash(buffer);
      return Convert.ToBase64String(buffer);
    }
  }
}
