using System;
using System.IO;

namespace LiveStreamBOT.Helpers
{
    internal class TrackStream : Stream
    {
        readonly Stream _baseStream;
        readonly Action<int> _bufferTransferCallback;
        public TrackStream(Stream baseStream, Action<int> bufferTransferCallback)
        {
            _baseStream = baseStream ?? throw new ArgumentNullException(nameof(baseStream));
            _bufferTransferCallback = bufferTransferCallback ?? throw new ArgumentNullException(nameof(bufferTransferCallback));
        }






        public override bool CanRead => _baseStream.CanRead;

        public override bool CanSeek => _baseStream.CanSeek;

        public override bool CanWrite => _baseStream.CanWrite;

        public override long Length => _baseStream.Length;

        public override long Position { get => _baseStream.Position; set => _baseStream.Position = value; }

        public override void Flush()
        {
            _baseStream.Flush();
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            int result = _baseStream.Read(buffer, offset, count);
            _bufferTransferCallback.Invoke(result);
            return result;
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            return _baseStream.Seek(offset, origin);
        }

        public override void SetLength(long value)
        {
            _baseStream.SetLength(value);
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            _baseStream.Write(buffer, offset, count);
            _bufferTransferCallback.Invoke(count);
        }
    }
}
