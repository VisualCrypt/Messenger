using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using VisualCrypt.Applications.Data;
using VisualCrypt.Applications.Models.Chat;
using VisualCrypt.Applications.Services.Interfaces;
using VisualCrypt.Applications.Services.PortableImplementations;
using VisualCrypt.Applications.ViewModels.Chat;
using VisualCrypt.Common;
using VisualCrypt.Cryptography.E2E;
using VisualCrypt.Cryptography.VisualCrypt2.DataTypes;
using VisualCrypt.Cryptography.VisualCrypt2.Interfaces;

namespace VisualCrypt.Applications.Workers
{
    public class ChatWorker
    {
        public event EventHandler<Message> OnIncomingMessagePersisted;
        public event EventHandler ContactUpdateReceived;

        readonly Container _container;
        readonly AppState _appState;
        readonly AppRepository _repo;
        readonly ILog _log;
        readonly IChatClient _chatClient;
        readonly IUdpConnection _udp;
        readonly ITcpConnection _tcp;
        readonly INetworkClient _networkClient;

        AbstractSettingsManager _settingsManager;
        string _myId;
        bool _isInitialized;
        bool _isRunning;
        int _interval;
        bool _readToReceive;

        public ChatWorker(Container container)
        {
            _container = container;
            _appState = container.Get<AppState>();
            _repo = container.Get<AppRepository>();
            _log = container.Get<ILog>();
            _tcp = container.Get<ITcpConnection>();
            _udp = container.Get<IUdpConnection>();
            _chatClient = _container.Get<IChatClient>();
            _networkClient = _container.Get<INetworkClient>();

        }

        #region Start, Stop, Run


        public void Init(string myId, byte[] myPrivateKey)
        {
            if (_isInitialized)
                return;
            _isInitialized = true;
            _settingsManager = _container.Get<AbstractSettingsManager>();
            //_settingsManager.ChatSettings.RemoteDnsHostAddress = "192.168.178.23";
            _settingsManager.ChatSettings.RemoteDnsHostAddress = "visualcryptservice.cloudapp.net";

            _interval = _settingsManager.ChatSettings.Interval;
            _myId = myId;
            _chatClient.Init(myId, myPrivateKey, this);

        }

        public async void StartRunning()
        {
            if(_isRunning)
                return;
            _isRunning = true;
            await Task.Run(() => RunUntilStop());
        }

        public async void StopRunLoopAndDisconnectAll()
        {
            _isRunning = false;
            await TcpDisconnect();
            await UdpDisconnect();
        }




        async void RunUntilStop()
        {
            while (_isRunning)
            {
                try
                {
                    await RunWithState();
                }
                catch (Exception e)
                {
                    _log.Exception(e);
                }
                await Task.Delay(_interval);
            }
        }

        async Task RunWithState()
        {
            if (!_appState.IsUdpConnected)
            {
                try
                {
                    await UdpConnectAsync();
                    _appState.SetUdpIsConnected(true);
                }
                catch (Exception e)
                {
                    _appState.SetUdpIsConnected(false);
                    _log.Exception(e);
                }

            }

            if (!_tcp.IsConnected)
            {
                if (!await TcpConnectAsync())
                    return;
            }

            if (!_appState.IsIdentityPublished)
            {
                var profile = await _repo.GetProfile(_myId);
                profile.IsIdentityPublished = false;
                await _repo.UpdateProfile(profile); // sync this, TODO
                bool isIdentityPublished = await PublishIdentity();
                _appState.SetIsIdentityPublished(isIdentityPublished);

                if (!isIdentityPublished)
                    return;
            }

            if (!_appState.HasValidContacts)
            {
                bool hasValidContacts = await CheckContacts();
                _appState.SetHasValidContacts(hasValidContacts);
                if (!hasValidContacts)
                    return;
            }
            _readToReceive = true;

            await SendReadReceipts();

            if (!_appState.IsMessagesWaiting)
            {
                var anyNewsResponse = await _chatClient.AnyNews(_myId);
                if (!anyNewsResponse.IsSuccess)
                {
                    // if we are here, the datagram socket may have become null, which happenes after a sleep of a couple of minutes.
                    await _udp.DisconnectAsync(); // 'disconnect' explicitly
                    _appState.IsUdpConnected = false;
                }
            }
            else
            {
                if (_readToReceive)
                {
                    _readToReceive = false;

                    var getMessagePartsResponse = await _chatClient.DownloadMessages(_myId);

                    if (getMessagePartsResponse.IsSuccess)
                    {
                        var receivedMessages = getMessagePartsResponse.Result;

                        var deliveryReceipts = receivedMessages.Where(x => x.MessageType == MessageType.DeliveryReceipt);
                        foreach (var item in deliveryReceipts)
                            await ReceiveMessageAsync(item);

                        var readReceipts = receivedMessages.Where(x => x.MessageType == MessageType.ReadReceipt);
                        foreach (var item in readReceipts)
                            await ReceiveMessageAsync(item);

                        foreach (var messagePart in receivedMessages.Where(x => x.MessageType != MessageType.DeliveryReceipt
                        && x.MessageType != MessageType.ReadReceipt).OrderBy(x => x.DynamicPublicKeyId))
                        {
                            await ReceiveMessageAsync(messagePart);
                        }
                        _appState.SetIsMessagesWaiting(false);
                    }
                    _readToReceive = true;
                }
            }

        }



