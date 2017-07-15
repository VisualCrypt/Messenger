//using Obsidian.Common;
//using Obsidian.Cryptography.TLS;
//using System;
//using System.Diagnostics;
//using System.Net;
//using System.Net.Sockets;
//using System.Threading;

//namespace Obsidian.MessageNode.Core.Server
//{
//	public class TcpServer
//	{
//		readonly IPEndPoint _ipEndPoint;
//		readonly Socket _listenerSocket;
//		readonly ManualResetEvent _connectSignal = new ManualResetEvent(false);
//		readonly IRequestHandler _requestHandler;


//		public TcpServer(IRequestHandler serverRequestHandler)
//		{
//			_requestHandler = serverRequestHandler;
//			_ipEndPoint = new IPEndPoint(IPAddress.Any, Config.TcpServerPort);
//			_listenerSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
//			//_listenerSocket.SetIPProtectionLevel(IPProtectionLevel.Unrestricted);
//		}
//		CancellationToken _cancellationToken;
//		public void Run(CancellationToken cancellationToken)
//		{
//			_cancellationToken = cancellationToken;
//			try
//			{
//				_listenerSocket.Bind(_ipEndPoint);
//				_listenerSocket.Listen(Config.TcpListenerBacklog);
//				Log.Info("TcpServer started listening. To stop, use the CancellationToken provides to VisualCryptMessageServer.RunAsync().");
//				while (!cancellationToken.IsCancellationRequested)
//				{
//					SocketAsyncEventArgs e = new SocketAsyncEventArgs();
//					e.Completed += AcceptCallback;
//					if (!_listenerSocket.AcceptAsync(e))
//					{
//						AcceptCallback(_listenerSocket, e);
//					}

//					_connectSignal.Reset();
//					Log.Info("Waiting for a connection...");

//					_connectSignal.WaitOne();
//				}

//			}
//			catch (Exception e)
//			{
//				Log.Error($"AsyncTcpServer.Run: {e.Message}");
//			}
//			finally
//			{
//				_listenerSocket.Shutdown(SocketShutdown.Both);
//				//_listenerSocket.Close();
//			}
//		}

//		void AcceptCallback(object sender, SocketAsyncEventArgs e)
//		{
//			_connectSignal.Set();

//			try
//			{
//				var acceptSocket = e.AcceptSocket;
//				Log.Info($"Accepted TCP connection from {acceptSocket.RemoteEndPoint}.");
				
				
//				if (_cancellationToken.IsCancellationRequested)
//				{
//					Log.Info($"TcpServer: Cancelled.");
//					acceptSocket.Shutdown(SocketShutdown.Both);
//					acceptSocket.Dispose();
//					return;
//				}
				

//				var reader = new SecureTcpReaderBuffer(acceptSocket);
//				e.Completed += ReceiveCompleted;
				

//				if (!reader.Client.ReceiveAsync(e))
//				{
//					ReceiveCompleted(reader, e);
//				}

//				//reader.Client.ReceiveAsync(reader.Buffer, 0, SecureTcpReaderBuffer.BufferSize, 0, ReadCallback, reader);
//			}
//			catch (Exception ex)
//			{
//				Log.Error($"ReadCallback: {ex.Message}");
//			}

//		}

//		async void ReceiveCompleted(object sender, SocketAsyncEventArgs e)
//		{
//			var reader = (SecureTcpReaderBuffer)sender;
//			reader.Buffer = e.Buffer;
//			try
//			{
//				var stopWatch = new Stopwatch();
//				stopWatch.Start();

//				var currentBytesRead = e.BytesTransferred;

//				if (currentBytesRead == 0) // readerBuffer.Payload MUST stay null!
//					return;

//				TLSEnvelopeExtensions.UpdatePayload(currentBytesRead, reader);

//				do
//				{
//					var packet = TLSEnvelopeExtensions.TryTakeOnePacket(ref reader.Payload);
//					if (packet == null)
//						break;

//					var reply = await _requestHandler.ProcessRequestAsync(packet.Serialize());
//					stopWatch.Stop();


//					if (reply != null)
//						Send(reader.Client, reply);
//					var sent = reply == null ? "nothing" : $"{reply.Length} bytes";
//					Log.Info($"TcpServer received {packet.Serialize().Length} bytes, sent {sent}, {stopWatch.ElapsedMilliseconds}ms, {reader.Client.RemoteEndPoint}.");
//				} while (reader.Payload != null);

//				//reader.Client.BeginReceive(reader.Buffer, 0, SecureTcpReaderBuffer.BufferSize, 0, ReadCallback, reader);


//			}
//			catch (Exception ex)
//			{
//				Log.Error($"ReadCallback: {ex.Message}");
//				reader.Client.Shutdown(SocketShutdown.Both);
//				//reader.Client.Close();
//				Log.Error($"ReadCallback: Connection Closed.");
//			}
//		}

//		async void ReadCallback(IAsyncResult ar)
//		{


//			var reader = (SecureTcpReaderBuffer)ar.AsyncState;
//			try
//			{
//				var stopWatch = new Stopwatch();
//				stopWatch.Start();

//				var currentBytesRead = reader.Client.EndReceive(ar);

//				if (currentBytesRead == 0) // readerBuffer.Payload MUST stay null!
//					return;

//				TLSEnvelopeExtensions.UpdatePayload(currentBytesRead, reader);

//				do
//				{
//					var packet = TLSEnvelopeExtensions.TryTakeOnePacket(ref reader.Payload);
//					if (packet == null)
//						break;

//					var reply = await _requestHandler.ProcessRequestAsync(packet.Serialize());
//					stopWatch.Stop();


//					if (reply != null)
//						Send(reader.Client, reply);
//					var sent = reply == null ? "nothing" : $"{reply.Length} bytes";
//					Log.Info($"TcpServer received {packet.Serialize().Length} bytes, sent {sent}, {stopWatch.ElapsedMilliseconds}ms, {reader.Client.RemoteEndPoint}.");
//				} while (reader.Payload != null);

//				reader.Client.BeginReceive(reader.Buffer, 0, SecureTcpReaderBuffer.BufferSize, 0, ReadCallback, reader);


//			}
//			catch (Exception e)
//			{
//				Log.Error($"ReadCallback: {e.Message}");
//				reader.Client.Shutdown(SocketShutdown.Both);
//				reader.Client.Close();
//				Log.Error($"ReadCallback: Connection Closed.");
//			}

//		}





//		void Send(Socket handler, byte[] reply)
//		{
//			try
//			{
//				handler.BeginSend(reply, 0, reply.Length, 0, SendCallback, handler);
//			}
//			catch (Exception e)
//			{
//				Log.Error($"Send: {e.Message}");
//			}
//		}

//		static void SendCallback(IAsyncResult ar)
//		{
//			((Socket)ar.AsyncState).EndSend(ar);
//		}
//	}
//}
