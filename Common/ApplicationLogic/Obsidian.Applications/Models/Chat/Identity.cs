using System;
using Obsidian.Common;

namespace Obsidian.Applications.Models.Chat
{
    public class Identity : IId
    {
        #region IId

        public string Id { get; set; }

        #endregion

        public string Name { get; set; }
        public byte[] Image { get; set; }
        public DateTime LastSeenUtc { get; set; }
        public DateTime FirstSeenUtc { get; set; }
        public byte[] StaticPublicKey { get; set; }
        public ContactState ContactState { get; set; }
        public byte[] CryptographicInformation { get; set; }



    }
}

