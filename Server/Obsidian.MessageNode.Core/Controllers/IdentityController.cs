using System;
using System.Threading.Tasks;
using Obsidian.Common;
using Obsidian.MessageNode.Core.Data;

namespace Obsidian.MessageNode.Core.Controllers
{
    public class IdentityController : IIdentityController
    {
        readonly IServerRepository _repository;

        public IdentityController()
        {
            _repository = new MemoryRepository();
        }

        public async Task<XIdentity> PublishIdentityAsync(XIdentity xIdentity, Action<string, byte[]> initTLSUser)
        {
            await _repository.AddIdentity(xIdentity);
            initTLSUser(xIdentity.ID, xIdentity.PublicIdentityKey);
            return xIdentity;
        }

        public async Task<XIdentity> GetPublishedIdentityAsync(string userId)
        {
            var identity = await _repository.GetIdentityAsync(userId);
            return identity;
        }
    }
}
