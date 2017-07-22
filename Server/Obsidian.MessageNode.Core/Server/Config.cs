namespace Obsidian.MessageNode.Core.Server
{
    static class Config
    {
        /// <summary>
        /// Make sure you open all server ports in ServiceDefinition.csdef.
        /// </summary>
        public const int UdpServerPort = 55555;
        public const int UdpReceiveBufferSize = 1024*1024*100;

        public const int TcpServerPort = 55556;
        public const int TcpListenerBacklog = 100;
        public const ushort TcpReaderBufferSize = 4096*2;
	    public const int TcpMaxConnections = 1000;

        public static void Configure()
        {
            //ServicePointManager.DefaultConnectionLimit = 100000;
            //ServicePointManager.UseNagleAlgorithm = false;
            //ServicePointManager.Expect100Continue = false;
            //ServicePointManager.CheckCertificateRevocationList = false;
           
        }
    }
}
