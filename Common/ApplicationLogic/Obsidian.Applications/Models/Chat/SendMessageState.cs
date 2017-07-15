namespace Obsidian.Applications.Models.Chat
{
    public enum SendMessageState : byte
    {
        None = 0,
        Created = 5,
        Encrypted = 10,
        Sending = 15,
        OnServer = 20,
        Delivered = 25,
        Read = 30,
        ErrorSending = 200
    }

    public enum LocalMessageState : byte
    {
        None = 0,

        /// <summary>
        /// Just stored, not yet decrypted.
        /// </summary>
        JustReceived = 5,

        /// <summary>
        /// Delivered, decrypted and wire key encyrpted with local key.
        /// </summary>
        Integrated = 10,

        /// <summary>
        /// Something went wrong, e.g. EncryptedE2EKey was null
        /// </summary>
        LocalDecryptionError = 200
    }
}
