using System.Threading.Tasks;
using Obsidian.Common;
using Obsidian.Cryptography.NoTLS;

namespace Obsidian.MessageNode.Core.Controllers
{
    public class NoTLSServerRequestHandler : IRequestHandler
    {
	    readonly NOTLSServerRatchet _ratchet;

        public NoTLSServerRequestHandler(string serverId)
        {
			_ratchet = new NOTLSServerRatchet();
        }

        public async Task<byte[]> ProcessRequestAsync(byte[] rawRequest)
        {
            if (rawRequest == null || rawRequest.Length == 0)
                return null;

            var tlsEnvelope = new NOTLSEnvelope(rawRequest);
	        var request = _ratchet.DecryptRequest(tlsEnvelope);

           

            Command command = request.ParseCommand();

            if (!command.CommandID.IsCommandDefined())
                return null;

			var cp = new CommandProcessor();
            byte[] response = await cp.ExecuteAuthenticatedRequestAsync(command);
			
            return response == null ?
                null :
                _ratchet.EncryptRequest(response).Serialize();
        }
     
    }
}
