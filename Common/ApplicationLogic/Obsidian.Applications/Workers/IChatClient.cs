using System.Collections.Generic;
using System.Threading.Tasks;
using Obsidian.Applications.Models.Chat;
using Obsidian.Common;
using Obsidian.Cryptography.Api.Infrastructure;

namespace Obsidian.Applications.Workers
{
    public interface IChatClient
    {
        void Init(string myId, byte[] myPrivateKey, ChatWorker chatWorker);

        string MyId { get;  }
        byte[] MyPrivateKey { get; }
       

        // UDP - Fire and forget
        Task<Response> AnyNews(string myId);
        void ReceiveAnyNewsResponse(byte messageCount);

        Task<Response<IReadOnlyCollection<XMessage>>> DownloadMessages(string myId);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="identity"></param>
        /// <returns>Returns the IdentityID that was successfilly registered.</returns>
        Task<Response<string>> PublishIdentityAsync(XIdentity identity);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="contactId"></param>
        /// <returns>The Identity of the contactID.</returns>
        Task<Response<XIdentity>> GetIdentityAsync(string contactId);

        /// <summary>
        /// Uploads a message.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <returns>A string in the format [messageId];[recipientId]</returns>
        Task<Response<string>>  UploadMessage(Message message);
    }
}