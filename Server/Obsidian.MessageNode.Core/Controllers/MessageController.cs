using System.Collections.Generic;
using System.Threading.Tasks;
using Obsidian.Common;
using Obsidian.MessageNode.Core.Data;

namespace Obsidian.MessageNode.Core.Controllers
{
    sealed class MessageController : IMessageController
    {
        readonly IServerRepository _serverRepository;

        public MessageController()
        {
            _serverRepository = new SqlRepository();
          
        }

        public async Task<byte> CheckForMessagesAsync(string myId)
        {
            var isNewMessage = await _serverRepository.AnyNews(myId);
            if (isNewMessage)
                return 1;
            return 0;
        }

        public async Task<string> StoreMessageAsync(XMessage message)
        {
            return await _serverRepository.AddMessage(message);
        }

        public async Task<List<XMessage>> DownloadMessagesAsync(string myId)
        {
            var messageParts = await _serverRepository.GetMessages(myId);
            return messageParts;
        }


    }
}
