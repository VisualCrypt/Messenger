using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using VisualCrypt.Applications.Data;
using VisualCrypt.Applications.Models.Chat;
using VisualCrypt.Applications.Models.Chat.MessageCollection.Framework;
using VisualCrypt.Applications.Services.Interfaces;

namespace VisualCrypt.Applications.ViewModels.Chat
{
    public class GetMessagesController
    {
        public Dictionary<string, string> TempClearText { get; }
        public Dictionary<string, byte[]> TempClearImage { get; }

        readonly AppRepository _repo;

        public GetMessagesController(Container container)
        {
            TempClearText = new Dictionary<string, string>();
            TempClearImage = new Dictionary<string, byte[]>();
            _repo = container.Get<AppRepository>();
        }


















        public async Task<IReadOnlyCollection<Message>> LoadMessageRangeAsync(ItemIndexRange itemIndexRange, string contactId)
        {
            if (contactId == null)
                throw new ArgumentNullException(nameof(contactId));

            var messages = await GetMessages(contactId, itemIndexRange.FirstIndex, itemIndexRange.Length);
            if (messages == null)
                return await LoadMessageRangeAsync(itemIndexRange, contactId);
            return messages;
        }
        async Task<IReadOnlyList<Message>> GetMessages(string contactId, int firstIndex, int max)
        {
            IReadOnlyList<Message> queryResult;
            checked
            {
                queryResult = await _repo.GetMessageRange((uint)firstIndex, (uint)max, contactId);
            }

            var isThreadReloadRequired = false;
            foreach (var message in queryResult)
            {
                // Validate everything what is important
                if (IsNotCorrupted(message))
                    continue;
                await _repo.DeleteMessage(message.Id, contactId);
                isThreadReloadRequired = true;
            }
            if (isThreadReloadRequired)
                return null;

            foreach (var message in queryResult)
            {
                var cacheKey = message.GetCacheKey();
                if (TempClearText.ContainsKey(cacheKey))
                    message.ThreadText = TempClearText[cacheKey];
                if (TempClearImage.ContainsKey(cacheKey))
                    message.ThreadMedia = TempClearImage[cacheKey];
            }
            return queryResult;
        }

        bool IsNotCorrupted(Message message)
        {
            if (string.IsNullOrWhiteSpace(message.SenderId))
                return false;
            if (string.IsNullOrWhiteSpace(message.RecipientId))
                return false;
            if (message.SendMessageState == SendMessageState.None && message.Side != MessageSide.You)
                return false;
            return true;
        }
        internal async Task UpdateStatus(string messageId, string recipientId, SendMessageState sentToServer)
        {
            var message = await _repo.GetMessage(messageId, recipientId);
            message.SendMessageState = sentToServer;
            await _repo.UpdateMessage(message);
        }

        public void CachePlaintext(string cacheKey, string clearTextContents)
        {
            if (TempClearText.ContainsKey(cacheKey))
            {
                Debug.WriteLine($"Message #{cacheKey}: WARNING, an attempt is being made to add Cleartext more than one time to the cache.");
                var cached = TempClearText[cacheKey];
                if (cached != clearTextContents)
                    throw new Exception("Different Contents, this sound like an Update.");
                return;
            }
            TempClearText.Add(cacheKey, clearTextContents);
        }

        public void CachePlainImage(string cacheKey, byte[] threadMedia)
        {
            if (TempClearImage.ContainsKey(cacheKey))
            {
                Debug.WriteLine($"Message #{cacheKey}: WARNING, an attempt is being made to add Cleartext more than one time to the cache.");
                var cached = TempClearImage[cacheKey];
                if (cached != threadMedia)
                    Debug.WriteLine("Different Contents, this sound like an Update.");
                return;
            }
            TempClearImage.Add(cacheKey, threadMedia);
        }

        public async Task<Message> GetMessageForSending(string messageId, string recipientId)
        {
            return await _repo.GetMessage(messageId, recipientId);
        }
    }
}
