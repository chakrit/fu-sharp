
using System;
using System.IO;

namespace Fu.Util
{
  public class ForwardRingBuffer : IDisposable
  {
    public const int DefaultBufferSize = 10 * 1024; // 10 KB default size

    private Stream _input;

    private byte[] _forwardBuffer;
    private byte[] _backBuffer;
    private byte[] _buffer;

    private bool _endOfStream;

    private int _bytesRead;
    private int _offset;
    private int _maxOffset;
    private int _minOffset;


    public event EventHandler FlushBuffer;

    public bool EndOfStream { get { return _endOfStream; } }
    public bool AutoFillNextBuffer { get; set; }

    public int BufferSize { get { return _buffer.Length; } }

    public int FlushOffset { get { return _offset + BufferSize; } }
    public int MaxOffset { get { return _maxOffset; } }
    public int MinOffset { get { return _minOffset; } }

    public byte this[int addr]
    {
      get
      {
        ensureOffset(addr);

        return (addr < _offset) ?
          _backBuffer[_backBuffer.Length - _offset + addr] :
          _buffer[addr - _offset];
      }
    }


    public ForwardRingBuffer(Stream input) :
      this(input, DefaultBufferSize) { }

    public ForwardRingBuffer(Stream input, int bufferSize)
    {
      AutoFillNextBuffer = false;

      _input = input;

      _buffer = _forwardBuffer = new byte[bufferSize];
      _backBuffer = new byte[bufferSize];

      _bytesRead = _offset = 0;
      _endOfStream = false;

      fillFirstBuffer();
    }

    private void fillFirstBuffer()
    {
      _offset = 0;
      _bytesRead = _input.Read(_buffer, 0, _buffer.Length);

      _minOffset = 0;
      _maxOffset = _bytesRead - 1;
    }


    public void WriteTo(Stream s, int ringOffset, int count)
    {
      ensureOffset(ringOffset);

      // needs -1 here so if AutoFillNextBuffer is on
      // ensureOffset would not goes off filling another buffer and causing
      // another flush and another potential CopyTo/WriteTo from the user
      ensureOffset(ringOffset + count - 1);

      if (ringOffset < _offset) {
        var backCount = _offset - ringOffset;
        s.Write(_backBuffer, _backBuffer.Length - backCount, backCount);

        ringOffset = _offset;
        count -= backCount;
      }

      s.Write(_buffer, ringOffset - _offset, count);
    }

    public void CopyTo(byte[] buffer, int ringOffset, int count)
    { CopyTo(buffer, 0, ringOffset, count); }

    public void CopyTo(byte[] buffer, int targetOffset, int ringOffset, int count)
    {
      ensureOffset(ringOffset);

      // needs -1 here so if AutoFillNextBuffer is on
      // ensureOffset would not goes off filling another buffer and causing
      // another flush and another potential CopyTo/WriteTo from the user
      ensureOffset(ringOffset + count - 1);

      if (ringOffset < _offset) {
        var backCount = _offset - ringOffset;
        Buffer.BlockCopy(
            _backBuffer, _backBuffer.Length - backCount,
            buffer, targetOffset,
            backCount);

        ringOffset = _offset;
        targetOffset += backCount;
        count -= backCount;
      }

      Buffer.BlockCopy(_buffer, ringOffset - _offset,
          buffer, targetOffset,
          count);
    }


    public void FillNextBuffer()
    {
      if (_endOfStream)
        throw new IOException("Cannot read past the end of the stream.");

      // fill the forward buffer first
      if (_bytesRead < _buffer.Length) {

        var count = _buffer.Length - _bytesRead;
        var chunkRead = _input.Read(_buffer, _bytesRead, count);
        if (chunkRead == 0) {
          _endOfStream = true;
          return;
        }

        _bytesRead += chunkRead;
        _maxOffset = _offset + _bytesRead - 1;
        return;
      }

      // if the forward buffer is filled, we flush the back buffer
      if (FlushBuffer != null)
        FlushBuffer(this, EventArgs.Empty);

      // and cycle the data in forward buffer to the back buffer
      _forwardBuffer = _backBuffer;
      _backBuffer = _buffer;
      _buffer = _forwardBuffer;

      // read new data into forward buffer
      _bytesRead = _input.Read(_buffer, 0, _buffer.Length);
      if (_bytesRead == 0) {
        _endOfStream = true;
        return;
      }

      _minOffset = _offset;
      _offset += _buffer.Length;
      _maxOffset = _offset + _bytesRead - 1;
    }


    private void ensureOffset(int offset)
    {
      if (offset < _minOffset)
        throw new IOException(
          "Cannot read back past the minimum offset of the ring buffer.");

      if (offset > _maxOffset) {
        if (!AutoFillNextBuffer)
          throw new IOException(
            "Cannot read beyond the maximumn offset of the current ring buffer. " +
            "You should call FillNextBuffer() first.");

        // AutoFillNextBuffer == true
        while (offset > _maxOffset)
          FillNextBuffer();

        // re-run all the tests after the buffer has been filled enough
        ensureOffset(offset);
      }
    }


    public void Dispose()
    {
      _forwardBuffer = null;
      _backBuffer = null;
      _buffer = null;
      _input = null;
    }
  }
}
