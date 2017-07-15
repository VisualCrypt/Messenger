using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Obsidian.MessageNode.Core;
using Obsidian.MessageNode.Core.Server.CP;

namespace Obsidian.MessageNode
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var host = new WebHostBuilder()
                .UseKestrel()
                .UseContentRoot(Directory.GetCurrentDirectory())
                .UseIISIntegration()
                .UseStartup<Startup>()
                .UseApplicationInsights()
                .Build();
			StartServer.Start();
			//var server = new VisualCryptMessageServer("testserver");
	  //      server.RunAsync(new CancellationTokenSource().Token);

            host.Run();
        }
    }
}
