using System.Numerics;
using Obsidian.Cryptography.Api.DataTypes;
using Obsidian.Cryptography.Api.Implementations;
using Obsidian.Cryptography.Api.Infrastructure;
using Obsidian.Cryptography.ECC;

namespace Obsidian.Cryptography.Api.Interfaces
{
    public interface IVisualCrypt2Service
    {
        void Init(IPlatform platform, string name = "Default");

        string Name { get; }

        SymmetricKeyRepository SymmetricKeyRepository { get; }

        Response<QualifiedRandom> GetRandom(int randomLenght);

        Response<QualifiedRandom> TestRandomNumberGeneration(int sampleSize, int randomLenght);

        Response<string> SuggestRandomPassword();
        
        Response<NormalizedPassword> NormalizePassword(string rawPassword);

        Response<KeyMaterial64> HashPassword(NormalizedPassword normalizedPassword);

        Response<CipherV2> Encrypt(Cleartext cleartext, KeyMaterial64 keyMaterial64, RoundsExponent roundsExponent, LongRunningOperationContext context);

        Response<Cleartext> Decrypt(CipherV2 cipherV2, KeyMaterial64 keyMaterial64, LongRunningOperationContext context);

        Response<VisualCryptText> EncodeVisualCrypt(CipherV2 cipherV2);

        Response<CipherV2> DecodeVisualCrypt(string visualCryptText, LongRunningOperationContext context);

        Response<CipherV2> BinaryEncrypt(Clearbytes clearBytes, KeyMaterial64 keyMaterial64, RoundsExponent roundsExponent, LongRunningOperationContext context);
       
        Response<Clearbytes> BinaryDecrypt(CipherV2 cipherV2, KeyMaterial64 keyMaterial64, LongRunningOperationContext context);

        Response<CipherV2> BinaryDecodeVisualCrypt(byte[] visualCryptBytes, LongRunningOperationContext context);

        Response<byte[]> BinaryEncodeVisualCrypt(CipherV2 cipherV2, LongRunningOperationContext context);

       
        Response<ECKeyPair> GenerateECKeyPair();

        byte[] CalculateAndHashSharedSecret(byte[] privateKey, byte[] publicKey);

        byte[] DefaultEncrypt(byte[] plaintextBytes, KeyMaterial64 nonDefaultKey = null);

        byte[] DefaultDecrypt(byte[] cipherTextBytes, KeyMaterial64 nonDefaultKey = null);

        BigInteger GetPositive520BitInteger();
    }
}