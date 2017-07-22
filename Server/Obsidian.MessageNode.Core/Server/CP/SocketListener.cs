using System;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using Obsidian.Common;
using Obsidian.Cryptography.Api.Implementations;
using Obsidian.Cryptography.NetStandard;
using Obsidian.Cryptography.TLS;
using Obsidian.MessageNode.Core.Controllers;

namespace Obsidian.MessageNode.Core.Server.CP
{
    /// <summary>
    /// Based on example from http://msdn2.microsoft.com/en-us/library/system.net.sockets.socketasynceventargs.aspx
    /// Implements the connection logic for the socket server.  
    /// After accepting a connection, all data read from the client is sent back. 
    /// The read and echo back to the client pattern is continued until the client disconnects.
    /// </summary>
    internal sealed class SocketListener
    {
	    readonly IRequestHandler _requestHandler;

		/// <summary>
		/// The socket used to listen for incoming connection requests.
		/// </summary>
		private Socket listenSocket;

        /// <summary>
        /// Mutex to synchronize server execution.
        /// </summary>
        private static Mutex mutex = new Mutex();

        /// <summary>
        /// Buffer size to use for each socket I/O operation.
        /// </summary>
        private Int32 bufferSize;

        /// <summary>
        /// The total number of clients connected to the server.
        /// </summary>
        private Int32 numConnectedSockets;

        /// <summary>
        /// the maximum number of connections the sample is designed to handle simultaneously.
        /// </summary>
        private Int32 numConnections;

        /// <summary>
        /// Pool of reusable SocketAsyncEventArgs objects for write, read and accept socket operations.
        /// </summary>
        private SocketAsyncEventArgsPool readWritePool;

        /// <summary>
        /// Controls the total number of clients connected to the server.
        /// </summary>
        private Semaphore semaphoreAcceptedClients;

        /// <summary>
        /// Create an uninitialized server instance.  
        /// To start the server listening for connection requests
        /// call the Init method followed by Start method.
        /// </summary>
        /// <param name="numConnections">Maximum number of connections to be handled simultaneously.</param>
        /// <param name="bufferSize">Buffer size to use for each socket I/O operation.</param>
        internal SocketListener(Int32 numConnections, Int32 bufferSize)
        {
            this.numConnectedSockets = 0;
            this.numConnections = numConnections;
            this.bufferSize = bufferSize;

            this.readWritePool = new SocketAsyncEventArgsPool(numConnections);
            this.semaphoreAcceptedClients = new Semaphore(numConnections, numConnections);

            // Preallocate pool of SocketAsyncEventArgs objects.
            for (Int32 i = 0; i < this.numConnections; i++)
            {
                SocketAsyncEventArgs readWriteEventArg = new SocketAsyncEventArgs();
				var reader = new SecureTcpReaderBuffer(null /* null because we don't have a socket yet */);
	            readWriteEventArg.UserToken = reader;
                readWriteEventArg.Completed += new EventHandler<SocketAsyncEventArgs>(OnIOCompleted);
                readWriteEventArg.SetBuffer(reader.Buffer, 0, this.bufferSize);

                // Add SocketAsyncEventArg to the pool.
                this.readWritePool.Push(readWriteEventArg);
            }
	        var _visualCrypt2Service = new VisualCrypt2Service();
	        _visualCrypt2Service.Init(new Platform_NetStandard(), "testserver");
	        _requestHandler = new ServerRequestHandler("testserver", _visualCrypt2Service);
		}

        /// <summary>
        /// Close the socket associated with the client.
        /// </summary>
        /// <param name="e">SocketAsyncEventArg associated with the completed send/receive operation.</param>
        private void CloseClientSocket(SocketAsyncEventArgs e)
        {
            //Token token = e.UserToken as Token;
			SecureTcpReaderBuffer token = e.UserToken as SecureTcpReaderBuffer;
            this.CloseClientSocket(token, e);
        }

		//  private void CloseClientSocket(Token token, SocketAsyncEventArgs e)
		private void CloseClientSocket(SecureTcpReaderBuffer token, SocketAsyncEventArgs e)
        {
	        try
	        {
		        token.Client.Dispose();
	        }
	        catch (Exception ex)
	        {
		        Debug.WriteLine(ex);
	        }
           

            // Decrement the counter keeping track of the total number of clients connected to the server.
            this.semaphoreAcceptedClients.Release();
            Interlocked.Decrement(ref this.numConnectedSockets);
	        Debug.WriteLine("A client has been disconnected from the server. There are {0} clients connected to the server", this.numConnectedSockets);

            // Free the SocketAsyncEventArg so they can be reused by another client.
            this.readWritePool.Push(e);
        }

