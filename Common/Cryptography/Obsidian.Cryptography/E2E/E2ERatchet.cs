using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Obsidian.Cryptography.Api.DataTypes;
using Obsidian.Cryptography.Api.Infrastructure;
using Obsidian.Cryptography.Api.Interfaces;

namespace Obsidian.Cryptography.E2E
{
    public class E2ERatchet
    {

        const int KeepLatestDynamicPrivateKeys = 50;

        readonly IVisualCrypt2Service _visualCrypt2Service;
        readonly byte[] _myStaticPrivateKey;
        readonly Func<string, Task<E2EUser>> _gu;  // use GetCheckedUser()
        readonly Func<E2EUser, Task> _updateUser;
        readonly RatchetTimer _ratchetTimer = new RatchetTimer();

        public E2ERatchet(string myId, byte[] myStaticPrivateKey, IVisualCrypt2Service visualCrypt2Service, Func<string, Task<E2EUser>> getUser, Func<E2EUser, Task> updateUser)
        {
            MyId = myId;
            _myStaticPrivateKey = myStaticPrivateKey;
            _visualCrypt2Service = visualCrypt2Service;
            _gu = getUser;
            _updateUser = updateUser;
        }

        public string MyId { get; }

        public async Task<KeyMaterial64> GetEndToEndDecryptionKey(string senderId, byte[] dynamicPublicKey, long privateKeyHint)
        {
            var user = await GetCheckedUser(senderId);
            byte[] dynamicPrivateKeyOrStaticPrivateKey;
            if (privateKeyHint == 0)
                dynamicPrivateKeyOrStaticPrivateKey = _myStaticPrivateKey;
            else
            {
                if (!user.DynamicPrivateDecryptionKeys.TryGetValue(privateKeyHint, out dynamicPrivateKeyOrStaticPrivateKey))
                    throw new Exception("I have lost my dynamic private keys! I really need to send new keys to my conversation partner (and ask to resend the message)!");
            }

            var dynamicSharedSecret = _visualCrypt2Service.CalculateAndHashSharedSecret(dynamicPrivateKeyOrStaticPrivateKey, dynamicPublicKey);

            var symmetricKeyMaterial = ByteArrays.Concatenate(dynamicSharedSecret, user.AuthSecret);

            return new KeyMaterial64(symmetricKeyMaterial);
        }

        public async Task SaveIncomingDynamicPublicKeyOnSuccessfulDecryption(string senderId, byte[] dynamicPublicKey, long dynamicPublicKeyId)
        {
            Guard.NotNull(senderId, dynamicPublicKey);
            if (dynamicPublicKeyId == 0)
                throw new ArgumentException("A dynamic public key must never have an ID of 0.");
            var user = await GetCheckedUser(senderId);
            user.LatestDynamicPublicKey = dynamicPublicKey;
            user.LatestDynamicPublicKeyId = dynamicPublicKeyId;
            await _updateUser(user);
        }

        public async Task<Tuple<KeyMaterial64, byte[], long, long>> GetEndToEndEncryptionKey(string recipientId)
        {
            var user = await GetCheckedUser(recipientId);
			long existingMaxKeyId = 0;
			if (user.DynamicPrivateDecryptionKeys.Keys.Count > 0) // count might be 0 initially...might be a bug or not
			{
				existingMaxKeyId = user.DynamicPrivateDecryptionKeys.Keys.Max();
			}

            long nextDynamicPublicKeyId = _ratchetTimer.GetNextTicks(existingMaxKeyId);

            var ecdhKeypair = _visualCrypt2Service.GenerateECKeyPair().Result;
            byte[] dynamicPublicKey = ecdhKeypair.PublicKey;

            long privateKeyHint;

            user.DynamicPrivateDecryptionKeys[nextDynamicPublicKeyId] = ecdhKeypair.PrivateKey;
            RemoveExcessKeys(user);
            await _updateUser(user);

            byte[] dynamicOrStaticPublicKey;
            if (user.LatestDynamicPublicKey != null)
            {
                dynamicOrStaticPublicKey = user.LatestDynamicPublicKey;
                privateKeyHint = user.LatestDynamicPublicKeyId;
            }
            else
            {
                dynamicOrStaticPublicKey = user.StaticPublicKey;
                privateKeyHint = 0;
            }

            var dynamicSharedSecret = _visualCrypt2Service.CalculateAndHashSharedSecret(ecdhKeypair.PrivateKey, dynamicOrStaticPublicKey);

            var symmetricKeyMaterial = ByteArrays.Concatenate(dynamicSharedSecret, user.AuthSecret);
            return new Tuple<KeyMaterial64, byte[], long, long>(new KeyMaterial64(symmetricKeyMaterial), dynamicPublicKey, nextDynamicPublicKeyId, privateKeyHint);
        }

        // TODO: Review this, compare it with TLSCLient.RemovePreviousKeys and when key cleanup is done
        // This may not work correctly.
        void RemoveExcessKeys(E2EUser user)
        {
            var excess = user.DynamicPrivateDecryptionKeys.Keys.OrderByDescending(k => k).Skip(KeepLatestDynamicPrivateKeys);
            foreach (var keyId in excess)
                user.DynamicPrivateDecryptionKeys.Remove(keyId);
        }

        async Task<E2EUser> GetCheckedUser(string userId)
        {
            E2EUser user = await _gu(userId);
            var changed = false;
            if (user.AuthSecret == null)
            {
                user.AuthSecret = _visualCrypt2Service.CalculateAndHashSharedSecret(_myStaticPrivateKey,
                    user.StaticPublicKey);
                changed = true;

            }
            if (user.DynamicPrivateDecryptionKeys == null)
            {
                user.DynamicPrivateDecryptionKeys = new Dictionary<long, byte[]>();
                changed = true;
            }
            if (changed)
                await _updateUser(user);
            return user;
        }
    }
}