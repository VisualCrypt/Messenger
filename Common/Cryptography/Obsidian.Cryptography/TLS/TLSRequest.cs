using Obsidian.Cryptography.NoTLS;

namespace Obsidian.Cryptography.TLS
{
    public class TLSRequest : IRequestCommandData
	{
        public string UserId;
        public byte[] CommandData { get; set; }
        public bool IsAuthenticated;
    }
}
