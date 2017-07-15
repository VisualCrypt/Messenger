namespace Obsidian.Cryptography.ECC
{
    public sealed class DynamicSecret
    {
        public string RecipientId { get; }
        public byte[] DynamicSharedSecret { get; }
        public byte[] DynamicPublicKey { get; }
        public long DynamicPublicKeyId { get; }
        public long PrivateKeyHint { get; }

        public DynamicSecret(string recipientId, byte[] dynamicSharedSecret, byte[] dynamicPublicKey, long dynamicPublicKeyId, long privateKeyHint)
        {
            RecipientId = recipientId;
            DynamicSharedSecret = dynamicSharedSecret;
            DynamicPublicKey = dynamicPublicKey;
            DynamicPublicKeyId = dynamicPublicKeyId;
            PrivateKeyHint = privateKeyHint;
        }

        public int UseCount { get; set; }
    }
}