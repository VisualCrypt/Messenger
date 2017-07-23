﻿namespace Obsidian.Cryptography.NoTLS
{
    public class NOTLSRequest : IRequestCommandData
	{
        public string UserId;
        public byte[] CommandData { get; set; }
        public bool IsAuthenticated;
    }
}
