namespace Obsidian.Common
{
    public sealed class RequestCommand
    {
        public readonly CommandID CommandId;
        public readonly object Contents;
        public RequestCommand(CommandID commandId, object serializableContents)
        {
            CommandId = commandId;
            Contents = serializableContents;
        }
    }
}
