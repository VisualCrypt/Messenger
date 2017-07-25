using System.IO;
using Microsoft.AspNetCore.Hosting;

namespace Obsidian.MessageNode
{
    public class Program
	{ 
        public static void Main(string[] args)
        {
	        Core.Server.Log.EntryWritten += Core.Server.Log.Log_EntryWritten;
			Core.Server.MessageNode.RunWithoutTLS();

			var host = new WebHostBuilder()
                .UseKestrel()
				.UseUrls("http://*:5000/")
                .UseContentRoot(Directory.GetCurrentDirectory())
               // .UseIISIntegration()
                .UseStartup<Startup>()
                //.UseApplicationInsights()
                .Build();
	        host.Run();

			
        }
    }
}
