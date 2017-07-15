using System;
using VisualCrypt.Applications.Models.Chat;
using VisualCrypt.Common;

namespace VisualCrypt.Applications.Models.Serialization
{
    public static class ProfileSerializer
    {
        public static byte[] SerializeCore(Profile profile)
        {
            byte[] serialized = PocoSerializer.Begin()
                .Append(profile.Id)
                .Append(profile.Name)
                .Append(profile.PublicKey)
                .Append(profile.PrivateKey)
                .Append(profile.IsIdentityPublished)
                .Append(profile.EncryptedProfileImage)
                .Finish();
            return serialized;
        }

        public static Profile Deserialize(byte[] serializedProfile)
        {
            if (serializedProfile == null)
                return null;
            try
            {
                var profile = new Profile();

                var ser = PocoSerializer.GetDeserializer(serializedProfile);

                profile.Id = ser.MakeString(0);
                profile.Name = ser.MakeString(1);
                profile.PublicKey = ser.MakeByteArray(2);
                profile.PrivateKey = ser.MakeByteArray(3);
                profile.IsIdentityPublished = ser.MakeBoolean(4);
                profile.EncryptedProfileImage = ser.MakeByteArray(5);
                return profile;
            }
            catch (Exception)
            {
                return null;
            }

        }
    }
}
