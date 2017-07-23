using System;

namespace Obsidian.Cryptography.NoTLS
{
    /// <summary>
    /// Format Specification for a VisualCrypt TLS datagram.
    /// </summary>
    /// <seealso cref="http://blog.fourthbit.com/2014/12/23/traffic-analysis-of-an-ssl-slash-tls-session"/>
    public class NOTLSEnvelope
    {
        public const int HeaderLength = 58;  // incl. Crc32
        public const byte Version = 0x01;
        public const byte MessageType = 0x01;
        public readonly int TotalLength;
        public readonly long PrivateKeyHint;
        public readonly long DynamicPublicKeyId;
        public readonly byte[] DynamicPublicKey;
        public readonly byte[] EncipheredPayload;
        public readonly int Crc32;
        public int? ActualCrc32;
        public bool? Crc32Success;

        public NOTLSEnvelope(byte[] rawRequest)
        {
            // index 0
            if (rawRequest[0] != Version)
                ThrowValidationError("Version must be 1.");

            // index 1
            if (rawRequest[1] != MessageType)
                ThrowValidationError("MessageType must be 1.");

            // index 2, 3, 4, 5,
            TotalLength = BitConverter.ToInt32(rawRequest, 2);
            if (TotalLength != rawRequest.Length)
                ThrowValidationError($"Invalid Size: Coded in rawRequest[2,3,4,5]: {TotalLength}, Actual: {rawRequest.Length} bytes.");

            // index 6, 7, 8, 9, Crc32, calculated on serialization
            Crc32 = BitConverter.ToInt32(rawRequest, 6);

            // index 10, 11, 12, 13, 14, 15, 16, 17
            PrivateKeyHint = BitConverter.ToInt64(rawRequest, 10);

            // index 18, 19, 20, 21, 22, 23, 24, 25
            DynamicPublicKeyId = BitConverter.ToInt64(rawRequest, 18);

            // index 26- 57
            DynamicPublicKey = new byte[32];
            Buffer.BlockCopy(rawRequest, 26, DynamicPublicKey, 0, 32);

            // index 58 (= HeaderLength) - EOF
            EncipheredPayload = new byte[rawRequest.Length - HeaderLength];
            Buffer.BlockCopy(rawRequest, HeaderLength, EncipheredPayload, 0, EncipheredPayload.Length);
        }

        public NOTLSEnvelope(long privateKeyHint, long dynamicPublicKeyId, byte[] dynamicPublickey, byte[] encipheredPayload)
        {
            DynamicPublicKey = dynamicPublickey;
            EncipheredPayload = encipheredPayload;
            PrivateKeyHint = privateKeyHint;
            DynamicPublicKeyId = dynamicPublicKeyId;
            TotalLength = HeaderLength + encipheredPayload.Length;
        }


        static void ThrowValidationError(string error)
        {
            throw new Exception($"{nameof(NOTLSEnvelope)}: {error}");
        }
    }
}
