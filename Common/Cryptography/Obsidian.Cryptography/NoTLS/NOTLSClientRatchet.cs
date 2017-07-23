using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Obsidian.Cryptography.Api.DataTypes;
using Obsidian.Cryptography.Api.Implementations;
using Obsidian.Cryptography.Api.Infrastructure;
using Obsidian.Cryptography.Api.Interfaces;
using Obsidian.Cryptography.E2E;
using Obsidian.Cryptography.ECC;

namespace Obsidian.Cryptography.NoTLS
{
	public class NOTLSClientRatchet
	{
		public async Task<NOTLSEnvelope> EncryptRequest(byte[] clearPacket)
		{
			Guard.NotNull(clearPacket);
			return new NOTLSEnvelope(clearPacket, clearPacket.Length);
		}

		public async Task<NOTLSRequest> DecryptRequest(IEnvelope tlsEnvelope)
		{
			Guard.NotNull(tlsEnvelope);
			var ar = new NOTLSRequest
			{
				CommandData = tlsEnvelope.EncipheredPayload,
				IsAuthenticated = false,
				UserId = "Anonym. N."
			};
			return ar;
		}

	

		
	}
}