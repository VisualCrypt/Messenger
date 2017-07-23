using System;
using System.Threading;
using System.Threading.Tasks;
using Obsidian.Common;
using Obsidian.Cryptography.Api.Implementations;
using Obsidian.Cryptography.Api.Interfaces;
using Obsidian.Cryptography.NetStandard;
using Obsidian.MessageNode.Core.Controllers;

namespace Obsidian.MessageNode.Core.Server
{
    public static class MessageNode
    {
	    public static readonly CancellationTokenSource CTS = new CancellationTokenSource();
        static IRequestHandler _requestHandler;

	    public static void RunWithTLS()
	    {
		    if(_requestHandler!= null)
				throw new InvalidOperationException("Already running");
		    IVisualCrypt2Service visualCrypt2Service = new VisualCrypt2Service();
		    visualCrypt2Service.Init(new Platform_NetStandard(),nameof(MessageNode));
		    _requestHandler = new TLSServerRequestHandler(nameof(MessageNode), visualCrypt2Service);

		    var tcpServer = new TcpAsyncServer(_requestHandler);
		    tcpServer.Run();
		    Task.Run(() => RunUdpServer(CTS.Token), CTS.Token);
		}

	    public static void RunWithoutTLS()
	    {
		    if (_requestHandler != null)
			    throw new InvalidOperationException("Already running");
		  
		    _requestHandler = new NoTLSServerRequestHandler(nameof(MessageNode));

		    var tcpServer = new TcpAsyncServer(_requestHandler);
		    tcpServer.Run();
		    Task.Run(() => RunUdpServer(CTS.Token), CTS.Token);
	    }

		static void RunUdpServer(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
				try
				{
					var udpServer = new UdpServer(_requestHandler);
					udpServer.Run(cancellationToken).Wait(cancellationToken);
				}
				catch (Exception e)
				{
					Log.Error($"UdpServer blew up: {e.Message}");
				}
			}
        }
    }
}
