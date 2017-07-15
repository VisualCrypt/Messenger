using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Obsidian.Applications.Data;
using Obsidian.Applications.Models.Chat;
using Obsidian.Applications.Services.Interfaces;
using Obsidian.Applications.ViewModels.Chat;
using Obsidian.Applications.Workers;
using Obsidian.Common;
using Obsidian.Cryptography.Api.DataTypes;
using Obsidian.Cryptography.Api.Implementations;
using Obsidian.Cryptography.Api.Infrastructure;
using Obsidian.Cryptography.Api.Interfaces;
using Obsidian.Cryptography.E2E;
using Obsidian.Cryptography.ECC;

namespace Obsidian.Applications.Services.PortableImplementations
{
    public interface IChatEncryptionService
    {
        Response SetMasterPassword(string unprunedUtf16LePassword);

        Response<string> GenerateId();

        Response<ECKeyPair> GenerateIdentityKeypair();

        Task<Response> EncryptMessage(Message message);

        Task<Response> DecryptCipherTextInVisibleBubble(Message message);

    }

    public class PortableChatEncryptionService : IChatEncryptionService
    {
        readonly Container _container;
        readonly IVisualCrypt2Service _visualCrypt2Service;
        readonly ILog _log;
        E2ERatchet _e2ERatchet;  // use GetRatchet()
        readonly GetMessagesController _messagesController;
        readonly IPhotoImportService _photoImportService;
        readonly AppRepository _repo;

        public PortableChatEncryptionService(Container container)
        {
            _container = container;
            _visualCrypt2Service = container.Get<IVisualCrypt2Service>();
            _log = container.Get<ILog>();
            _messagesController = container.Get<GetMessagesController>();
            _photoImportService = container.Get<IPhotoImportService>();
            _repo = container.Get<AppRepository>();

        }

        public Response SetMasterPassword(string unprunedUtf16LePassword)
        {
            var response = new Response();

            try
            {
                Response<KeyMaterial64> sha512Pw64Response = _visualCrypt2Service.HashPassword(_visualCrypt2Service.NormalizePassword(unprunedUtf16LePassword).Result);
                if (sha512Pw64Response.IsSuccess)
                {
                    _visualCrypt2Service.SymmetricKeyRepository.SetPasswordHash(sha512Pw64Response.Result);
                    response.SetSuccess();
                }
                else
                    response.SetError(sha512Pw64Response.Error);
            }
            catch (Exception e)
            {
                response.SetError(e);
                _log.Exception(e);
            }
            return response;
        }

        public Response<string> GenerateId()
        {
            var response = new Response<string>();
            try
            {
                string id;
                while (true)
                {
                    var randomBytes = _visualCrypt2Service.GetRandom(7).Result.X;
                    id = new string(Base64Encoder.EncodeDataToBase64CharArray(randomBytes)).Replace("==", "");
                    Debug.Assert(id.Length == 10);
                    if (id.Contains("+") || id.Contains("/")
                        || id.Contains("I") || id.Contains("l"))
                        continue;
                    break;
                }
                response.Result = id;
                response.SetSuccess();
            }
            catch (Exception e)
            {
                response.SetError(e);
            }
            return response;
        }

        public Response<ECKeyPair> GenerateIdentityKeypair()
        {
            return _visualCrypt2Service.GenerateECKeyPair();
        }

        public async Task<Response> EncryptMessage(Message message)
        {
            var response = new Response();
            try
            {
                var roundsExponent = new RoundsExponent(RoundsExponent.DontMakeRounds);

                Tuple<KeyMaterial64, byte[], long, long> result = await GetRatchet().GetEndToEndEncryptionKey(message.RecipientId);
                KeyMaterial64 diffieHellmannKeyMaterial = result.Item1;
                message.DynamicPublicKey = result.Item2;
                message.DynamicPublicKeyId = result.Item3;
                message.PrivateKeyHint = result.Item4;

                await Task.Run(() => EncryptWithStrategy(message, diffieHellmannKeyMaterial, roundsExponent));
                response.SetSuccess();
            }
            catch (Exception e)
            {
                response.SetError(e.Message);
            }
            return response;

        }
        async Task EncryptWithStrategy(Message message, KeyMaterial64 diffieHellmannKeyMaterial, RoundsExponent roundsExponent)
        {
            switch (message.MessageType)
            {
                case MessageType.Text:
                    message.TextCipher = EncryptTextToBytes(message.ThreadText, diffieHellmannKeyMaterial, roundsExponent);
                    break;
                case MessageType.Media:
                    {
                        var photo = await _photoImportService.ImportPhoto(message.ImageImportPath);
                        message.ThreadMedia = photo.OriginalFileBytes; // we have already set this on message creation?!
                        message.ImageCipher = _visualCrypt2Service.DefaultEncrypt(photo.OriginalFileBytes, diffieHellmannKeyMaterial);


                        message.ImageCipherSnippet = message.GetVisualCryptPreview();
                        break;
                    }
                case MessageType.TextAndMedia:
                    {
                        message.TextCipher = EncryptTextToBytes(message.ThreadText, diffieHellmannKeyMaterial, roundsExponent);
                        var photo = await _photoImportService.ImportPhoto(message.ImageImportPath);
                        message.ThreadMedia = photo.OriginalFileBytes; // we have already set this on message creation?!
                        message.ImageCipher = _visualCrypt2Service.DefaultEncrypt(photo.OriginalFileBytes, diffieHellmannKeyMaterial);
                        message.ImageCipherSnippet = message.GetVisualCryptPreview();
                    }
                    break;
                case MessageType.DeliveryReceipt:
                case MessageType.ReadReceipt:
                    message.TextCipher = EncryptTextToBytes(message.ThreadText, diffieHellmannKeyMaterial, roundsExponent);
                    break;
                default:
                    throw new Exception("Invalid MessageType");
            }
            if (message.MessageType != MessageType.ReadReceipt)
                message.EncryptedE2EEncryptionKey = _visualCrypt2Service.DefaultEncrypt(diffieHellmannKeyMaterial.GetBytes());
        }

