using System;
using System.Threading.Tasks;
using Obsidian.Common;

namespace Obsidian.MessageNode.Core.Controllers
{
    public interface IIdentityController
    {
        Task<XIdentity> PublishIdentityAsync(XIdentity xIdentity, Action<string, byte[]> initTLSUser);
        Task<XIdentity> GetPublishedIdentityAsync(string userId);
    }
}