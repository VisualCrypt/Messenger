using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using Obsidian.Common;
using Obsidian.Cryptography.TLS;

namespace Obsidian.MessageNode.Core.Server
{
	public class TcpAsyncServer
	{
		readonly IRequestHandler _requestHandler; // must be one instance, if you create one per request, the tls ratchet is always blank.
		Socket _listenSocket;
		public static  int NumConnectedSockets;
		readonly Semaphore _maxNumberAcceptedClients;
	
		public TcpAsyncServer(IRequestHandler requestHandler)
		{
			NumConnectedSockets = 0;
			_maxNumberAcceptedClients = new Semaphore(Config.TcpMaxConnections, Config.TcpMaxConnections);
			_requestHandler = requestHandler;
		}

		public void Run()
		{
			var endPoint = new IPEndPoint(IPAddress.Any, Config.TcpServerPort);
			_listenSocket = new Socket(endPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
			_listenSocket.Bind(endPoint);
			_listenSocket.Listen(100);
			StartAccept(null);
		}


		void StartAccept(SocketAsyncEventArgs acceptEventArg)
		{
			if (acceptEventArg == null)
			{
				acceptEventArg = new SocketAsyncEventArgs();
				acceptEventArg.Completed += new EventHandler<SocketAsyncEventArgs>(AcceptEventArg_Completed);
			}
			else
			{
				// socket must be cleared since the context object is being reused
				acceptEventArg.AcceptSocket = null;
			}

			_maxNumberAcceptedClients.WaitOne();
			bool willRaiseEvent = _listenSocket.AcceptAsync(acceptEventArg);
			if (!willRaiseEvent)
			{
				ProcessAccept(acceptEventArg);
			}
		}

		// This method is the callback method associated with Socket.AcceptAsync 
		// operations and is invoked when an accept operation is complete
		//
		void AcceptEventArg_Completed(object sender, SocketAsyncEventArgs e)
		{
			ProcessAccept(e);
		}

		void ProcessAccept(SocketAsyncEventArgs e)
		{
			Interlocked.Increment(ref NumConnectedSockets);
			Debug.WriteLine("Client connection accepted. There are {0} clients connected to the server",
				NumConnectedSockets);

			SocketAsyncEventArgs args = CreateArgs();
			((AsyncUserToken)args.UserToken).Socket = e.AcceptSocket;

			// As soon as the client is connected, post a receive to the connection
			bool willRaiseEvent = e.AcceptSocket.ReceiveAsync(args);
			if (!willRaiseEvent)
			{
				ProcessReceive(args);
			}

			// Accept the next connection request
			StartAccept(e);
		}

		void IO_Completed(object sender, SocketAsyncEventArgs e)
		{
			// determine which type of operation just completed and call the associated handler
			switch (e.LastOperation)
			{
				case SocketAsyncOperation.Receive:
					ProcessReceive(e);
					break;
				case SocketAsyncOperation.Send:
					ProcessSend(e);
					break;
				default:
					throw new ArgumentException("The last operation completed on the socket was not a receive or send");
			}

		}

		// This method is invoked when an asynchronous receive operation completes. 
		// If the remote host closed the connection, then the socket is closed.  
		// If data was received then the data is echoed back to the client.
		//
		void ProcessReceive(SocketAsyncEventArgs e)
		{
			AsyncUserToken token = (AsyncUserToken)e.UserToken;
			if (e.BytesTransferred > 0 && e.SocketError == SocketError.Success) 
			{
				TLSEnvelopeExtensions.UpdatePayload(e.BytesTransferred, token);

				do
				{
					var packet = TLSEnvelopeExtensions.TryTakeOnePacket(ref token.Payload);
					if (packet == null) // null -> noch nicht komplett
					{
						if (!token.Socket.ReceiveAsync(e))
							ProcessReceive(e);
						return;
					}
					
					var reply = _requestHandler.ProcessRequestAsync(packet.Serialize()).Result;
					Log.Info("reply created");
					if (reply != null)
					{
						SocketAsyncEventArgs sendArgs = CreateArgs();
						((AsyncUserToken)sendArgs.UserToken).Socket = token.Socket; 
						sendArgs.SetBuffer(reply,0,reply.Length);
						if (!token.Socket.SendAsync(sendArgs))
						{
							ProcessSend(sendArgs);
						}
					}
				} while (token.Payload != null);
			}
			else
			{
				CloseClientSocket(e);
			}
		}

		void ProcessSend(SocketAsyncEventArgs e)
		{
			if (e.SocketError == SocketError.Success)
			{
				// done echoing data back to the client
				AsyncUserToken token = (AsyncUserToken)e.UserToken;

				SocketAsyncEventArgs readArgs = CreateArgs();
				((AsyncUserToken)readArgs.UserToken).Socket = token.Socket; // copy the right socket
			
				// read the next block of data send from the client
				bool willRaiseEvent = token.Socket.ReceiveAsync(readArgs);
				if (!willRaiseEvent)
				{
					ProcessReceive(readArgs);
				}
			}
			else
			{
				CloseClientSocket(e);
			}
		}

		private void CloseClientSocket(SocketAsyncEventArgs e)
		{
			AsyncUserToken token = e.UserToken as AsyncUserToken;

			// close the socket associated with the client
			try
			{
				token.Socket.Shutdown(SocketShutdown.Send);
			}
			// throws if client process has already closed
			catch (Exception) { }
			token.Socket.Dispose();

			// decrement the counter keeping track of the total number of clients connected to the server
			Interlocked.Decrement(ref NumConnectedSockets);
			_maxNumberAcceptedClients.Release();
			Debug.WriteLine("A client has been disconnected from the server. There are {0} clients connected to the server", NumConnectedSockets);

			// Free the SocketAsyncEventArg so they can be reused by another client
			//m_readWritePool.Push(e);
		}

		SocketAsyncEventArgs CreateArgs()
		{
			var args = new SocketAsyncEventArgs();
			args.Completed += IO_Completed;
			var token = new AsyncUserToken();
			token.Buffer = new byte[Config.TcpReaderBufferSize];
			args.UserToken = token;
			args.SetBuffer(token.Buffer, 0, Config.TcpReaderBufferSize);
			return args;
		}

	}


