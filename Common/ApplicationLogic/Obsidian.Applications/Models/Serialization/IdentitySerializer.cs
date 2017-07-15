using System;
using Obsidian.Applications.Models.Chat;
using Obsidian.Common;

namespace Obsidian.Applications.Models.Serialization
{
    public static class IdentitySerializer
    {
        public static byte[] SerializeCore(Identity identity)
        {
            byte[] serialized = PocoSerializer.Begin()
                .Append(identity.Id)
                .Append(identity.Name)
                .Append(identity.Image)
                .Append(identity.LastSeenUtc)
                .Append(identity.FirstSeenUtc)
                .Append(identity.StaticPublicKey)
                .Append((byte)identity.ContactState)
                .Append(identity.CryptographicInformation)
                .Finish();
            return serialized;
        }

        public static Identity Deserialize(byte[] serializedIdentity)
        {
            if (serializedIdentity == null)
                return null;
            try
            {
                var identity = new Identity();

                var ser = PocoSerializer.GetDeserializer(serializedIdentity);

                identity.Id = ser.MakeString(0);
                identity.Name = ser.MakeString(1);
                identity.Image = ser.MakeByteArray(2);
                identity.LastSeenUtc = ser.MakeDateTime(3);
                identity.FirstSeenUtc = ser.MakeDateTime(4);
                identity.StaticPublicKey = ser.MakeByteArray(5);
                identity.ContactState = (ContactState) ser.MakeByte(6);
                identity.CryptographicInformation = ser.MakeByteArray(7);

                return identity;
            }
            catch (Exception)
            {
                return null;
            }

        }
    }
}
