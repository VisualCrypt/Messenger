using System;
using System.Diagnostics;

namespace Obsidian.MessageNode.Core.Server.CP
{
    public static class StartServer
    {
        const Int32 DEFAULT_PORT = 55556, 
			DEFAULT_NUM_CONNECTIONS = 10, 
			DEFAULT_BUFFER_SIZE = Int16.MaxValue;

        public static void Start()
        {
            try
            {
                

                SocketListener sl = new SocketListener(DEFAULT_NUM_CONNECTIONS, DEFAULT_BUFFER_SIZE);
                sl.Start(DEFAULT_PORT);

                Debug.WriteLine("Server listening on port {0}.", DEFAULT_PORT);
                //Console.Read();

                //sl.Stop();

            }
           
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

    }
}
