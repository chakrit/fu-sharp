
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Mime;
using System.Text;

using Fu.Exceptions;
using Fu.Util;
using System.Net;

namespace Fu.Services.Web
{
    public class MultipartParser : IDisposable
    {
        public const int RingBufferSize = 4 * 1024; // 4 KB
        public const int ReadBufferSize = 256; // 256 Bytes

        public const int MaxHeaderLineLength = 1 * 1024; // 1 KB

        public static ContentType DefaultContentType =
            new ContentType(Mime.TextPlain + "; charset=" + Encoding.Default.WebName);

        public static ContentDisposition DefaultDisposition =
            new ContentDisposition("form-data");


        private enum Boundary { NoBoundary, NormalBoundary, LastBoundary, }


        private IDictionary<string, string> _values;
        private IDictionary<string, UploadedFile> _files;

        private Stream _input;
        private Encoding _encoding;
        private byte[] _boundary;

        private ForwardRingBuffer _ring;
        private MemoryStream _readBuffer;
        private byte[] _stringBuffer;

        private int _lastOffset;
        private int _offset;


        public IDictionary<string, string> Values { get { return _values; } }
        public IDictionary<string, UploadedFile> Files { get { return _files; } }

        public MultipartParser(Stream input, Encoding encoding, byte[] boundary)
        {
            _input = input;
            _encoding = encoding;
            _boundary = boundary;

            _files = new Dictionary<string, UploadedFile>();
            _values = new Dictionary<string, string>();

            _lastOffset = _offset = 0;

            // TODO: Make buffer size configurable
            _ring = new ForwardRingBuffer(input, RingBufferSize)
            {
                AutoFillNextBuffer = true
            };

            _readBuffer = new MemoryStream(ReadBufferSize);

            // buffer must be large enough to accomodate two boundary lines
            // so we can detect the entire boundary line inside a single ring buffer
            if (_boundary.Length >= _ring.BufferSize)
            {
                throw new InvalidOperationException(
                    "Boundary is too large, it should be smaller than the current " +
                    "ring buffer's size.");
            }

            // TODO: We should do the same check as above for
            //       header lines... what if the Content-Disposition has a
            //       very very long name field?
        }


        public void Parse()
        {
            // setup parsing variables
            var ignoreCase = StringComparison.OrdinalIgnoreCase;

            string header;
            int nameCounter = 0;
            bool isFile = false;

            ContentDisposition disposition;
            ContentType contentType;
            TransferEncoding encoding;
            string name, filename;


            // begin parsing
            Boundary boundary = readBoundary();
            while (boundary != Boundary.LastBoundary)
            {
                contentType = null;
                disposition = null;
                name = header = null;
                isFile = false;

                while (!string.IsNullOrEmpty(header = readHeaderLine()))
                {
                    if (header.StartsWith("content-type", ignoreCase))
                        contentType = new ContentType(
                            header.Substring("content-type:".Length).Trim());

                    else if (header.StartsWith("content-disposition", ignoreCase))
                    {
                        // HACK: an empty filename field seems to break ContentDisposition
                        //       so we did an empty replace here...
                        //       maybe we should have a proper HeaderParser and did our own
                        //       HTTP sockets implementation after all.... duh!
                        if (header.EndsWith("filename=\"\""))
                        {
                            header = header.Substring(0, header.Length - "filename=\"\"".Length);
                            isFile = true;
                        }

                        header = header.Substring("content-disposition:".Length).Trim();
                        disposition = new ContentDisposition(header);
                    }

                    else if (header.StartsWith("content-transfer-encoding", ignoreCase))
                        encoding = (TransferEncoding)Enum.Parse(typeof(TransferEncoding),
                            header.Substring("content-transfer-encoding".Length).Trim());

                    // TODO: What about content-transfer-encoding?
                    // TODO: What to do with unrecognized header?
                }

                // check wether it's an uploaded file or just a form value
                if (disposition == null) disposition = DefaultDisposition;

                name = disposition.Parameters["name"];
                if (string.IsNullOrEmpty(name))
                    name = "__field" + (nameCounter++).ToString();

                if (!isFile &&
                    string.IsNullOrEmpty(disposition.FileName) &&
                    contentType == null)
                {
                    // we have a normal form value
                    _readBuffer.Seek(0, SeekOrigin.Begin);
                    boundary = readFileContent(_readBuffer);

                    // TODO: int.MaxValue is 2GB do we need to account for
                    //       requests which are too large?
                    //       e.g. MaxUploadedFileSize?
                    var length = (int)(_readBuffer.Position % int.MaxValue);
                    var value = _encoding.GetString(
                        _readBuffer.GetBuffer(), 0, length);

                    // save the value
                    _values[name] = value;
                }
                else
                {
                    // we have an uploaded file
                    // read the request body into a temp file
                    var tempFile = Path.GetTempFileName();
                    var fs = File.OpenWrite(tempFile);

                    boundary = readFileContent(fs);
                    fs.Flush();
                    fs.Close();

                    // save the file
                    _files[name] = new UploadedFile(contentType, disposition, tempFile);
                }
            }
        }


