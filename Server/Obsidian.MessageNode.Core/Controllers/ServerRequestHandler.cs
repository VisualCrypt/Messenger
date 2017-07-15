using System.Threading.Tasks;
using Obsidian.Common;
using Obsidian.Cryptography.Api.Interfaces;
using Obsidian.Cryptography.TLS;
using Obsidian.MessageNode.Core.Data;
using Obsidian.MessageNode.Core.Server;

namespace Obsidian.MessageNode.Core.Controllers
{
    public class ServerRequestHandler : IRequestHandler
    {
        readonly CommandProcessor _commandProcessor;
        readonly TLSServerRatchet _ratchet;

        public ServerRequestHandler(string serverId, IVisualCrypt2Service visualCrypt2Service)
        {
            _commandProcessor = new CommandProcessor();
            _ratchet = new TLSServerRatchet(serverId, ServerKeyRepository.ServerPrivateKey, visualCrypt2Service);
        }

        public async Task<byte[]> ProcessRequestAsync(byte[] rawRequest)
        {
            if (rawRequest == null || rawRequest.Length == 0)
                return null;

            var tlsEnvelope = new TLSEnvelope(rawRequest);
            var request = _ratchet.TLSServerDecryptRequest(tlsEnvelope);

            if (request == null)
            {
                Log.Warn($"ServerRequestHandler: Unknown DynamicPublicKey {tlsEnvelope.DynamicPublicKeyId}, sending {CommandID.LostDynamicKey_Response}.");
                // request == null means we lost the dynamic private key to to the public key the client used.
                // We don't know who the client is, because TLS could not decrypt the message at all.
                // We now need to tell the client it needs to reset its key schedule, i.e. start over
                // using my static public key.
                // We must use our static private key to encrypt this message, so that the client knows the server is talking.
                // We then use the dynamic public key we just received, to calculate a new shared secret that only the unknown
                // client can reproduce for decryption of this message.
                byte[] lostDynamicKeyResponseCommand = _commandProcessor.ExecuteLostDynamicKey();
                byte[] lostDynamicKeyResponse = _ratchet.TLSServerEncryptRequestAnonymous(lostDynamicKeyResponseCommand, tlsEnvelope.DynamicPublicKey, tlsEnvelope.DynamicPublicKeyId).Serialize();
                return lostDynamicKeyResponse;
            }

            Command command = request.ParseCommand();

            if (!command.CommandID.IsCommandDefined())
                return null;

            byte[] response;
            if (CommandProtocol.IsAuthenticationRequired(command.CommandID))
            {
                if (!request.IsAuthenticated)
                {
                    Log.Warn($"ServerRequestHandler: Command {command.CommandID} from {request.UserId} requires Auth, which failed. Sending {CommandID.NoSuchUser_Response}");
                    // We may be here, when the server has lost the identity (and client public key for the calling user id), or if the 
                    // user id is locked, revoked etc.
                    byte[] noSuchUserResponseCommand = _commandProcessor.ExecuteNoSuchUser();
                    byte[] nosuchUserResponse = _ratchet.TLSServerEncryptRequestAnonymous(noSuchUserResponseCommand, tlsEnvelope.DynamicPublicKey, tlsEnvelope.DynamicPublicKeyId).Serialize();
                    return nosuchUserResponse;
                }

                response = await _commandProcessor.ExecuteAuthenticatedRequestAsync(command);
            }
            else
            {
                response = await _commandProcessor.ExecutePublishIdentityAsync(command, _ratchet.RefreshTLSUser);
                // when PublishIdentity has returned succcessfully, we can save the DynamicPublicKey we did not save in TLSDecrpt, because we wanted more logic to run before this
                // Specifically, the IdentityController has called RefreshTLSUser by now, creating the place where this key can be stored.
                if (response != null)
                    _ratchet.SaveIncomingDynamicPublicKey(request.UserId, tlsEnvelope.DynamicPublicKey, tlsEnvelope.DynamicPublicKeyId);
            }
            return response == null ?
                null :
                _ratchet.TLSServerEncryptRequest(response, request.UserId).Serialize();
        }
     
    }
}