        public async Task TcpDisconnect()
        {
            await _tcp.DisconnectAsync();
        }

        public async Task UdpDisconnect()
        {
            await _udp.DisconnectAsync();
        }

        public async Task<bool> TcpConnectAsync()
        {
            var remoteDnsHostAddress = _settingsManager.ChatSettings.RemoteDnsHostAddress;
            var remoteTcpPort = _settingsManager.ChatSettings.RemoteTcpPort;
            Debug.WriteLine($"ChatWorker: Attempting to connect via TCP: IP: {remoteDnsHostAddress}, Port: {remoteTcpPort}");
            return await _tcp.ConnectAsync(remoteDnsHostAddress, remoteTcpPort);

        }

        public async Task UdpConnectAsync()
        {
            var remoteDnsHostAddress = _settingsManager.ChatSettings.RemoteDnsHostAddress;
            var remoteUdpPort = _settingsManager.ChatSettings.RemoteUdpPort;
            Debug.WriteLine($"ChatWorker: Attempting to connect via UDP: IP: {remoteDnsHostAddress}, Port: {remoteUdpPort}");
            await _udp.ConnectAsync(remoteDnsHostAddress, remoteUdpPort, _networkClient.Receive);

        }

        async Task<bool> PublishIdentity()
        {
            // publish profile all the time because the server does not save this yet
            var profile = await _repo.GetProfile(_myId);

            if (profile.IsIdentityPublished)
                return true;

            if (!_tcp.IsConnected)
                return false;
            var identityToPublish = new XIdentity
            {
                ID = profile.Id,
                PublicIdentityKey = profile.PublicKey,
                LastSeenUTC = DateTime.UtcNow
            };

            var response = await _chatClient.PublishIdentityAsync(identityToPublish);
            if (response.IsSuccess)
            {
                if (response.Result == _myId && response.Result == profile.Id)
                {
                    profile.IsIdentityPublished = true;
                    await _repo.UpdateProfile(profile);
                    _appState.SetIsIdentityPublished(true);
                    return true;
                }
                _log.Debug($"Error in {nameof(PublishIdentity)}");
            }
            return false;
        }



        async Task<bool> CheckContacts()
        {
            var contacts = await _repo.GetAllContacts();
            foreach (var c in contacts)
            {
                if (c.ContactState == ContactState.Valid)
                {
                    return true;
                }
            }
            return false;
        }
        #endregion

        public void ReceiveCheckForMessagesReply(byte isAnyNews)
        {
            _appState.SetIsMessagesWaiting(isAnyNews > 0);
        }

