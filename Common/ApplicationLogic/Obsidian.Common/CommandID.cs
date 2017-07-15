namespace Obsidian.Common
{
    public enum CommandID : byte
    {
        Zero = 0,
        AnyNews = 10,
        DownloadMessageParts = 20,
        UploadMessageParts = 30,
        AnyNews_Response = 31,
        DownloadMessageParts_Response = 32,
        UploadMessageParts_Response = 33,
        UploadMessage = 34,
        UploadMessage_Response = 35,
        PublishIdentity = 50,
        PublishIdentity_Response = 51,
        GetIdentity = 52,
        GetIdentity_Response = 53,
        Headerless = 54,
        LostDynamicKey_Response = 55,
        NoSuchUser_Response = 56,
        DownloadMessage = 57,
        DownloadMessage_Response = 58
    }

    public enum CommandHeader
    {
        Yes,
        No
    }
}
