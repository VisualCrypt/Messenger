using System;
using Obsidian.Applications.Models.Chat;
using Obsidian.Common;

namespace Obsidian.Applications.Models.Serialization
{
    public static class MessageSerializer
    {
        public static byte[] SerializeCore(Message m)
        {
            byte[] serialized = PocoSerializer.Begin()
                // XMessage members
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
                // Message members
                .Append(m.YourId)
                .Append((byte)m.Side)
                .Append((byte)m.PrevSide)
                .Append((byte)m.SendMessageState)
                .Append((byte)m.LocalMessageState)
                .Append(m.EncryptedE2EEncryptionKey)
                // some members are ignored!
                .Finish();
            return serialized;
        }

        public static Message Deserialize(byte[] serializedMessage)
        {
            if (serializedMessage == null)
                return null;

            try
            {
                var m = new Message();

                var ser = PocoSerializer.GetDeserializer(serializedMessage);

                // XMessage members
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
                // Message members
                m.YourId = ser.MakeString(10);
                m.Side = (MessageSide)ser.MakeByte(11);
                m.PrevSide = (MessageSide)ser.MakeByte(12);
                m.SendMessageState = (SendMessageState)ser.MakeByte(13);
                m.LocalMessageState = (LocalMessageState)ser.MakeByte(14);
                m.EncryptedE2EEncryptionKey = ser.MakeByteArray(15);

                return m;
            }
            catch (Exception)
            {
                return null;
            }

        }
    }
}