        async Task ReceiveMessageAsync(XMessage xm)
        {
            var message = new Message
            {
                Id = null,
                YourId = xm.Id,
                LocalMessageState = LocalMessageState.JustReceived,
                SendMessageState = SendMessageState.None,
                Side = MessageSide.You,
                TextCipher = xm.TextCipher,
                ImageCipher = xm.ImageCipher,
                RecipientId = xm.RecipientId,
                SenderId = xm.SenderId,
                MessageType = xm.MessageType,
                DynamicPublicKey = xm.DynamicPublicKey,
                DynamicPublicKeyId = xm.DynamicPublicKeyId,
                PrivateKeyHint = xm.PrivateKeyHint,
                EncryptedDateUtc = xm.EncryptedDateUtc
            };

            if (message.MessageType == MessageType.Text || message.MessageType == MessageType.Media || message.MessageType == MessageType.TextAndMedia)
            {
                await _repo.AddMessage(message);
                await SendDeliveryReceipt(xm);
                var handler = OnIncomingMessagePersisted;
                handler?.Invoke(null, message);

            }
            else if (message.MessageType == MessageType.DeliveryReceipt || message.MessageType == MessageType.ReadReceipt)
            {
                var decryptionkey = await _container.Get<E2ERatchet>().GetEndToEndDecryptionKey(message.SenderId, message.DynamicPublicKey, message.PrivateKeyHint);
                string receiptForSenderMessageId = DecryptBytesToText(message.TextCipher, decryptionkey);
                await _container.Get<E2ERatchet>().SaveIncomingDynamicPublicKeyOnSuccessfulDecryption(message.SenderId, message.DynamicPublicKey, message.DynamicPublicKeyId);
                var messageToConfirm = await _repo.GetMessage(receiptForSenderMessageId, message.SenderId);

                var targetState = message.MessageType == MessageType.DeliveryReceipt
                    ? SendMessageState.Delivered
                    : SendMessageState.Read;

                // Avoid that we overwrite Read with Delivered in case the receipt order is reversed.
                if (messageToConfirm.SendMessageState != SendMessageState.Read)
                {
                    messageToConfirm.SendMessageState = targetState;

                    // Try to create a minimal delay between Delivered and Read.
                    if (targetState == SendMessageState.Delivered)
                    {
                        await _container.Get<MessagesViewModel>().UpdateMessageInThreadReadState(messageToConfirm);
                        await _repo.UpdateMessage(messageToConfirm);
                    }
                    else
                    {
                        await Task.Delay(1000);
                        await _repo.UpdateMessage(messageToConfirm);
                        await _container.Get<MessagesViewModel>().UpdateMessageInThreadReadState(messageToConfirm);
                    }
                }
            }
            _readToReceive = true;
        }



        // same in ChatWorker
        string DecryptBytesToText(byte[] cipherBytes, KeyMaterial64 keyMaterial64)
        {

            var decodeResponse = _container.Get<IVisualCrypt2Service>().BinaryDecodeVisualCrypt(cipherBytes, null);
            if (!decodeResponse.IsSuccess)
                throw new Exception(decodeResponse.Error);
            var decrpytResponse = _container.Get<IVisualCrypt2Service>().Decrypt(decodeResponse.Result, keyMaterial64, null);
            if (!decrpytResponse.IsSuccess)
                throw new Exception(decrpytResponse.Error);
            Cleartext cleartext = decrpytResponse.Result;
            return cleartext.Text;
        }

        internal async Task VerifyAddedContact(string addedContactId)
        {
            try
            {
                var response = await _chatClient.GetIdentityAsync(addedContactId);
                if (response.IsSuccess)
                {
                    XIdentity contactAdded = response.Result;
                    await _repo.UpdateContactWithPublicKey(contactAdded);
                    var handler = ContactUpdateReceived;
                    handler?.Invoke(null, EventArgs.Empty);
                }

            }
            catch (Exception e)
            {
                _log.Exception(e);
                await TcpDisconnect();
            }
        }

        readonly ConcurrentQueue<Message> _readReceipts = new ConcurrentQueue<Message>();

        public async Task PrepareSendReadReceipt(Message message)
        {
            var readReceipt = new Message
            {
                RecipientId = message.SenderId,
                SenderId = _myId,
                MessageType = MessageType.ReadReceipt,
                ThreadText = message.YourId,
                SendMessageState = SendMessageState.Created
            };
            await _container.Get<IChatEncryptionService>().EncryptMessage(readReceipt);

            _readReceipts.Enqueue(readReceipt);
        }


        async Task SendDeliveryReceipt(XMessage xm)
        {
            var deliveryReceipt = new Message
            {
                RecipientId = xm.SenderId,
                SenderId = _myId,
                MessageType = MessageType.DeliveryReceipt,
                ThreadText = xm.Id,
            };
            await _container.Get<IChatEncryptionService>().EncryptMessage(deliveryReceipt);
            await _chatClient.UploadMessage(deliveryReceipt);
        }

        async Task SendReadReceipts()
        {
            Message readReceipt;
            while (_readReceipts.TryDequeue(out readReceipt))
            {
                await _chatClient.UploadMessage(readReceipt);
            }
        }
    }
}
