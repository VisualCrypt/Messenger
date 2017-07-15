using System;
using VisualCrypt.Applications.Models.Chat;
using VisualCrypt.Common;
using VisualCrypt.Cryptography.E2E;

namespace VisualCrypt.Applications.Models.Serialization
{
    public static class RepositorySerializer
    {
        public static byte[] Serialize<T>(T item) where T : class
        {
            if (item == null)
                return null;

            if (typeof(T) == typeof(Message))
                return MessageSerializer.SerializeCore(item as Message);

            if (typeof(T) == typeof(Identity))
                return IdentitySerializer.SerializeCore(item as Identity);

            if (typeof(T) == typeof(Profile))
                return ProfileSerializer.SerializeCore(item as Profile);
           
            throw new Exception();
        }

        public static T Deserialize<T>(byte[] data) where T : class
        {
            if (typeof(T) == typeof(Message))
                return MessageSerializer.Deserialize(data) as T; 

            if (typeof(T) == typeof(Profile))
                return ProfileSerializer.Deserialize(data) as T;
            
            if (typeof(T) == typeof(Identity))
                return IdentitySerializer.Deserialize(data) as T;
            
            throw new Exception();
        }
    }
}
