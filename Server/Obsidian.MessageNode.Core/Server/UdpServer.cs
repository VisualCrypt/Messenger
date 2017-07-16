using System;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using Obsidian.Common;
using Obsidian.Cryptography.Api.Implementations;
using Obsidian.Cryptography.NetStandard;
using Obsidian.MessageNode.Core.Controllers;

namespace Obsidian.MessageNode.Core.Server
{
    public class UdpServer
    {
        readonly UdpClient _udpClient;
       
        IPEndPoint _ipEndPoint;

        public UdpServer()
        {
            _udpClient = new UdpClient(Config.UdpServerPort);
            Log.Info($"Default ReceiveBuffer size is {_udpClient.Client.ReceiveBufferSize}");
            _udpClient.Client.ReceiveBufferSize = Config.UdpReceiveBufferSize;

            _ipEndPoint = new IPEndPoint(IPAddress.Any, Config.UdpServerPort);
           
        }

        public async Task Run(CancellationToken ct)
        {
            var stopwatch = new Stopwatch();
            while (!ct.IsCancellationRequested)
            {
                try
                {
	                var udpReceiveResult = await _udpClient.ReceiveAsync();
                    //var packet = _udpClient.Receive(ref _ipEndPoint);
                    stopwatch.Restart();
	                const string info = "UDPHandler";
					var vcs = new VisualCrypt2Service();
					vcs.Init(new Platform_NetStandard(),info);
					var requestHandler = new ServerRequestHandler(info, vcs);
                    var reply = await requestHandler.ProcessRequestAsync(udpReceiveResult.Buffer);
                    if (reply == null)
                        continue;

                    await Send(reply,udpReceiveResult.RemoteEndPoint);
                    Log.Info($"UdpServer received {udpReceiveResult.Buffer.Length} bytes, sent {reply?.Length} bytes, {stopwatch.ElapsedMilliseconds}ms, {_ipEndPoint.Address}:{_ipEndPoint.Port}.");
                }
                catch (Exception e)
                {
                    Log.Error("UdpServer.Run: BOOOM!" + e.Message);
                }
            }
        }

        async Task Send(byte[] reply, IPEndPoint remoteEndPoint)
        {
            int bytesSent = await _udpClient.SendAsync(reply, reply.Length, remoteEndPoint);
        }
    }
}
