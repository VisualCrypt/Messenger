using System;
using System.IO;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading;
using System.Threading.Tasks;
using Windows.Networking.Sockets;
using Windows.Storage.Streams;

namespace Obsidian.UWP.Core.Network
{
    public class SocketStream : Stream
    {
        readonly StreamSocket _streamSocket;
        public SocketStream(StreamSocket streamSocket)
        {
            _streamSocket = streamSocket;
        }

        public override async Task<int> ReadAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
        {
            var iBuffer = WindowsRuntimeBuffer.Create(count);
            var resultBuffer = await _streamSocket.InputStream.ReadAsync(iBuffer, (uint) count, InputStreamOptions.Partial);
            if (resultBuffer == null || resultBuffer.Capacity == 0)
                return 0;
            var resultBufferBytes = resultBuffer.ToArray();
            resultBufferBytes.CopyTo(buffer, 0);
            return (int) resultBuffer.Length;
        }

        public override void Flush()
        {
            throw new NotImplementedException();
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            throw new NotImplementedException();
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            throw new NotImplementedException();
        }

        public override void SetLength(long value)
        {
            throw new NotImplementedException();
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            throw new NotImplementedException();
        }

        public override bool CanRead { get; }
        public override bool CanSeek { get; }
        public override bool CanWrite { get; }
        public override long Length { get; }
        public override long Position { get; set; }
    }
}