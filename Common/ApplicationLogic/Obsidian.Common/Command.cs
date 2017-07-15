namespace Obsidian.Common
{
    public sealed class Command
    {
        public readonly CommandID CommandID;
        public readonly byte[] CommandData;

        public Command(CommandID commandID, byte[] commandData)
        {
            CommandID = commandID;
            CommandData = commandData;
        }
    }
}
