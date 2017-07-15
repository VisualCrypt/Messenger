using System;
using System.Globalization;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using Obsidian.Cryptography.Api.Implementations;
using Obsidian.Cryptography.NetStandard;
using Obsidian.Cryptography.TLS;
using Obsidian.MessageNode.Core.Controllers;

namespace Obsidian.MessageNode.Core.Server
{
    sealed class SecureTcpReaderBuffer : TLSEnvelopeReaderBuffer, IDisposable
    {
        public const ushort BufferSize = Config.TcpReaderBufferSize;
        public readonly Socket Connection;
	    private Int32 currentIndex;

		public SecureTcpReaderBuffer(Socket connection)
        {
            Connection = connection;
            Buffer = new byte[Int16.MaxValue];
        }

		public void Dispose()
		{
			try
			{
				this.Connection.Shutdown(SocketShutdown.Send);
			}
			catch (Exception)
			{
				// Throw if client has closed, so it is not necessary to catch.
			}
			finally
			{
				this.Connection.Dispose();
			}
		}

	    public void SetData(SocketAsyncEventArgs socketAsyncEventArgs)
	    {
			int count = socketAsyncEventArgs.BytesTransferred;

		    if ((this.currentIndex + count) > this.Buffer.Length)
		    {
			    throw new ArgumentOutOfRangeException("count",
				    String.Format(CultureInfo.CurrentCulture, "Adding {0} bytes on buffer which has {1} bytes, the listener buffer will overflow.", count, this.currentIndex));
		    }

		    System.Buffer.BlockCopy(socketAsyncEventArgs.Buffer, socketAsyncEventArgs.Offset, this.Buffer, currentIndex,
			    count);
		    //sb.Append(Encoding.ASCII.GetString(args.Buffer, args.Offset, count));
		    this.currentIndex += count;
		}

	    public void ProcessData(SocketAsyncEventArgs args)
	    {
		   var _visualCrypt2Service = new VisualCrypt2Service();
		     _visualCrypt2Service.Init(new Platform_NetStandard(), "testserver");
		    var _serverRequestHandler = new ServerRequestHandler("testserver", _visualCrypt2Service);

		    byte[] usedBuffer = new byte[currentIndex];
			System.Buffer.BlockCopy(Buffer,0,usedBuffer,0,currentIndex);

			var reply =  _serverRequestHandler.ProcessRequestAsync(usedBuffer).Result;

			//// Get the message received from the client.
			//String received = this.sb.ToString();

		 //   //TODO Use message received to perform a specific operation.
		 //   Console.WriteLine("Received: \"{0}\". The server has read {1} bytes.", received, received.Length);

		 //   Byte[] sendBuffer = Encoding.ASCII.GetBytes(received);
		    args.SetBuffer(reply, 0, reply.Length);

		    // Clear StringBuffer, so it can receive more data from a keep-alive connection client.
		    //sb.Length = 0;
		    this.currentIndex = 0;
		}
    }
}
