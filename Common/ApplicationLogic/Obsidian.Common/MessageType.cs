namespace Obsidian.Common
{
    public enum  MessageType : byte
    {
        None = 0,
        Text = 1,
        Media = 2,
        TextAndMedia = 3,
        ReadReceipt = 20,
        DeliveryReceipt = 21,
    }
}
