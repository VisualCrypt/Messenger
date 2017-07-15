using System;
using System.Threading.Tasks;
using Obsidian.Common;

namespace Obsidian.MessageNode.Core.Controllers
{
    public class CommandProcessor
    {
        readonly IMessageController _messageController;
        readonly IIdentityController _identityController;

        public CommandProcessor()
        {
            _messageController = new MessageController();
            _identityController = new IdentityController();
        }

        public async Task<byte[]> ExecuteAuthenticatedRequestAsync(Command command)
        {
            switch (command.CommandID)
            {
                case CommandID.AnyNews:
                    var newMessages = await _messageController.CheckForMessagesAsync(command.CommandData.DeserializeStringCore());
                    if (newMessages == 0)
                        return null;
                    return new RequestCommand(CommandID.AnyNews_Response, newMessages).Serialize(CommandHeader.Yes);  // when the client receives it via UDP a header is needed.

              

                case CommandID.DownloadMessage:

                    var messages = await _messageController.DownloadMessagesAsync(command.CommandData.DeserializeStringCore());
                    if (messages.Count == 0)
                        return null;
                    return new RequestCommand(CommandID.DownloadMessage_Response, messages).Serialize(CommandHeader.Yes);

                case CommandID.UploadMessage:

                    var message = command.CommandData.DeserializeMessage();
                    var ack = await _messageController.StoreMessageAsync(message);
                    return new RequestCommand(CommandID.UploadMessage_Response, ack).Serialize(CommandHeader.Yes);


                case CommandID.GetIdentity:

                    var addedContactId = command.CommandData.DeserializeStringCore();
                    XIdentity identity = await _identityController.GetPublishedIdentityAsync(addedContactId);
                    return new RequestCommand(CommandID.GetIdentity_Response, identity).Serialize(CommandHeader.Yes);
                default:
                    throw new Exception("Unknown CommandID.");
            }
        }

        public byte[]  ExecuteLostDynamicKey()
        {
            byte dummy = 0xcc; // it doesn't work with zero size contents
            return new RequestCommand(CommandID.LostDynamicKey_Response, dummy).Serialize(CommandHeader.Yes);
        }

        public byte[] ExecuteNoSuchUser()
        {
            byte dummy = 0xcc; // it doesn't work with zero size contents
            return new RequestCommand(CommandID.NoSuchUser_Response, dummy).Serialize(CommandHeader.Yes);
        }

        public async Task<byte[]> ExecutePublishIdentityAsync(Command command, Action<string,byte[]> initTLSUser)
        {
            if(command.CommandID != CommandID.PublishIdentity)
                throw new InvalidOperationException();

            XIdentity identityBeingPublished = command.CommandData.DeserializeXIdentityCore();
            XIdentity verifiedPublishedIdentity = await _identityController.PublishIdentityAsync(identityBeingPublished, initTLSUser);
            return new RequestCommand(CommandID.PublishIdentity_Response, verifiedPublishedIdentity.ID).Serialize(CommandHeader.Yes);
        }
    }
}
