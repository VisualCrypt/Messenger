using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Obsidian.Applications.Models.Chat;
using Obsidian.Applications.Models.Serialization;
using Obsidian.Applications.Services.Interfaces;
using Obsidian.Common;
using Obsidian.Cryptography.E2E;

namespace Obsidian.Applications.Data
{
    public class AppRepository
    {
        static readonly SemaphoreSlim ProfilesSemaphore = new SemaphoreSlim(1, 1);
        static readonly SemaphoreSlim ContactsSemaphore = new SemaphoreSlim(1, 1);
        static readonly SemaphoreSlim MessagesSemaphore = new SemaphoreSlim(1, 1);

        readonly IAsyncRepository<Profile> _profiles;
        readonly IAsyncRepository<Identity> _contacts;
        readonly IAsyncRepository<Message> _messages;
        string _myId;

        string MyId
        {
            set
            {
                if (_myId == null && value != null)
                    _myId = value;
                if (value != _myId)
                    throw new InvalidOperationException();
            }
        }

        public AppRepository(Container container)
        {
            _profiles = container.Get<IAsyncRepository<Profile>>();
            _contacts = container.Get<IAsyncRepository<Identity>>();
            _messages = container.Get<IAsyncRepository<Message>>();
        }

        public async Task AddProfile(Profile profile)
        {
            MyId = profile.Id;
            await ProfilesSemaphore.WaitAsync();
            try
            {
                await _profiles.Add(profile);
            }
            finally
            {
                ProfilesSemaphore.Release();
            }
        }

        public async Task<Profile> GetProfile(string myId)
        {
            MyId = myId;
            await ProfilesSemaphore.WaitAsync();
            try
            {
                return await _profiles.Get(myId);
            }
            finally
            {
                ProfilesSemaphore.Release();
            }
        }

        public async Task<IReadOnlyList<Profile>> GetProfiles()
        {
            await ProfilesSemaphore.WaitAsync();
            try
            {
                return await _profiles.GetAll();
            }
            finally
            {
                ProfilesSemaphore.Release();
            }
        }

        public async Task UpdateProfile(Profile profile)
        {
            await ProfilesSemaphore.WaitAsync();
            try
            {
                await _profiles.Update(profile);
            }
            finally
            {
                ProfilesSemaphore.Release();
            }
        }

        public async Task UpdateEncryptedProfileImage(string id, byte[] encryptedProfileImage)
        {
            await ProfilesSemaphore.WaitAsync();
            try
            {
                var profile = await _profiles.Get(id);
                profile.EncryptedProfileImage = encryptedProfileImage;
                await _profiles.Update(profile);
            }
            finally
            {
                ProfilesSemaphore.Release();
            }
        }

	    public async Task UpdateEncryptedContactImage(string id, byte[] encryptedContactImage)
	    {
		    await ProfilesSemaphore.WaitAsync();
		    try
		    {
			    Identity contact = await _contacts.Get(id);
			    contact.Image = encryptedContactImage;
			    await _contacts.Update(contact);
		    }
		    finally
		    {
			    ProfilesSemaphore.Release();
		    }
	    }

	    public async Task UpdateContactName(string id, string newName)
	    {
		    await ProfilesSemaphore.WaitAsync();
		    try
		    {
			    Identity contact = await _contacts.Get(id);
			    contact.Name = newName;
			    await _contacts.Update(contact);
		    }
		    finally
		    {
			    ProfilesSemaphore.Release();
		    }
	    }

		public async Task<IReadOnlyList<Identity>> GetAllContacts()
        {
            await ContactsSemaphore.WaitAsync();
            try
            {
                return await _contacts.GetAll();
            }
            finally
            {
                ContactsSemaphore.Release();
            }

        }

        public async Task DeleteContacts(IEnumerable<Identity> contacts)
        {
            await ContactsSemaphore.WaitAsync();
            try
            {
                foreach (var contact in contacts)
                    await _contacts.Delete(contact.Id);
            }
            finally
            {
                ContactsSemaphore.Release();
            }
        }

        public async Task AddContact(Identity contact)
        {
            await ContactsSemaphore.WaitAsync();
            try
            {
                await _contacts.Add(contact);
            }
            finally
            {
                ContactsSemaphore.Release();
            }
        }


