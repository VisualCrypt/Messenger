using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Obsidian.Cryptography.Api.Infrastructure;
using Obsidian.Cryptography.TLS;

namespace Obsidian.Common
{
    public interface IConnection
    {
        bool IsConnected { get; }

        Task<bool> ConnectAsync(string remoteDnsHost, int remotePort, Func<byte[], Transport, Task<string>> receiver = null);

        Task DisconnectAsync();

        Task<Response<List<TLSEnvelope>>> SendRequestAsync(byte[] request);

    }
}
