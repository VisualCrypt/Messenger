using System.Collections.Generic;
using System.Threading.Tasks;
using Obsidian.Common;

namespace Obsidian.MessageNode.Core.Data
{
    interface IServerRepository
    {
        Task<bool> AnyNews(string myId);
        Task<List<XMessage>> GetMessages(string myId);
        Task AddIdentity(XIdentity identity);
        Task<XIdentity> GetIdentityAsync(string identityId);
        Task<string> AddMessage(XMessage message);
    }
}