        public async Task<Response> DecryptCipherTextInVisibleBubble(Message message)
        {
            var response = new Response();
            try
            {
                if (message.ThreadText != null && message.MessageType == MessageType.Text) // TODO: do this properly ....nothing to do, already decrypted
                {
                    response.SetSuccess();
                    return response;
                }

                KeyMaterial64 decryptionkey;
                if (message.LocalMessageState == LocalMessageState.JustReceived) // this is an incoming message
                {
                    decryptionkey = await GetRatchet().GetEndToEndDecryptionKey(message.SenderId, message.DynamicPublicKey, message.PrivateKeyHint);
                    await Task.Run(() => DecryptToCache(message, decryptionkey));
                    await GetRatchet().SaveIncomingDynamicPublicKeyOnSuccessfulDecryption(message.SenderId, message.DynamicPublicKey, message.DynamicPublicKeyId);
                    message.EncryptedE2EEncryptionKey = _visualCrypt2Service.DefaultEncrypt(decryptionkey.GetBytes());
                    message.LocalMessageState = LocalMessageState.Integrated;

                    Debug.Assert(message.MessageType != MessageType.ReadReceipt && message.MessageType != MessageType.DeliveryReceipt);

                    await _repo.UpdateMessage(message);
                    await _container.Get<ChatWorker>().PrepareSendReadReceipt(message);
                }
                else // this is a message we have stored locally
                {
                    if (message.EncryptedE2EEncryptionKey == null)
                        message.LocalMessageState = LocalMessageState.LocalDecryptionError;
                    else
                    {
                        decryptionkey = new KeyMaterial64(_visualCrypt2Service.DefaultDecrypt(message.EncryptedE2EEncryptionKey));
                        await Task.Run(() => DecryptToCache(message, decryptionkey));
                    }
                    // should we check that a read receipt has really been sent and retry till we are sure?

                }
                response.SetSuccess();
            }
            catch (Exception e)
            {
                _log.Exception(e);
            }
            return response;
        }






        byte[] EncryptTextToBytes(string clearText, KeyMaterial64 keyMaterial64, RoundsExponent roundsExponent)
        {
            var encryptResponse = _visualCrypt2Service.Encrypt(new Cleartext(clearText), keyMaterial64, roundsExponent, null);
            if (!encryptResponse.IsSuccess)
                throw new InvalidOperationException(encryptResponse.Error);
            var encodeResponse = _visualCrypt2Service.BinaryEncodeVisualCrypt(encryptResponse.Result, null);
            if (!encodeResponse.IsSuccess)
                throw new InvalidOperationException(encodeResponse.Error);
            return encodeResponse.Result;
        }






        void DecryptToCache(Message message, KeyMaterial64 keyMaterial64)
        {
            switch (message.MessageType)
            {
                case MessageType.Text:
                    {
                        message.ThreadText = DecryptBytesToText(message.TextCipher, keyMaterial64);
                        _messagesController.CachePlaintext(message.GetCacheKey(), message.ThreadText);
                    }
                    break;
                case MessageType.Media:
                    {
                        message.ThreadMedia = _visualCrypt2Service.DefaultDecrypt(message.ImageCipher, keyMaterial64);
                        _messagesController.CachePlainImage(message.GetCacheKey(), message.ThreadMedia);
                        message.ImageCipherSnippet = message.GetVisualCryptPreview();
                    }
                    break;
                case MessageType.TextAndMedia:
                    {
                        message.ThreadText = DecryptBytesToText(message.TextCipher, keyMaterial64);
                        _messagesController.CachePlaintext(message.GetCacheKey(), message.ThreadText);
                        message.ThreadMedia = _visualCrypt2Service.DefaultDecrypt(message.ImageCipher, keyMaterial64);
                        _messagesController.CachePlainImage(message.GetCacheKey(), message.ThreadMedia);
                        message.ImageCipherSnippet = message.GetVisualCryptPreview();
                    }
                    break;
                //case MessageType.ReadReceipt:
                //    {
                //        message.ThreadText = DecryptBytesToText(message.TextCipher, keyMaterial64);
                //        _messagesController.CachePlaintext(message.GetCacheKey(), message.ThreadText);
                //    }
                //    break;
                default:
                    throw new Exception("Invalid MessageType.");
            }
        }

        // same in ChatWorker
        string DecryptBytesToText(byte[] cipherBytes, KeyMaterial64 keyMaterial64)
        {

            var decodeResponse = _visualCrypt2Service.BinaryDecodeVisualCrypt(cipherBytes, null);
            if (!decodeResponse.IsSuccess)
                throw new Exception(decodeResponse.Error);
            var decrpytResponse = _visualCrypt2Service.Decrypt(decodeResponse.Result, keyMaterial64, null);
            if (!decrpytResponse.IsSuccess)
                throw new Exception(decrpytResponse.Error);
            Cleartext cleartext = decrpytResponse.Result;
            return cleartext.Text;
        }



        E2ERatchet GetRatchet()
        {
            if (_e2ERatchet != null)
                return _e2ERatchet;
            _e2ERatchet = _container.Get<E2ERatchet>();
            return _e2ERatchet;
        }


    }
}