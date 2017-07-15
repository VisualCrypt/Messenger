using System;
using System.Threading.Tasks;
using Obsidian.Common;
using Obsidian.MessageNode.Core.Data;

namespace Obsidian.MessageNode.Core.Controllers
{
    public class IdentityController : IIdentityController
    {
        readonly IServerRepository _serverRepository;

        public IdentityController()
        {
            _serverRepository = new ServerRepository();
        }

        public async Task<XIdentity> PublishIdentityAsync(XIdentity xIdentity, Action<string, byte[]> initTLSUser)
        {
            await _serverRepository.AddIdentity(xIdentity);
            initTLSUser(xIdentity.ID, xIdentity.PublicIdentityKey);
            return xIdentity;
        }

        public async Task<XIdentity> GetPublishedIdentityAsync(string userId)
        {
            var identity = await _serverRepository.GetIdentityAsync(userId);
            return identity;
        }
    }
}
