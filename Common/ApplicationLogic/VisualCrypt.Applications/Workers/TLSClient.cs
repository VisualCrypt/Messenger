using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using VisualCrypt.Applications.Services.Interfaces;
using VisualCrypt.Cryptography.VisualCrypt2.Interfaces;
using VisualCrypt.Common;
using VisualCrypt.Cryptography.TLS;
using VisualCrypt.Cryptography.VisualCrypt2.Infrastructure;

namespace VisualCrypt.Applications.Workers
{
    public class TLSClient : INetworkClient
    {
        readonly Container _container;
        readonly IChatClient _chatClient;
        readonly ITcpConnection _tcp;
        readonly IUdpConnection _udp;
        readonly ILog _log;
        readonly AppState _appState;
        TLSClientRatchet _r;

        public TLSClient(Container container)
        {
            _container = container;
            _chatClient = container.Get<IChatClient>();
            _log = container.Get<ILog>();
            _tcp = container.Get<ITcpConnection>();
            _udp = container.Get<IUdpConnection>();
            _appState = container.Get<AppState>();
        }

        // public for tests only
        public TLSClientRatchet Ratchet
        {
            get
            {
                if (_r != null)
                    return _r;
                _r = new TLSClientRatchet(_chatClient.MyId,
                    _chatClient.MyPrivateKey,
                    new TLSUser(HardCoded.Server0001, HardCoded.Server0001StaticPublicKey),
                _container.Get<IVisualCrypt2Service>());
                return _r;
            }
        }

        public async Task<string> Receive(byte[] rawRequest, Transport transport)
        {
            Debug.Assert(transport == Transport.UDP);

            try
            {
                if (rawRequest == null || rawRequest.Length == 0)
                    throw new Exception("TLSClient received null or empty packet");

                var tlsEnvelope = new TLSEnvelope(rawRequest);

                int actualCrc32;
                if (!rawRequest.ValidateCrc32(out actualCrc32))
                    throw new Exception($"TLSEnvelope CRC32 Error: Expected: {tlsEnvelope.Crc32}, actual: {actualCrc32}");

                var request = await Ratchet.DecryptRequest(tlsEnvelope);


                var command = request.ParseCommand();
                if (!command.CommandID.IsCommandDefined())
                    throw new Exception($"TLSClient: The command {command.CommandID} is not defined.");

                await ProcessCommand(command);
            }
            catch (Exception e)
            {
                var error = $"{nameof(TLSClient)} received bad request via {transport}: {e.Message}";
                _log.Debug(error);
            }
            return null; // this goes back to the IUdpConnection and is not used there.
        }

        async Task ProcessCommand(Command command)
        {
            switch (command.CommandID)
            {
                case CommandID.AnyNews_Response:
                    _chatClient.ReceiveAnyNewsResponse(command.CommandData.DeserializeByteCore());
                    return;
                case CommandID.NoSuchUser_Response:
                    _appState.SetIsIdentityPublished(false);
                    return;
                case CommandID.LostDynamicKey_Response:
                    await Ratchet.Reset();
                    return;
            }
        }

        public async Task<Response<List<TLSRequest>>> SendRequestAsync(byte[] request, Transport transport)
        {
            var response = new Response<List<TLSRequest>>();
            try
            {
                var tlsRequestEnvelope = await Ratchet.EncryptRequest(request);
                var requestBytes = tlsRequestEnvelope.Serialize();
                Response<List<TLSEnvelope>> networkResponse;
                if (transport == Transport.TCP)
                    networkResponse = await _tcp.SendRequestAsync(requestBytes).ConfigureAwait(false); // http://stackoverflow.com/a/19727627
                else
                {
                    networkResponse = await _udp.SendRequestAsync(requestBytes);
                }
                if (networkResponse.IsSuccess)
                {
                    if (transport == Transport.UDP && networkResponse.Result == null)
                    {
                        response.SetSuccess();
                        return response;
                    }

                    var authenticatedResponses = new List<TLSRequest>();
                    foreach (TLSEnvelope tlsEnvelope in networkResponse.Result)
                    {
                        TLSRequest tlsRequest = await Ratchet.DecryptRequest(tlsEnvelope);

                        if (!tlsRequest.IsAuthenticated)
                            throw new Exception("Authentication failed.");
                        _log.Debug($"{tlsRequest.UserId} is authenticated.");

                        Command command = tlsRequest.ParseCommand();
                        if (!command.CommandID.IsCommandDefined())
                            throw new Exception($"Invalid Command {command.CommandID}");

                        if (command.CommandID == CommandID.LostDynamicKey_Response)
                        {
                            await Ratchet.Reset();
                        }
                        else if (command.CommandID == CommandID.NoSuchUser_Response)
                        {
                            _appState.SetIsIdentityPublished(false);
                        }
                        tlsRequest.CommandData = command.CommandData; // minus the header
                        authenticatedResponses.Add(tlsRequest);
                    }


                    response.Result = authenticatedResponses;
                    response.SetSuccess();
                }
                else response.SetError(networkResponse.Error);

            }
            catch (Exception e)
            {
                response.SetError(e);
                _log.Exception(e);
            }
            return response;

        }


    }
}
