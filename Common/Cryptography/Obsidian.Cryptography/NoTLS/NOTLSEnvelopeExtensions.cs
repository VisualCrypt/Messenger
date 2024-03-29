using System;
using Obsidian.Cryptography.Api.Infrastructure;
using Obsidian.Cryptography.TLS;

namespace Obsidian.Cryptography.NoTLS
{
    public static class NOTLSEnvelopeExtensions
    {
        public static byte[] Serialize(this NOTLSEnvelope tlsEnvelope)
        {
            if (tlsEnvelope.TotalLength != NOTLSEnvelope.HeaderLength + tlsEnvelope.EncipheredPayload.Length)
                throw new InvalidOperationException("Actual payload lenght does not match Length field.");

            int serializedLength = NOTLSEnvelope.HeaderLength + tlsEnvelope.EncipheredPayload.Length;
            var serialized = new byte[serializedLength];

            serialized[0] = NOTLSEnvelope.Version;
            serialized[1] = NOTLSEnvelope.MessageType;

            byte[] lenghtBytes = BitConverter.GetBytes(serializedLength);
            serialized[2] = lenghtBytes[0];
            serialized[3] = lenghtBytes[1];
            serialized[4] = lenghtBytes[2];
            serialized[5] = lenghtBytes[3];

            Buffer.BlockCopy(tlsEnvelope.EncipheredPayload, 0, serialized, NOTLSEnvelope.HeaderLength, tlsEnvelope.EncipheredPayload.Length);

	        var crc32 = Crc32.Compute(serialized);
	        byte[] crc32Bytes = BitConverter.GetBytes(crc32);
	        serialized[6] = crc32Bytes[0];
	        serialized[7] = crc32Bytes[1];
	        serialized[8] = crc32Bytes[2];
	        serialized[9] = crc32Bytes[3];

			return serialized;
        }








        public static void UpdatePayload(int currentBytesRead, EnvelopeReaderBuffer readerBuffer)
        {
            // read all available Data
            if (readerBuffer.Payload == null) // first read, because readerBuffer.Payload is null
            {
                readerBuffer.Payload = new byte[currentBytesRead];
                Buffer.BlockCopy(readerBuffer.Buffer, 0, readerBuffer.Payload, 0, currentBytesRead);
            }
            else
            {
                var existingLenght = readerBuffer.Payload.Length;
                var newPayload = new byte[existingLenght + currentBytesRead];
                Buffer.BlockCopy(readerBuffer.Payload, 0, newPayload, 0, existingLenght);
                Buffer.BlockCopy(readerBuffer.Buffer, 0, newPayload, 0 + existingLenght, currentBytesRead);
                readerBuffer.Payload = newPayload;
            }
        }


        public static NOTLSEnvelope TryTakeOnePacket(ref byte[] readerPayload)
        {
            if (readerPayload.Length < NOTLSEnvelope.HeaderLength)
                return null;

            var advertisedLength = ExtractLenght(readerPayload);
            if (readerPayload.Length < advertisedLength)
                return null;

            // We appear to have enough data to extract one tlsPacket
            byte[] tlsPacket = new byte[advertisedLength];
            Buffer.BlockCopy(readerPayload, 0, tlsPacket, 0, advertisedLength);

            // calculate CRC32 but make no decisions.
            int actualCrc32;
            var crc32Success = ValidateCrc32(tlsPacket, out actualCrc32);


            var remainingLenght = readerPayload.Length - advertisedLength;
            if (remainingLenght == 0)
                readerPayload = null;
            else
            {
                var modifiedReaderPayLoad = new byte[remainingLenght];
                Buffer.BlockCopy(readerPayload, advertisedLength, modifiedReaderPayLoad, 0, modifiedReaderPayLoad.Length);
                readerPayload = modifiedReaderPayLoad;
            }
            return new NOTLSEnvelope(tlsPacket) { ActualCrc32 = actualCrc32, Crc32Success = crc32Success };
        }

        static int ExtractLenght(byte[] rawRequest)
        {
            return BitConverter.ToInt32(rawRequest, 2);
        }



        public static bool ValidateCrc32(this byte[] tlsPacketBytes, out int actualCrc32)
        {
            int adbvertisedCrc32 = BitConverter.ToInt32(tlsPacketBytes, 6);

            // The bytes for the crc in the message must be zero, as they were in the original calculation.
            tlsPacketBytes[6] = 0;
            tlsPacketBytes[7] = 0;
            tlsPacketBytes[8] = 0;
            tlsPacketBytes[9] = 0;

            // Calculate
            actualCrc32 = Crc32.Compute(tlsPacketBytes);

            // Set tlsPacketBytes back to original state.
            byte[] crc32Bytes = BitConverter.GetBytes(adbvertisedCrc32);
            tlsPacketBytes[6] = crc32Bytes[0];
            tlsPacketBytes[7] = crc32Bytes[1];
            tlsPacketBytes[8] = crc32Bytes[2];
            tlsPacketBytes[9] = crc32Bytes[3];

            return adbvertisedCrc32.Equals(actualCrc32);
        }


    }
}