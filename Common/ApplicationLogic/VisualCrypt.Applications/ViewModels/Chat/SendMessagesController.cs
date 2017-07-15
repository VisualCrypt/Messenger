using System;
using System.Diagnostics;
using System.Threading.Tasks;
using VisualCrypt.Applications.Data;
using VisualCrypt.Applications.Models.Chat;
using VisualCrypt.Applications.Models.Chat.MessageCollection.Framework;
using VisualCrypt.Applications.Services.Interfaces;
using VisualCrypt.Applications.Services.PortableImplementations;
using VisualCrypt.Applications.Workers;
using VisualCrypt.Common;
using VisualCrypt.Cryptography.VisualCrypt2.Infrastructure;
using Helpers = VisualCrypt.Applications.Models.Chat.Helpers;

namespace VisualCrypt.Applications.ViewModels.Chat
{
    public class SendMessagesController
    {
        public event EventHandler<MessageDescriptor> OnOutgoingMessagePersisted;

        readonly ContactsViewModel _contactsViewModel;
        readonly ProfileViewModel _profileViewModel;
        readonly GetMessagesController _getMessagesController;
        readonly AppRepository _repo;
        readonly MessagesViewModel _messagesViewModel;
        readonly IChatEncryptionService _encryptionService;
        readonly IChatClient _chatClient;
        readonly ChatWorker _chatWorker; // this is only for connection management, not nice

        public SendMessagesController(Container container)
        {
            _contactsViewModel = container.Get<ContactsViewModel>();
            _profileViewModel = container.Get<ProfileViewModel>();
            _getMessagesController = container.Get<GetMessagesController>();
            _repo = container.Get<AppRepository>();
            _messagesViewModel = container.Get<MessagesViewModel>();
            _encryptionService = container.Get<IChatEncryptionService>();
            _chatClient = container.Get<IChatClient>();
            _chatWorker = container.Get<ChatWorker>();
        }

        /// <summary>
        /// Must be called with ConfigureAwait(false). Besides perf, downstream locking may otherwise cause deadlocks.
        /// http://blog.stephencleary.com/2012/07/dont-block-on-async-code.html
        /// </summary>
        public async Task SendMessage(string text, string localRelativePath)
        {
            try
            {
                if (_contactsViewModel.CurrentContact == null)
                    return;

                var messageType = SetMessageType(text, localRelativePath);

                var message = new Message
                {
                    Id = null,
                    SenderId = _profileViewModel.Profile.Id,
                    RecipientId = _contactsViewModel.CurrentContact.Id,
                    ThreadText = null,
                    ImageImportPath = null,
                    SendMessageState = SendMessageState.Created,
                    Side = MessageSide.Me,
                    EncryptedDateUtc = DateTime.UtcNow,
                    MessageType = messageType
                };

                await _repo.AddMessage(message);

                message.ThreadText = text;
                message.ImageImportPath = localRelativePath;

                var cacheKey = message.GetCacheKey(); // after saving the message has an id and then this will work
                _getMessagesController.TempClearText.Add(cacheKey, text);

                await _messagesViewModel.AddNewSendMessageToThreadBeforeEncryption(message);

                var response = await _encryptionService.EncryptMessage(message);
                if (!response.IsSuccess)
                    throw new Exception(response.Error); // want to we actually want do if this happens?
                message.SendMessageState = SendMessageState.Encrypted;
                await _repo.UpdateMessage(message);
                await _messagesViewModel.UpdateMessageInThreadToEncryptedState(message);

                await Task.Run(() => Send_MessageAsyncNew(message));
            }
            catch (Exception e)
            {
                // we must catch here, because the try/catch in the calling methid is useless, because we are called with ConfigureAwait(false). 
            }
        }


        async Task Send_MessageAsyncNew(Message message, int failCount = 0)
        {
            message.SendMessageState = SendMessageState.Sending;
            await _repo.UpdateMessage(message);
            await _messagesViewModel.UpdateMessageInThreadToSendingState(message);
            var response = await _chatClient.UploadMessage(message);
            if (response.IsSuccess)
            {
                var sendAck = response.Result.Split(';');
                if (sendAck[0] == message.Id && sendAck[1] == message.RecipientId)
                {
                    message.SendMessageState = SendMessageState.OnServer;
                    await _repo.UpdateMessage(message);
                    await _messagesViewModel.UpdateMessageInThreadToOnServerState(message);
                }
                else
                    throw new Exception();
            }
            else
            {
                if (failCount > 0)
                {
                    message.SendMessageState = SendMessageState.ErrorSending;
                    await _repo.UpdateMessage(message);
                    await _messagesViewModel.UpdateMessageInThreadToErrorSendingState(message);
                    return;
                }
                await _chatWorker.TcpConnectAsync();
                await Send_MessageAsyncNew(message, failCount + 1);
            }
        }


        static MessageType SetMessageType(string text, string imagePath)
        {
            if (!string.IsNullOrWhiteSpace(text) && string.IsNullOrEmpty(imagePath))
                return MessageType.Text;
            if (string.IsNullOrWhiteSpace(text) && !string.IsNullOrEmpty(imagePath))
                return MessageType.Media;
            if (!string.IsNullOrWhiteSpace(text) && !string.IsNullOrEmpty(imagePath))
                return MessageType.TextAndMedia;
            throw new Exception("Messages with neither text nor media are not allowed.");
        }

    }
}
