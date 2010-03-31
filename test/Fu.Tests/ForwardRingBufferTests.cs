
using System.IO;
using System.Security.Cryptography;

using MbUnit.Framework;

using Fu.Util;

namespace Fu.Tests
{
  [TestFixture]
  public class ForwardRingBufferTests
  {
    public const int RingBufferCapacity = 16;
    public const int TestBufferCycles = 7;
    public const int TestDataSize = RingBufferCapacity * TestBufferCycles;

    [Test]
    public void SimpleForwardReadTest()
    {
      var buffer = getTestBuffer();

      var ms = new MemoryStream(buffer);
      var ring = getRing(ms);

      for (var i = 0; i < TestDataSize; i++) {
        if (i > ring.MaxOffset)
          ring.FillNextBuffer();

        Assert.AreEqual(buffer[i], ring[i]);
      }
    }

    [Test]
    public void ForwardReadWithAutofillTest()
    {
      var buffer = getTestBuffer();

      var ms = new MemoryStream(buffer);
      var ring = getRing(ms);

      ring.AutoFillNextBuffer = true;
      for (var i = 0; i < TestDataSize; i++)
        Assert.AreEqual(buffer[i], ring[i]);
    }

    [Test]
    public void BackwardReadTest()
    {
      var buffer = getTestBuffer();

      var ms = new MemoryStream(buffer);
      var ring = getRing(ms);

      for (var i = 0; i < TestDataSize; i += RingBufferCapacity) {
        if (i > ring.MaxOffset)
          ring.FillNextBuffer();

        for (var j = i; j >= ring.MinOffset; j--)
          Assert.AreEqual(buffer[j], ring[j]);
      }
    }

    [Test]
    public void BackwardReadWithAutofillTest()
    {
      var buffer = getTestBuffer();

      var ms = new MemoryStream(buffer);
      var ring = getRing(ms);

      ring.AutoFillNextBuffer = true;
      for (var i = 0; i < TestDataSize; i += RingBufferCapacity)
        for (var j = i; j >= ring.MinOffset; j--)
          Assert.AreEqual(buffer[j], ring[j]);
    }


    private ForwardRingBuffer getRing(Stream input)
    {
      return new ForwardRingBuffer(input, RingBufferCapacity);
    }

    private byte[] getTestBuffer()
    {
      var rng = new RNGCryptoServiceProvider();
      var buffer = new byte[TestDataSize];

      rng.GetBytes(buffer);
      return buffer;
    }
  }
}
