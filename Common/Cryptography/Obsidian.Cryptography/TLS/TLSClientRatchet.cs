using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Obsidian.Cryptography.Api.DataTypes;
using Obsidian.Cryptography.Api.Implementations;
using Obsidian.Cryptography.Api.Infrastructure;
using Obsidian.Cryptography.Api.Interfaces;
using Obsidian.Cryptography.E2E;
using Obsidian.Cryptography.ECC;

namespace Obsidian.Cryptography.TLS
{
    public class TLSClientRatchet
    {
        readonly SemaphoreSlim _publicMemberLock = new SemaphoreSlim(1, 1);
        readonly RatchetTimer _ratchetTimer = new RatchetTimer();

        const int TimesToReUseDynamicSecretAfterEstablishment = 5;
        const int RequiredPushesBeforeReUseIsAllowed = 2; // do not change

        readonly IVisualCrypt2Service _visualCrypt2Service;
        readonly TLSUser _server;
        readonly byte[] _myIdBytes;

        public TLSClientRatchet(string myId, byte[] myPrivateKey, TLSUser server, IVisualCrypt2Service visualCrypt2Service)
        {
            Guard.NotNull(myId, myPrivateKey, server, visualCrypt2Service);
            Guard.NotNull(server.StaticPublicKey);
            MyId = myId;
            _myIdBytes = Encoding.UTF8.GetBytes(MyId);
            _server = server;
            _visualCrypt2Service = visualCrypt2Service;
            _server.AuthSecret = _visualCrypt2Service.CalculateAndHashSharedSecret(myPrivateKey, _server.StaticPublicKey);
        }

        public string MyId { get; }

        public async Task<TLSEnvelope> EncryptRequest(byte[] clearPacket)
        {
            await _publicMemberLock.WaitAsync();
            try
            {
                DynamicSecret dynamicSecret = GetDynamicSecretForEncryption();
                Debug.WriteLine(
                    $"{MyId}: TLSEncrypt: DynamicPublicKeyID: {dynamicSecret.DynamicPublicKeyId}, PrivateKeyHint: {dynamicSecret.PrivateKeyHint}.");

                byte[] authSecret = _server.AuthSecret;

                var securable = ByteArrays.Concatenate(authSecret, _myIdBytes, clearPacket);
                var symmetricKeyMaterial64 = ByteArrays.Concatenate(dynamicSecret.DynamicSharedSecret, new byte[32]);

                var lro = new LongRunningOperation(progress => { }, () => { });
                var clearBytes = new Clearbytes(securable);
                var keyMaterial64 = new KeyMaterial64(symmetricKeyMaterial64);
                var method = new RoundsExponent(0xff);

                var encryptResponse = _visualCrypt2Service.BinaryEncrypt(clearBytes, keyMaterial64, method, lro.Context);
                if (!encryptResponse.IsSuccess)
                    throw new Exception(encryptResponse.Error);

                var encodeResponse = _visualCrypt2Service.BinaryEncodeVisualCrypt(encryptResponse.Result, lro.Context);
                if (!encodeResponse.IsSuccess)
                    throw new Exception(encodeResponse.Error);

                return new TLSEnvelope(dynamicSecret.PrivateKeyHint, dynamicSecret.DynamicPublicKeyId,
                    dynamicSecret.DynamicPublicKey, encodeResponse.Result);
            }
            finally
            {
                _publicMemberLock.Release();
            }

        }

