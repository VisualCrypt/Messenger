using System.Collections.Generic;
using System.Threading.Tasks;
using Obsidian.Cryptography.Api.Infrastructure;
using Obsidian.Cryptography.TLS;

namespace Obsidian.Common
{
    public interface INetworkClient
    {
        Task<Response<List<TLSRequest>>> SendRequestAsync(byte[] request, Transport transport);
        Task<string> Receive(byte[] rawRequest, Transport transport);
    }
}
