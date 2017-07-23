﻿using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Obsidian.Cryptography.NoTLS
{
	public static class NOTLSEnvelopeReader
	{
		public static async Task<List<NOTLSEnvelope>> ReceivePackets(NOTLSEnvelopeReaderBuffer readerBuffer, Stream stream, CancellationToken ct)
		{
			var receivedPackets = new List<NOTLSEnvelope>();

			int currentBytesRead;
			NOTLSEnvelope packet;
			do
			{
				currentBytesRead = await stream.ReadAsync(readerBuffer.Buffer, 0, readerBuffer.Buffer.Length, ct);
				NOTLSEnvelopeExtensions.UpdatePayload(currentBytesRead, readerBuffer);
				packet = NOTLSEnvelopeExtensions.TryTakeOnePacket(ref readerBuffer.Payload);
				if (packet != null)
					receivedPackets.Add(packet);

			} while (currentBytesRead > 0 && (packet == null || readerBuffer.Payload != null));
			return receivedPackets;
		}
	}
}