        public async Task<TLSRequest> DecryptRequest(TLSEnvelope tlsEnvelope)
        {
            Guard.NotNull(tlsEnvelope);
            await _publicMemberLock.WaitAsync();
            try
            {
                var ar = new TLSRequest();

                byte[] clientDynamicPrivateKey;
                _server.DynamicPrivateDecryptionKeys.TryGetValue(tlsEnvelope.PrivateKeyHint, out clientDynamicPrivateKey);
                if (clientDynamicPrivateKey == null)
                    throw new Exception(
                        "This should rarely happen. It means, the server has hinted me to private Key I no longer have.");

                RemovePreviousKeys(tlsEnvelope.PrivateKeyHint);

                var dynamicSharedSecret = _visualCrypt2Service.CalculateAndHashSharedSecret(clientDynamicPrivateKey,
                    tlsEnvelope.DynamicPublicKey);
                Debug.WriteLine($"{MyId}: TLSDecrypt:  PrivateKeyHint: {tlsEnvelope.PrivateKeyHint}");

                // TLSAuthMode Combined
                byte[] authSecretBytes = _server.AuthSecret;
                var symmetricKeyMaterial = ByteArrays.Concatenate(dynamicSharedSecret, authSecretBytes);
                // End TLSAuthMode Combined

                // TODO: make LRO optional!
                var lro = new LongRunningOperation(progress => { }, () => { });
                var cipherV2 = VisualCrypt2Formatter.DissectVisualCryptBytes(tlsEnvelope.EncipheredPayload, lro.Context);
                var decryptResponse = _visualCrypt2Service.BinaryDecrypt(cipherV2,
                    new KeyMaterial64(symmetricKeyMaterial), lro.Context);
                var isIncomingKeyUnusable = false;
                if (!decryptResponse.IsSuccess)
                {
                    // has the server just lost the dynamic keys?
                    _server.DynamicPrivateDecryptionKeys.TryGetValue(tlsEnvelope.PrivateKeyHint,
                        out clientDynamicPrivateKey);
                    if (clientDynamicPrivateKey == null)
                        throw new Exception(
                            "This should rarely happen. It means, the server has hinted me to private Key I no longer have.");
                    dynamicSharedSecret = _visualCrypt2Service.CalculateAndHashSharedSecret(clientDynamicPrivateKey,
                        _server.StaticPublicKey);
                    authSecretBytes = new byte[32];
                    symmetricKeyMaterial = ByteArrays.Concatenate(dynamicSharedSecret, authSecretBytes);
                    var decryptResponse2 = _visualCrypt2Service.BinaryDecrypt(cipherV2,
                        new KeyMaterial64(symmetricKeyMaterial), lro.Context);
                    if (!decryptResponse2.IsSuccess)
                        throw new Exception("Decryption failed in all ways!");
                    decryptResponse = decryptResponse2;
                    Debug.WriteLine("Decryption succeded in Anonymous mode.");
                    await Reset();
                    isIncomingKeyUnusable = true;
                }

                byte[] tlsDecryptedRequest = decryptResponse.Result.GetBytes();

                ar.IsAuthenticated = true;
                ar.UserId = _server.UserId;
                ar.CommandData = tlsDecryptedRequest;

                if (!isIncomingKeyUnusable)
                {
                    Guard.NotNull(tlsEnvelope.DynamicPublicKey);
                    if (tlsEnvelope.DynamicPublicKeyId == 0)
                        throw new ArgumentException("A dynamic public key must never have an ID of 0.");
                    _server.LatestDynamicPublicKey = tlsEnvelope.DynamicPublicKey;
                    _server.LatestDynamicPublicKeyId = tlsEnvelope.DynamicPublicKeyId;
                }
                return ar;
            }
            finally
            {
                _publicMemberLock.Release();
            }

        }

        void RemovePreviousKeys(long latestHintToMyPrivateKeyFromServer)
        {
            var canditatesForRemoval = new List<long>();
            foreach (var dynamicPrivateKeyId in _server.DynamicPrivateDecryptionKeys.Keys)
            {
                if (dynamicPrivateKeyId < latestHintToMyPrivateKeyFromServer) // if the key is older than the one the server
                    canditatesForRemoval.Add(dynamicPrivateKeyId);          // has used, it's a candidate for removal
            }

            var toBeRemoved = canditatesForRemoval.OrderByDescending(x => x).Skip(5); // from the candidates, we keep the 5 newest

            foreach (var keyId in toBeRemoved)
                _server.DynamicPrivateDecryptionKeys.Remove(keyId);
        }