        /// <summary>
        /// Callback method associated with Socket.AcceptAsync 
        /// operations and is invoked when an accept operation is complete.
        /// </summary>
        /// <param name="sender">Object who raised the event.</param>
        /// <param name="e">SocketAsyncEventArg associated with the completed accept operation.</param>
        private void OnAcceptCompleted(object sender, SocketAsyncEventArgs e)
        {
            this.ProcessAccept(e);
        }

        /// <summary>
        /// Callback called whenever a receive or send operation is completed on a socket.
        /// </summary>
        /// <param name="sender">Object who raised the event.</param>
        /// <param name="e">SocketAsyncEventArg associated with the completed send/receive operation.</param>
        private void OnIOCompleted(object sender, SocketAsyncEventArgs e)
        {
            // Determine which type of operation just completed and call the associated handler.
            switch (e.LastOperation)
            {
                case SocketAsyncOperation.Receive:
                    this.ProcessReceive(e);
                    break;
                case SocketAsyncOperation.Send:
                    this.ProcessSend(e);
                    break;
                default:
                    throw new ArgumentException("The last operation completed on the socket was not a receive or send");
            }
        }

        /// <summary>
        /// Process the accept for the socket listener.
        /// </summary>
        /// <param name="e">SocketAsyncEventArg associated with the completed accept operation.</param>
        private void ProcessAccept(SocketAsyncEventArgs e)
        {
           
            if (e.AcceptSocket.Connected)
            {
                try
                {
                    SocketAsyncEventArgs readEventArgs = this.readWritePool.Pop(); // contains already a SecureTcpReaderBuffer
                    if (readEventArgs != null)
                    {
                        // Get the socket for the accepted client connection and put it into the 
                        // ReadEventArg object user token.
	                    ((SecureTcpReaderBuffer)readEventArgs.UserToken).Client = e.AcceptSocket;
						
                        // readEventArgs.UserToken = new Token(s, this.bufferSize);
						// readEventArgs.UserToken = new SecureTcpReaderBuffer(s);

                        Interlocked.Increment(ref this.numConnectedSockets);
                        Debug.WriteLine("Client connection accepted. There are {0} clients connected to the server",
                            this.numConnectedSockets);

                        if (!e.AcceptSocket.ReceiveAsync(readEventArgs))
                        {
                            this.ProcessReceive(readEventArgs);
                        }
                    }
                    else
                    {
	                    Debug.WriteLine("There are no more available sockets to allocate.");
                    }
                }
                catch (SocketException ex)
                {
	                SecureTcpReaderBuffer token = e.UserToken as SecureTcpReaderBuffer;
	                Debug.WriteLine("Error when processing data received from {0}:\r\n{1}", token.Client.RemoteEndPoint, ex);
                }
                catch (Exception ex)
                {
	                Debug.WriteLine(ex.ToString());
                }

                // Accept the next connection request.
                this.StartAccept(e);
            }
        }

        private void ProcessError(SocketAsyncEventArgs e)
        {
            // Token token = e.UserToken as Token;
			SecureTcpReaderBuffer token = e.UserToken as SecureTcpReaderBuffer;
            IPEndPoint localEp = token.Client.LocalEndPoint as IPEndPoint;

            this.CloseClientSocket(token, e);

	        Debug.WriteLine("Socket error {0} on endpoint {1} during {2}.", (Int32)e.SocketError, localEp, e.LastOperation);
        }

        /// <summary>
        /// This method is invoked when an asynchronous receive operation completes. 
        /// If the remote host closed the connection, then the socket is closed.  
        /// If data was received then the data is echoed back to the client.
        /// </summary>
        /// <param name="e">SocketAsyncEventArg associated with the completed receive operation.</param>
        async void ProcessReceive(SocketAsyncEventArgs e)
        {
	        var stopWatch = new Stopwatch();
	        stopWatch.Start();

			// Check if the remote host closed the connection.
			if (e.BytesTransferred > 0)
            {
                if (e.SocketError == SocketError.Success)
                {
	                try
	                {
						//Token token = e.UserToken as Token;
						var reader = (SecureTcpReaderBuffer)e.UserToken;
	              
	                //var currentBytesRead = reader.Client.Available;
	                var currentBytesRead = e.BytesTransferred;

					if (currentBytesRead == 0) // readerBuffer.Payload MUST stay null!
		                return;

	                TLSEnvelopeExtensions.UpdatePayload(currentBytesRead, reader);

	                do
	                {
		                var packet = TLSEnvelopeExtensions.TryTakeOnePacket(ref reader.Payload);
		                if (packet == null)
			                break;

		              
			                var reply = await _requestHandler.ProcessRequestAsync(packet.Serialize());
		               
		              


		                if (reply != null)
		                {
			                e.SetBuffer(reply, 0, reply.Length);
			                if (!reader.Client.SendAsync(e))
			                {
				                // Set the buffer to send back to the client.
				                this.ProcessSend(e);
			                }
							//Send(reader.Client, reply);
						}
			              
		                var sent = reply == null ? "nothing" : $"{reply.Length} bytes";
		                Log.Info($"TcpServer received {packet.Serialize().Length} bytes, sent {sent}, {stopWatch.ElapsedMilliseconds}ms, {reader.Client.RemoteEndPoint}.");
	                } while (reader.Payload != null);

					//            reader.Client.BeginReceive(reader.Buffer, 0, SecureTcpReaderBuffer.BufferSize, 0, ReadCallback, reader);

					//if (s.Available == 0) // fertig gelesen
					//               {
					//                   // Set return buffer.
					//                   reader.ProcessData(e);
					//                   if (!s.SendAsync(e))
					//                   {
					//                       // Set the buffer to send back to the client.
					//                       this.ProcessSend(e);
					//                   }
					//               }
					//               else 
	                await Task.Delay(5000);
					if (!reader.Client.ReceiveAsync(e))
					{
						// Read the next block of data sent by client.
						this.ProcessReceive(e);
					}
	                }
	                catch (Exception ex)
	                {
						Debug.WriteLine(ex.Message);
						ProcessError(e);
	                }
				}
                else
                {
                    this.ProcessError(e);
                }
            }
            else
            {
                this.CloseClientSocket(e);
            }
        }

