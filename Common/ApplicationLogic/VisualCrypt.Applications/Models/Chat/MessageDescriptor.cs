using System;

namespace VisualCrypt.Applications.Models.Chat
{
    public sealed class MessageDescriptor
    {
        const int MessageIdLenght = 6; // e.g. 999998
        public readonly string MessageId;
        public readonly string RecipientId;
        readonly string _senderId;
      
        public MessageDescriptor(string messageId, string senderId, string recipientId)
        {
            if (messageId == null || senderId == null || recipientId == null
                || messageId.Length != MessageIdLenght || senderId.Length != 10 || recipientId.Length != 10)
                throw new ArgumentException(nameof(MessageDescriptor));
            MessageId = messageId;
            _senderId = senderId;
            RecipientId = recipientId;
        }

        public bool BelongsToThread(string contactId, string profileId)
        {
            return RecipientId == profileId && _senderId == contactId ||
                   RecipientId == contactId && _senderId == profileId;
        }

        public Message ToMessage()
        {
            return new Message { Id = MessageId, SenderId = _senderId, RecipientId = RecipientId };
        }
    }
}
