using System;
using System.Threading;
using System.Threading.Tasks;
using Obsidian.Common;
using Obsidian.Cryptography.Api.Implementations;
using Obsidian.Cryptography.Api.Interfaces;
using Obsidian.MessageNode.Core.Controllers;
using Obsidian.MessageNode.Core.Server;
using Obsidian.Cryptography.NetStandard;

namespace Obsidian.MessageNode.Core
{
    public class VisualCryptMessageServer
    {
        readonly IRequestHandler _serverRequestHandler;
        readonly IVisualCrypt2Service _visualCrypt2Service;
        public VisualCryptMessageServer(string name)
        {
            _visualCrypt2Service = new VisualCrypt2Service();
            _visualCrypt2Service.Init(new Platform_NetStandard(),  name);
            _serverRequestHandler = new ServerRequestHandler(name, _visualCrypt2Service);
        }

        public async Task RunAsync(CancellationToken cancellationToken)
        {
            Log.Info("Starting VisualCrypt Worker Role.");
            Config.Configure();
            var tcpTask = Task.Run(() => RunTcpServer(cancellationToken), cancellationToken);
            //var udpTask = Task.Run(() => RunUdpServer(cancellationToken), cancellationToken);
            //await Task.WhenAll(tcpTask, udpTask);
        }

        void RunTcpServer(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                try
                {
                    var tcpServer = new Sudo(_serverRequestHandler);
                    //tcpServer.Run(cancellationToken);
	                break;
                }
                catch (Exception e)
                {
                    Log.Error($"TcpServer blew up: {e.Message}");
                }
            }
        }

        void RunUdpServer(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                //try
                //{
                //    var udpServer = new UdpServer(_serverRequestHandler);
                //    udpServer.Run(cancellationToken).Wait(cancellationToken);
                //}
                //catch (Exception e)
                //{
                //    Log.Error("UdpServer blew up: {e.Message}");
                //}
            }
        }
    }
}
