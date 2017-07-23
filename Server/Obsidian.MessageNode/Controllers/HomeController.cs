using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Obsidian.MessageNode.Core.Server;

namespace Obsidian.MessageNode.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

	    public IActionResult Connections()
	    {
		    ViewData["NumConnectedSockets"] = TcpAsyncServer.NumConnectedSockets;
		    return View();
	    }

	    public IActionResult Log(string id)
	    {
			ViewData["NumConnectedSockets"] = TcpAsyncServer.NumConnectedSockets;
		    ViewData["Logs"] = Core.Server.Log.LogEntries;
		    return View();
	    }

		

		public IActionResult About()
        {
            ViewData["Message"] = "Your application description page.";

            return View();
        }

        public IActionResult Contact()
        {
            ViewData["Message"] = "Your contact page.";

            return View();
        }

        public IActionResult Error()
        {
            return View();
        }
    }
}