        /// <summary>
        /// This method is invoked when an asynchronous send operation completes.  
        /// The method issues another receive on the socket to read any additional 
        /// data sent from the client.
        /// </summary>
        /// <param name="e">SocketAsyncEventArg associated with the completed send operation.</param>
        private void ProcessSend(SocketAsyncEventArgs e)
        {
            if (e.SocketError == SocketError.Success)
            {
                // Done echoing data back to the client.
                //Token token = e.UserToken as Token;
				SecureTcpReaderBuffer token = e.UserToken as SecureTcpReaderBuffer;

                if (!token.Client.ReceiveAsync(e))
                {
                    // Read the next block of data send from the client.
                    this.ProcessReceive(e);
                }
            }
            else
            {
                this.ProcessError(e);
            }
        }

        /// <summary>
        /// Starts the server listening for incoming connection requests.
        /// </summary>
        /// <param name="port">Port where the server will listen for connection requests.</param>
        internal void Start(Int32 port)
        {
            // Get host related information.
	        IPAddress[] addressList = Dns.GetHostAddressesAsync("localhost").Result;

			// Get endpoint for the listener.
			//IPEndPoint localEndPoint = new IPEndPoint(addressList[addressList.Length - 1], port);
			IPEndPoint localEndPoint = new IPEndPoint(IPAddress.Any, port);

			// Create the socket which listens for incoming connections.
			this.listenSocket = new Socket(localEndPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            this.listenSocket.ReceiveBufferSize = this.bufferSize;
            this.listenSocket.SendBufferSize = this.bufferSize;

            if (localEndPoint.AddressFamily == AddressFamily.InterNetworkV6)
            {
                // Set dual-mode (IPv4 & IPv6) for the socket listener.
                // 27 is equivalent to IPV6_V6ONLY socket option in the winsock snippet below,
                // based on http://blogs.msdn.com/wndp/archive/2006/10/24/creating-ip-agnostic-applications-part-2-dual-mode-sockets.aspx
                this.listenSocket.SetSocketOption(SocketOptionLevel.IPv6, (SocketOptionName)27, false);
                this.listenSocket.Bind(new IPEndPoint(IPAddress.IPv6Any, localEndPoint.Port));
            }
            else
            {
                // Associate the socket with the local endpoint.
                this.listenSocket.Bind(localEndPoint);
            }

            // Start the server.
            this.listenSocket.Listen(this.numConnections);

            // Post accepts on the listening socket.
            this.StartAccept(null);

            // Blocks the current thread to receive incoming messages.
            mutex.WaitOne();
        }

        /// <summary>
        /// Begins an operation to accept a connection request from the client.
        /// </summary>
        /// <param name="acceptEventArg">The context object to use when issuing 
        /// the accept operation on the server's listening socket.</param>
        private void StartAccept(SocketAsyncEventArgs acceptEventArg)
        {
            if (acceptEventArg == null)
            {
                acceptEventArg = new SocketAsyncEventArgs();
                acceptEventArg.Completed += new EventHandler<SocketAsyncEventArgs>(OnAcceptCompleted);
            }
            else
            {
                // Socket must be cleared since the context object is being reused.
                acceptEventArg.AcceptSocket = null;
            }

            this.semaphoreAcceptedClients.WaitOne();
            if (!this.listenSocket.AcceptAsync(acceptEventArg))
            {
                this.ProcessAccept(acceptEventArg);
            }
        }

        /// <summary>
        /// Stop the server.
        /// </summary>
        internal void Stop()
        {
            this.listenSocket.Dispose();
            mutex.ReleaseMutex();
        }
    }
}
