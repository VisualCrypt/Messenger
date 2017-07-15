using System.Collections.Generic;
using System.Threading.Tasks;
using Obsidian.Common;

namespace Obsidian.MessageNode.Core.Controllers
{
    public interface IMessageController
    {
        Task<byte> CheckForMessagesAsync(string myId);
        Task<string> StoreMessageAsync(XMessage message);
        Task<List<XMessage>> DownloadMessagesAsync(string myId);
       
    }
}