using System.IO;
using Microsoft.AspNetCore.Hosting;

namespace Obsidian.MessageNode
{
    public class Program
	{ 
        public static void Main(string[] args)
        {
	        Core.Server.Log.EntryWritten += Core.Server.Log.Log_EntryWritten;
			Core.MessageNode.Run();

			var host = new WebHostBuilder()
                .UseKestrel()
                .UseContentRoot(Directory.GetCurrentDirectory())
                .UseIISIntegration()
                .UseStartup<Startup>()
                .UseApplicationInsights()
                .Build();
	        host.Run();

			
        }
    }
}