        // assumes that the current ring buffer can hold the entire boundary string + CRLFs
        private Boundary checkBoundary(int startOffset, int endOffset)
        {
            if (endOffset - startOffset < _boundary.Length)
                return Boundary.NoBoundary;

            // boundary starts with -- (double dash)
            if (_ring[startOffset] != 0x2D &&
                _ring[startOffset + 1] != 0x2D)
                return Boundary.NoBoundary;

            // followed by the boundary specified in the content-type multipart header
            for (var i = 0; i < _boundary.Length; i++)
                if (_boundary[i] != _ring[startOffset + i + 2])
                    return Boundary.NoBoundary;

            // the last boundary also ends with -- (double dash)
            if (_ring[endOffset - 2] == 0x2D &&
                _ring[endOffset - 1] == 0x2D)
                return Boundary.LastBoundary;
            else
                return Boundary.NormalBoundary;
        }


        private Boundary readBoundary()
        {
            // read until next CRLF
            _lastOffset = _offset;

            while (!(_ring[_offset] == 0x0D && _ring[_offset + 1] == 0x0A) &&
                (_offset - _lastOffset) <= (_boundary.Length + 4))
                _offset++;

            // verify if its a boundary
            var result = checkBoundary(_lastOffset, _offset);
            if (result == Boundary.NoBoundary)
                throw new BadRequestDataException(string.Format(
                    "Expected boundary at offset {0:X} of request data.", _lastOffset));

            // consume the CRLF
            _offset += 2;

            return result;
        }

        private string readHeaderLine()
        {
            // read until next CRLF
            _lastOffset = _offset;

            while (!(_ring[_offset] == 0x0D && _ring[_offset + 1] == 0x0A) &&
                (_offset - _lastOffset) <= MaxHeaderLineLength)
                _offset++;

            var lineLength = _offset - _lastOffset;

            if (lineLength > MaxHeaderLineLength)
                throw new BadRequestDataException(string.Format(
                    "Header line is too long, maximum is {0} chars.", MaxHeaderLineLength));

            // de-code the header line as string
            var buffer = new byte[lineLength];

            _ring.CopyTo(buffer, _lastOffset, lineLength);
            var result = Encoding.Default.GetString(buffer);
            buffer = null;

            // consume trailing CRLF
            _offset += 2;

            return result;
        }

        private Boundary readFileContent(Stream outputStream)
        {
            _lastOffset = _offset;

            Boundary boundary;
            var lastCrlfOffset = -1;

            // setup ring buffer to flush to outputStream
            EventHandler flushHandler = (sender, e) =>
            {
                // Assume: _offset > _ring.FlushOffset > _lastOffset
                _ring.WriteTo(outputStream, _lastOffset, _ring.FlushOffset - _lastOffset);
                _lastOffset = _ring.FlushOffset;
            };

            _ring.FlushBuffer += flushHandler;

            while (true)
            {
                // read forward until CRLF is encountered
                // if buffer is flushed in the meanwhile the FlushBuffer handler
                // above will flush it out to outputStream automatically
                while (!(_ring[_offset] == 0x0D && _ring[_offset + 1] == 0x0A))
                    _offset++;

                // check for a boundary
                if (lastCrlfOffset != -1)
                {
                    // we've found a second CRLF

                    // interimLength = currentOffset - lastSeenCRLF - "\r\n".Length
                    var interimLength = _offset - lastCrlfOffset - 2;

                    // is it a possible boundary?
                    if (interimLength == _boundary.Length + 2 ||
                        interimLength == _boundary.Length + 4)
                    {
                        // it seems so, runs the checkBounary routine
                        boundary = checkBoundary(lastCrlfOffset + 2, _offset);
                        if (boundary != Boundary.NoBoundary)
                        {
                            // end of request or part
                            _offset += 2; // consume CRLF
                            break;
                        }
                        // else {
                        //   it's still part of the request data, just ignore it.
                        //   so it gets flushed to outputStream }
                    }
                }

                // records the last seen CRLF for next check
                // and just consumes it
                lastCrlfOffset = _offset;
                _offset += 2;
            }

            // flush last chunk to outputStream
            var endOfData = _offset - _boundary.Length;

            // endOfData - (leading CRLF) - (2 dashes) - (trailing CRLF)
            // endOfData - "\r\n".Length - "--".Length - "\r\n".Length;
            endOfData = endOfData - 2 - 2 - 2;

            // if it's the last boundary, we have 2 more dashes at the end to skip
            if (boundary == Boundary.LastBoundary)
                endOfData -= 2;

            // write last chunk to output stream
            _ring.WriteTo(outputStream, _lastOffset, endOfData - _lastOffset);

            // cleanup
            _ring.FlushBuffer -= flushHandler;

            return boundary;
        }


        public void Dispose()
        {
            _readBuffer.Dispose();
            _ring.Dispose();
        }
    }
}
