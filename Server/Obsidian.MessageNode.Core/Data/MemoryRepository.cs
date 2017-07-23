using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Obsidian.Common;
using Obsidian.Cryptography.Api.Infrastructure;

namespace Obsidian.MessageNode.Core.Data
{
    class MemoryRepository : IServerRepository
    {
        static readonly TimeSpan SimulatedDbDelay = new TimeSpan(0, 0, 0, 0, 1);

        // Key: RecipientID
        static readonly ConcurrentDictionary<string, List<XMessage>> MessagesByRecipientId = new ConcurrentDictionary<string, List<XMessage>>();
        static readonly List<XMessage> EmptyMessageList = new List<XMessage>();

        static readonly List<XIdentity> Identities = new List<XIdentity>();
        public async Task<bool> AnyNews(string myId)
        {
            await Task.Delay(SimulatedDbDelay);
            List<XMessage> messageParts;
            var isAnyParts = MessagesByRecipientId.TryGetValue(myId, out messageParts);
            if (!isAnyParts || messageParts == null || messageParts.Count == 0)
                return false;
            return true;
        }


        public async Task AddIdentity(XIdentity identity)
        {
            await Task.Delay(SimulatedDbDelay);

            if (identity != null && !string.IsNullOrEmpty(identity.ID) && identity.PublicIdentityKey != null &&
                identity.PublicIdentityKey.Length == 32)
            {
                var alreadyExists = Identities.SingleOrDefault(i => i.ID == identity.ID);
                if (alreadyExists != null)
                {
                    if (!ByteArrays.AreAllBytesEqual(identity.PublicIdentityKey, alreadyExists.PublicIdentityKey))
                        throw new Exception("Updating PublicIdentityKey is not allowed!");
                }
                else
                {
                    identity.ContactState = ContactState.Valid;
                    Identities.Add(identity);
                }

                return;
            }
            throw new Exception("Invalid request to publish an Identity.");

        }

        public async Task<XIdentity> GetIdentityAsync(string identityId)
        {
            await Task.Delay(SimulatedDbDelay);
            var requestedIdentity = new XIdentity { ID = identityId };
            var foundIdentity = Identities.SingleOrDefault(i => i.ID == identityId);
            if (foundIdentity == null)
            {
                requestedIdentity.ContactState = ContactState.NonExistent;
            }
            else if (foundIdentity.ContactState == ContactState.Revoked)
            {
                requestedIdentity.ContactState = ContactState.Revoked;
            }
            else
            {
                if (foundIdentity.ContactState != ContactState.Valid)
                    throw new Exception("Expected a state of 'Publihed'");
                requestedIdentity.ContactState = ContactState.Valid;
                requestedIdentity.FirstSeenUTC = foundIdentity.FirstSeenUTC;
                requestedIdentity.LastSeenUTC = foundIdentity.LastSeenUTC;
                requestedIdentity.Name = foundIdentity.Name;
                requestedIdentity.Image = foundIdentity.Image;
                requestedIdentity.ImagePath = foundIdentity.ImagePath;
                requestedIdentity.PublicIdentityKey = foundIdentity.PublicIdentityKey;
            }
            return requestedIdentity;
        }

        public async Task<string> AddMessage(XMessage message)
        {
            await Task.Delay(SimulatedDbDelay);
            List<XMessage> messages;
            var isAnyParts = MessagesByRecipientId.TryGetValue(message.RecipientId, out messages);
            if (isAnyParts && messages != null)
            {
                messages.Add(message);
            }
            else
            {
                messages = new List<XMessage> { message };
                MessagesByRecipientId[message.RecipientId] = messages;
            }
            return $"{message.Id};{message.RecipientId}";
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="myId"></param>
        /// <returns>A non-null collection.</returns>
        public async Task<List<XMessage>> GetMessages(string myId)
        {
            await Task.Delay(SimulatedDbDelay);
            // TODO: remove only after successful download
            List<XMessage> messageParts;
            var isAnyParts = MessagesByRecipientId.TryRemove(myId, out messageParts);
            if (!isAnyParts || messageParts == null || messageParts.Count == 0)
                return EmptyMessageList;
            return messageParts;
        }
    }
}
