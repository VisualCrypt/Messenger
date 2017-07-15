using Obsidian.Cryptography.Api.Infrastructure;

namespace Obsidian.Common
{
    public static class XMessageExtensions
    {
        public static bool EqualDeep(this XMessage m1, XMessage m2)
        {
            if (ReferenceEquals(m1, m2))
                return true;

            if (m1 == null || m2 == null)
                return false;
            if (m1.Id != m2.Id)
                return false;
            if (m1.SenderId != m2.SenderId)
                return false;
            if (m1.RecipientId != m2.RecipientId)
                return false;
            if (m1.MessageType != m2.MessageType)
                return false;
            if (m1.EncryptedDateUtc != m2.EncryptedDateUtc)
                return false;
            if (m1.TextCipher != m2.TextCipher)
                return false;
            if (m1.ImageCipher != m2.ImageCipher)
                return false;
            if (m1.DynamicPublicKey != m2.DynamicPublicKey)
                return false;
            if (m1.DynamicPublicKeyId != m2.DynamicPublicKeyId)
                return false;
            if (m1.PrivateKeyHint != m2.PrivateKeyHint)
                return false;

            if (!ByteArrays.AreAllBytesEqualOrBothNull(m1.DynamicPublicKey, m2.DynamicPublicKey))
                return false;

            if (!ByteArrays.AreAllBytesEqualOrBothNull(m1.TextCipher, m2.TextCipher))
                return false;

            if (!ByteArrays.AreAllBytesEqualOrBothNull(m1.ImageCipher, m2.ImageCipher))
                return false;

            return true;
        }

       
        public static byte[] SerializeCore(this XMessage m)
        {
            byte[] serialized = PocoSerializer.Begin()
                .Append(m.Id)
                .Append(m.SenderId)
                .Append(m.RecipientId)
                .Append((byte)m.MessageType)
                .Append(m.EncryptedDateUtc)
                .Append(m.TextCipher)
                .Append(m.ImageCipher)
                .Append(m.DynamicPublicKey)
                .Append(m.DynamicPublicKeyId)
                .Append(m.PrivateKeyHint)
                .Finish();
            return serialized;
        }

       

        public static XMessage DeserializeMessage(this byte[] message)
        {
            var m = new XMessage();

            var ser = PocoSerializer.GetDeserializer(message);

            m.Id = ser.MakeString(0);
            m.SenderId = ser.MakeString(1);
            m.RecipientId = ser.MakeString(2);
            m.MessageType = (MessageType)ser.MakeByte(3);
            m.EncryptedDateUtc = ser.MakeDateTime(4);
            m.TextCipher = ser.MakeByteArray(5);
            m.ImageCipher = ser.MakeByteArray(6);
            m.DynamicPublicKey = ser.MakeByteArray(7);
            m.DynamicPublicKeyId = ser.MakeInt64(8);
            m.PrivateKeyHint = ser.MakeInt64(9);

            return m;
        }

       
    }
}
