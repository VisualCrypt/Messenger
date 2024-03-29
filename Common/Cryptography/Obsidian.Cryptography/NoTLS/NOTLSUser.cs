﻿using System.Collections.Generic;
using Obsidian.Cryptography.ECC;

namespace Obsidian.Cryptography.NoTLS
{
    public class NOTLSUser
    {
        public NOTLSUser(string  userId, byte[] staticPublicKey)
        {
            UserId = userId;
            StaticPublicKey = staticPublicKey;
            DynamicPrivateDecryptionKeys = new Dictionary<long, byte[]>();
        }
        public string UserId { get;}
        public byte[] StaticPublicKey { get; }

        public byte[] LatestDynamicPublicKey { get; set; }
        public long LatestDynamicPublicKeyId { get; set; }
        public byte[] AuthSecret { get; set; }
        public DynamicSecret DynamicSecret { get; set; }
        public int PushesDone { get; set; }
        public  Dictionary<long, byte[]> DynamicPrivateDecryptionKeys { get; } 
    }

    
}