	class AsyncUserToken : TLSEnvelopeReaderBuffer
	{
		internal Socket Socket;
	}

	// This class creates a single large buffer which can be divided up 
	// and assigned to SocketAsyncEventArgs objects for use with each 
	// socket I/O operation.  
	// This enables bufffers to be easily reused and guards against 
	// fragmenting heap memory.
	// 
	// The operations exposed on the BufferManager class are not thread safe.
	// https://msdn.microsoft.com/en-us/library/bb517542(v=vs.110).aspx
	class BufferManager
	{
		int m_numBytes;                 // the total number of bytes controlled by the buffer pool
		byte[] m_buffer;                // the underlying byte array maintained by the Buffer Manager
		Stack<int> m_freeIndexPool;     // 
		int m_currentIndex;
		int m_bufferSize;

		public BufferManager(int totalBytes, int bufferSize)
		{
			m_numBytes = totalBytes;
			m_currentIndex = 0;
			m_bufferSize = bufferSize;
			m_freeIndexPool = new Stack<int>();
		}

		// Allocates buffer space used by the buffer pool
		public void InitBuffer()
		{
			// create one big large buffer and divide that 
			// out to each SocketAsyncEventArg object
			m_buffer = new byte[m_numBytes];
		}

		// Assigns a buffer from the buffer pool to the 
		// specified SocketAsyncEventArgs object
		//
		// <returns>true if the buffer was successfully set, else false</returns>
		public bool SetBuffer(SocketAsyncEventArgs args)
		{

			if (m_freeIndexPool.Count > 0)
			{
				args.SetBuffer(m_buffer, m_freeIndexPool.Pop(), m_bufferSize);
			}
			else
			{
				if ((m_numBytes - m_bufferSize) < m_currentIndex)
				{
					return false;
				}
				args.SetBuffer(m_buffer, m_currentIndex, m_bufferSize);
				m_currentIndex += m_bufferSize;
			}
			return true;
		}

		// Removes the buffer from a SocketAsyncEventArg object.  
		// This frees the buffer back to the buffer pool
		public void FreeBuffer(SocketAsyncEventArgs args)
		{
			m_freeIndexPool.Push(args.Offset);
			args.SetBuffer(null, 0, 0);
		}

	}
}