        public async Task Reset()
        {
            await _publicMemberLock.WaitAsync();
            try
            {
                _server.DynamicSecret = null;
                _server.LatestDynamicPublicKey = null;
                _server.LatestDynamicPublicKeyId = 0;
                _server.DynamicPrivateDecryptionKeys.Clear();
                _server.PushesDone = 0;
            }
            finally
            {
                _publicMemberLock.Release();
            }
            Debug.WriteLine("Ratchet Reset Done.");
        }


        /// <summary>
        /// Gets the new or reused dynamic secret for TLS encryption.
        /// </summary>
        /// <returns>The dynamic encryption secret.</returns>
        DynamicSecret GetDynamicSecretForEncryption()
        {

            // Here on the client, it's different: as the client initiates conversation,
            // we may mot have a server dynamic public key yet!
            if (_server.LatestDynamicPublicKey == null)
            {
                Debug.Assert(_server.LatestDynamicPublicKeyId == 0 && _server.DynamicSecret == null);
                var newKeyPair = _visualCrypt2Service.GenerateECKeyPair().Result;
                var newDynamicKeyId = _ratchetTimer.GetNextTicks(_server.DynamicPrivateDecryptionKeys.Count > 0 
                    ? _server.DynamicPrivateDecryptionKeys.Keys.Max() 
                    : 0);

                _server.DynamicPrivateDecryptionKeys[newDynamicKeyId] = newKeyPair.PrivateKey; // private Key

                return new DynamicSecret(recipientId: null,
                    dynamicSharedSecret: _visualCrypt2Service.CalculateAndHashSharedSecret(newKeyPair.PrivateKey, _server.StaticPublicKey),
                    dynamicPublicKey: newKeyPair.PublicKey,
                    dynamicPublicKeyId: newDynamicKeyId,
                    privateKeyHint: 0);
            }
            // From here, it's the same like on the server.
            if (_server.DynamicSecret == null)
            {
                NewDynamicEncryptionSecretInUser();
                _server.PushesDone++;
                Debug.WriteLine($"{MyId} *** Created a new dynamic secret for encryption, because there is none for reuse.");
            }
            else if (_server.PushesDone < RequiredPushesBeforeReUseIsAllowed)
            {
                NewDynamicEncryptionSecretInUser();
                _server.PushesDone++;
                Debug.WriteLine($"{MyId} *** Created a new dynamic secret for encryption, because key reuse is not yet allowed");
            }
            else if (_server.DynamicSecret.UseCount >= TimesToReUseDynamicSecretAfterEstablishment)
            {
                NewDynamicEncryptionSecretInUser();
                _server.PushesDone++;
                Debug.WriteLine($"{MyId} *** Created a new dynamic secret for encryption, because time was up.");
            }
            Debug.Assert(_server.DynamicSecret != null);
            _server.DynamicSecret.UseCount++;
            Debug.WriteLine($"{MyId} *** Used same dynamic secret {_server.DynamicSecret.UseCount} times. Pushed {_server.PushesDone} dynamic secrets in total.");
            return _server.DynamicSecret;
        }


        void NewDynamicEncryptionSecretInUser()
        {
            var newKeyPair = _visualCrypt2Service.GenerateECKeyPair().Result;
            var newDynamicKeyId = _ratchetTimer.GetNextTicks(_server.DynamicPrivateDecryptionKeys.Count > 0
                    ? _server.DynamicPrivateDecryptionKeys.Keys.Max()
                    : 0);
            var newDynamicSecret = _visualCrypt2Service.CalculateAndHashSharedSecret(newKeyPair.PrivateKey, _server.LatestDynamicPublicKey);
            _server.DynamicSecret = new DynamicSecret(recipientId: null,
                dynamicSharedSecret: newDynamicSecret,
                dynamicPublicKey: newKeyPair.PublicKey,
                dynamicPublicKeyId: newDynamicKeyId,
                privateKeyHint: _server.LatestDynamicPublicKeyId)
            { UseCount = 0 };

            _server.DynamicPrivateDecryptionKeys[newDynamicKeyId] = newKeyPair.PrivateKey;

            // We cannot purge our keys here, because the server may reuse keys and we don't rely on how long.
            // If we delete keys here, the may be missign when wee need them.
        }
    }
}