        public async Task UpdateContactWithPublicKey(XIdentity identity)
        {
            if (identity == null)
                throw new ArgumentNullException(nameof(identity));

            if (identity.ContactState != ContactState.Valid)
                throw new Exception($"Expected IdentityState.Added but was {identity.ContactState}");

            if (identity.PublicIdentityKey == null)
                throw new Exception("The public key must not be null");

            await ContactsSemaphore.WaitAsync();
            try
            {
                var contact = await _contacts.Get(identity.ID);
                if (contact != null)
                {
                    contact.FirstSeenUtc = identity.FirstSeenUTC;
                    contact.LastSeenUtc = identity.LastSeenUTC;
                    contact.Name = identity.Name;
                    contact.Image = identity.Image;
                    contact.StaticPublicKey = identity.PublicIdentityKey;
                    contact.ContactState = identity.ContactState;
                    await _contacts.Update(contact);
                }
            }
            finally
            {
                ContactsSemaphore.Release();
            }
        }

        public async Task UpdateUser(E2EUser user)
        {
            await ContactsSemaphore.WaitAsync();
            try
            {
                Identity contact = await _contacts.Get(user.UserId);
                var serialized = E2EUserSerializer.Serialize(user);
                contact.CryptographicInformation = serialized;
                await _contacts.Update(contact);
            }
            finally
            {
                ContactsSemaphore.Release();
            }
        }



        public async Task<E2EUser> GetUserById(string userId)
        {
            await ContactsSemaphore.WaitAsync();
            try
            {
                var contact = await _contacts.Get(userId);
                Debug.Assert(contact.Id == userId);
                E2EUser user = E2EUserSerializer.Deserialize(contact.CryptographicInformation);
                if (user == null) // for example, when the contact has just been added
                    user = new E2EUser {DynamicPrivateDecryptionKeys = new Dictionary<long, byte[]>()};
                user.UserId = userId;
                user.StaticPublicKey = contact.StaticPublicKey;
                return user;
            }
            finally
            {
                ContactsSemaphore.Release();
            }
        }


        public async Task AddMessage(Message message)
        {
            var page = GuessPage(message);
            await MessagesSemaphore.WaitAsync();
            try
            {
                var previousMessages = await _messages.GetRange(0, 1, page);
                var previousMessage = previousMessages.Count == 0 ? null : previousMessages[0];
                message.SetPreviousSide(previousMessage);
                await _messages.Add(message, page);
            }
            finally
            {
                MessagesSemaphore.Release();
            }

        }

        public async Task<Message> GetMessage(string messageId, string page)
        {
            ValidateMessagePage(page);
            await MessagesSemaphore.WaitAsync();
            try
            {
                return await _messages.Get(messageId, page);
            }
            finally
            {
                MessagesSemaphore.Release();
            }
        }


        public async Task UpdateMessage(Message message)
        {
            var page = GuessPage(message);
            await MessagesSemaphore.WaitAsync();
            try
            {
                await _messages.Update(message, page);
            }
            finally
            {
                MessagesSemaphore.Release();
            }
        }

        public async Task DeleteMessage(string messageId, string page)
        {
            ValidateMessagePage(page);
            await MessagesSemaphore.WaitAsync();
            try
            {
                await _messages.Delete(messageId, page);
            }
            finally
            {
                MessagesSemaphore.Release();
            }
        }




        public async Task<uint> GetMessageCount(string page)
        {
            ValidateMessagePage(page);
            await MessagesSemaphore.WaitAsync();
            try
            {
                return await _messages.Count(page);
            }
            finally
            {
                MessagesSemaphore.Release();
            }
        }

        public async Task<IReadOnlyList<Message>> GetMessageRange(uint firstIndex, uint maxCount, string page)
        {
            ValidateMessagePage(page);
            await MessagesSemaphore.WaitAsync();
            try
            {
                var messages = await _messages.GetRange(firstIndex, maxCount, page);
                return messages;
            }
            finally
            {
                MessagesSemaphore.Release();
            }
          
        }

        void ValidateMessagePage(string page)
        {
            if (string.IsNullOrWhiteSpace(page) || page == _myId)
                throw new InvalidOperationException();
        }

        string GuessPage(Message message)
        {
            string page;
            if (message.Side == MessageSide.Me)
                page = message.RecipientId;
            else if (message.Side == MessageSide.You)
                page = message.SenderId;
            else throw new InvalidOperationException();
            ValidateMessagePage(page);
            return page;
        }

    }
}
