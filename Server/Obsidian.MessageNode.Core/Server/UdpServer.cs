using System;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using Obsidian.Common;

namespace Obsidian.MessageNode.Core.Server
{
    class UdpServer
    {
        readonly UdpClient _udpClient;
        readonly IRequestHandler _requestHandler;
        IPEndPoint _ipEndPoint;

        public UdpServer(IRequestHandler serverRequestHandler)
        {
            _udpClient = new UdpClient(Config.UdpServerPort);
            Log.Info($"Default ReceiveBuffer size is {_udpClient.Client.ReceiveBufferSize}");
            _udpClient.Client.ReceiveBufferSize = Config.UdpReceiveBufferSize;

            _ipEndPoint = new IPEndPoint(IPAddress.Any, Config.UdpServerPort);
            _requestHandler = serverRequestHandler;
        }

        public async Task Run(CancellationToken ct)
        {
            var stopwatch = new Stopwatch();
            while (!ct.IsCancellationRequested)
            {
                try
                {
	                var packet = await _udpClient.ReceiveAsync();
                    //var packet = _udpClient.Receive(ref _ipEndPoint);
                    stopwatch.Restart();
                   
                    var reply = await _requestHandler.ProcessRequestAsync(packet.Buffer);
                    if (reply == null)
                        continue;

                    await Send(reply);
                    Log.Info($"UdpServer received {packet.Buffer.Length} bytes, sent {reply?.Length} bytes, {stopwatch.ElapsedMilliseconds}ms, {_ipEndPoint.Address}:{_ipEndPoint.Port}.");
                }
                catch (Exception e)
                {
                    Log.Error("UdpServer.Run: BOOOM!" + e.Message);
                }
            }
        }

        async Task Send(byte[] reply)
        {
            int bytesSent = await _udpClient.SendAsync(reply, reply.Length, _ipEndPoint);
        }
    }
}
