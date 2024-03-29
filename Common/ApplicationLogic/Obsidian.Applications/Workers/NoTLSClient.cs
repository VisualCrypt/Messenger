﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using Obsidian.Applications.Services.Interfaces;
using Obsidian.Common;
using Obsidian.Cryptography.Api.Infrastructure;
using Obsidian.Cryptography.NoTLS;
using Obsidian.Cryptography.TLS;

namespace Obsidian.Applications.Workers
{
    public class NoTLSClient : INetworkClient
    {
        readonly Container _container;
        readonly IChatClient _chatClient;
        readonly ITcpConnection _tcp;
        readonly IUdpConnection _udp;
        readonly ILog _log;
        readonly AppState _appState;
        NOTLSClientRatchet _r;

        public NoTLSClient(Container container)
        {
            _container = container;
            _chatClient = container.Get<IChatClient>();
            _log = container.Get<ILog>();
            _tcp = container.Get<ITcpConnection>();
            _udp = container.Get<IUdpConnection>();
            _appState = container.Get<AppState>();
        }

        // public for tests only
        public NOTLSClientRatchet Ratchet
        {
            get
            {
                if (_r != null)
                    return _r;
                _r = new NOTLSClientRatchet();
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

                var tlsEnvelope = new NOTLSEnvelope(rawRequest);

                int actualCrc32;
                if (!NOTLSEnvelopeExtensions.ValidateCrc32(rawRequest, out actualCrc32))
                    throw new Exception($"TLSEnvelope CRC32 Error: Expected: {tlsEnvelope.Crc32}, actual: {actualCrc32}");

                var request = await Ratchet.DecryptRequest(tlsEnvelope);


                var command = request.ParseCommand();
	          
				if (!command.CommandID.IsCommandDefined())
                    throw new Exception($"TLSClient: The command {command.CommandID} is not defined.");

                await ProcessCommand(command);
            }
            catch (Exception e)
            {
                var error = $"{nameof(NoTLSClient)} received bad request via {transport}: {e.Message}";
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
					_log.Debug("LostDynamicKey_Response is not applicable in NoTLS mode.");
                    return;
            }
        }

        public async Task<Response<List<IRequestCommandData>>> SendRequestAsync(byte[] request, Transport transport)
        {
            var response = new Response<List<IRequestCommandData>>();
            try
            {
                var tlsRequestEnvelope = await Ratchet.EncryptRequest(request);
                var requestBytes = tlsRequestEnvelope.Serialize();
                Response<List<IEnvelope>> networkResponse;
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

                    var authenticatedResponses = new List<IRequestCommandData>();
                    foreach (IEnvelope tlsEnvelope in networkResponse.Result)
                    {
                        NOTLSRequest tlsRequest = await Ratchet.DecryptRequest(tlsEnvelope);

                       Debug.Assert(tlsRequest.IsAuthenticated == false);
                     

                        Command command = tlsRequest.ParseCommand();
						
                        if (!command.CommandID.IsCommandDefined())
                            throw new Exception($"Invalid Command {command.CommandID}");

                       
                        if (command.CommandID == CommandID.NoSuchUser_Response)
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
