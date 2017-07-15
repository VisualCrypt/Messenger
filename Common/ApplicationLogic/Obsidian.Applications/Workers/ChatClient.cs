using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Obsidian.Applications.Models.Chat;
using Obsidian.Applications.Services.Interfaces;
using Obsidian.Common;
using Obsidian.Cryptography.Api.Infrastructure;
using Obsidian.Cryptography.TLS;

namespace Obsidian.Applications.Workers
{
    public class ChatClient : IChatClient
    {
        readonly ILog _log;
        readonly Container _container;

        INetworkClient _networkClient;
        ChatWorker _chatWorker;
       
        public ChatClient(Container container)
        {
            _log = container.Get<ILog>();
            _container = container;
        }

        public void Init(string myId, byte[] myPrivateKey, ChatWorker chatWorker)
        {
            MyId = myId;
            MyPrivateKey = myPrivateKey;
            _chatWorker = chatWorker;
            _networkClient = _container.Get<INetworkClient>();
        }

        public string MyId { get; private set; }
        public byte[] MyPrivateKey { get; private set; }

        public async Task<Response<IReadOnlyCollection<XMessage>>> DownloadMessages(string myId)
        {
            var response = new Response<IReadOnlyCollection<XMessage>>();
            try
            {
                var requestCommand = new RequestCommand(CommandID.DownloadMessage, myId).Serialize(CommandHeader.Yes);
                var tlsResponse = await _networkClient.SendRequestAsync(requestCommand, Transport.TCP);
                if (tlsResponse.IsSuccess)
                {
                    List<TLSRequest> resp = tlsResponse.Result;

                    List<XMessage> messages = new List<XMessage>();
                    foreach (var authenticableRequest in resp)
                    {
                        List<XMessage> mess = authenticableRequest.CommandData.DeserializeCollection(XMessageExtensions.DeserializeMessage);
                        messages.AddRange(mess);
                    }
                    response.Result = messages;

                    response.SetSuccess();
                }
                else response.SetError(tlsResponse.Error);
            }
            catch (Exception e)
            {
                _log.Exception(e);
                response.SetError(e);
            }
            return response;
        }

        public async Task<Response>  AnyNews(string myId)
        {
            var response = new Response();
            try
            {
                var requestCommand = new RequestCommand(CommandID.AnyNews, myId).Serialize(CommandHeader.Yes);
                var tlsResponse = await _networkClient.SendRequestAsync(requestCommand, Transport.UDP);
                if (tlsResponse.IsSuccess)
                {
                    response.SetSuccess();
                } else response.SetError(tlsResponse.Error);
            }
            catch (Exception e)
            {
                _log.Exception(e);
                response.SetError(e);
            }
            return response;
        }


        public async Task<Response<string>> UploadMessage(Message message)
        {
            var response = new Response<string>();
            try
            {
                var requestCommand = new RequestCommand(CommandID.UploadMessage, message).Serialize(CommandHeader.Yes);
                var tlsResponse = await _networkClient.SendRequestAsync(requestCommand, Transport.TCP);
                if (tlsResponse.IsSuccess)
                {
                    response.Result = tlsResponse.Result.Single().CommandData.DeserializeStringCore();
                    response.SetSuccess();
                }
                else response.SetError(tlsResponse.Error);
            }
            catch (Exception e)
            {
                _log.Exception(e);
                response.SetError(e);
            }
            return response;
        }

        public async Task<Response<string>> PublishIdentityAsync(XIdentity identity)
        {
            var response = new Response<string>();
            try
            {
                var requestCommand = new RequestCommand(CommandID.PublishIdentity, identity).Serialize(CommandHeader.Yes);
                var tlsResponse = await _networkClient.SendRequestAsync(requestCommand, Transport.TCP);
                if (tlsResponse.IsSuccess)
                {
                    response.Result = tlsResponse.Result.Single().CommandData.DeserializeStringCore();
                    response.SetSuccess();
                }
                else response.SetError(tlsResponse.Error);
            }
            catch (Exception e)
            {
                _log.Exception(e);
                response.SetError(e);
            }
            return response;
        }

        public async Task<Response<XIdentity>> GetIdentityAsync(string contactId)
        {
            var response = new Response<XIdentity>();
            try
            {
                var requestCommand = new RequestCommand(CommandID.GetIdentity, contactId).Serialize(CommandHeader.Yes);
                var tlsResponse = await _networkClient.SendRequestAsync(requestCommand, Transport.TCP);
                if (tlsResponse.IsSuccess)
                {
                    response.Result = tlsResponse.Result.Single().CommandData.DeserializeXIdentityCore();
                    response.SetSuccess();
                } else response.SetError(tlsResponse.Error);
            }
            catch (Exception e)
            {
                _log.Exception(e);
                response.SetError(e);
            }
            return response;
        }

       

        public void ReceiveAnyNewsResponse(byte isAnyNews)
        {
             _chatWorker.ReceiveCheckForMessagesReply(isAnyNews);
        }
    }
}
