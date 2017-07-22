using System.Net.Sockets;
using Obsidian.Cryptography.TLS;

namespace Obsidian.MessageNode.Core.Server.CP
{
	sealed class SecureTcpReaderBuffer : TLSEnvelopeReaderBuffer
	{
		public const ushort BufferSize = Config.TcpReaderBufferSize;
		public  Socket Client;

		public SecureTcpReaderBuffer(Socket client)
		{
			Client = client;
			Buffer = new byte[BufferSize];
		}
	}
}
