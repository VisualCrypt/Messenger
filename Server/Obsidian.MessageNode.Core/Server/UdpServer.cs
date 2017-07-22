using System;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using Obsidian.Common;

namespace Obsidian.MessageNode.Core.Server
{
    public class UdpServer
    {
		readonly UdpClient _udpClient;
	    readonly IRequestHandler _requestHandler;
	    readonly IPEndPoint _ipEndPoint;

		public UdpServer(IRequestHandler requestHandler)
        {
			_udpClient = new UdpClient(Config.UdpServerPort);
	        Log.Info($"Default ReceiveBuffer size is {_udpClient.Client.ReceiveBufferSize}");
	        _udpClient.Client.ReceiveBufferSize = Config.UdpReceiveBufferSize;

	        _ipEndPoint = new IPEndPoint(IPAddress.Any, Config.UdpServerPort);
	        _requestHandler = requestHandler;

		}

        public async Task Run(CancellationToken ct)
        {
            var stopwatch = new Stopwatch();
            while (!ct.IsCancellationRequested)
            {
                try
                {
	                var udpReceiveResult = await _udpClient.ReceiveAsync();
                    stopwatch.Restart();
	              
                    var reply = await _requestHandler.ProcessRequestAsync(udpReceiveResult.Buffer);
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
