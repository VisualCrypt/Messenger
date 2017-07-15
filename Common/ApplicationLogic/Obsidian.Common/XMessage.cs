using System;

namespace Obsidian.Common
{
    public class XMessage
    {
        public string Id { get; set; }
        public string SenderId { get; set; }
        public string RecipientId { get; set; }
        public MessageType MessageType { get; set; }
        public DateTime EncryptedDateUtc { get; set; }
        public byte[] TextCipher { get; set; }
        public byte[] ImageCipher { get; set; }
        public byte[] DynamicPublicKey { get; set; }
        public long DynamicPublicKeyId { get; set; }
        public long PrivateKeyHint { get; set; }

    }
}
