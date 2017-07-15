using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using Obsidian.Common;

namespace Obsidian.MessageNode.Core.Server
{
	// https://stackoverflow.com/questions/14044852/20-receives-per-second-with-socketasynceventargs
	public class Sudo
	{
		Socket _listener;
		int _port = 48797;
		private IRequestHandler _serverRequestHandler;

		public Sudo()
		{
			var ipEndPoint = new IPEndPoint(IPAddress.Any, _port);
			_listener = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
			_listener.Bind(ipEndPoint);

			_listener.Listen(100);

			Accept(null);
		}

		public Sudo(IRequestHandler serverRequestHandler)
		{
			_serverRequestHandler = serverRequestHandler;
			var ipEndPoint = new IPEndPoint(IPAddress.Any, _port);
			_listener = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
			_listener.Bind(ipEndPoint);

			_listener.Listen(100);

			Accept(null);
		}

		void Accept(SocketAsyncEventArgs acceptEventArg)
		{
			if (acceptEventArg == null)
			{
				acceptEventArg = new SocketAsyncEventArgs();
				acceptEventArg.Completed += AcceptCompleted;
			}
			else acceptEventArg.AcceptSocket = null;

			bool willRaiseEvent = _listener.AcceptAsync(acceptEventArg); ;

			if (!willRaiseEvent) Accepted(acceptEventArg);
		}

		void AcceptCompleted(object sender, SocketAsyncEventArgs e)
		{
			Accepted(e);
		}

		void Accepted(SocketAsyncEventArgs e)
		{
			var acceptSocket = e.AcceptSocket;
			var readEventArgs = CreateArg(acceptSocket);

			var willRaiseEvent = acceptSocket.ReceiveAsync(readEventArgs);

			Accept(e);

			if (!willRaiseEvent) Received(readEventArgs);
		}

		SocketAsyncEventArgs CreateArg(Socket acceptSocket)
		{
			var arg = new SocketAsyncEventArgs();
			arg.Completed += IOCompleted;

			var buffer = new byte[64 * 1024];
			arg.SetBuffer(buffer, 0, buffer.Length);

			arg.AcceptSocket = acceptSocket;

			arg.SocketFlags = SocketFlags.None;

			return arg;
		}

		void IOCompleted(object sender, SocketAsyncEventArgs e)
		{
			switch (e.LastOperation)
			{
				case SocketAsyncOperation.Receive:
					Received(e);
					break;
				default: break;
			}
		}

		void Received(SocketAsyncEventArgs e)
		{
			if (e.SocketError != SocketError.Success || e.BytesTransferred == 0 || e.Buffer == null || e.Buffer.Length == 0)
			{
				// Kill(e);
				return;
			}

			var bytesList = new List<byte>();
			for (var i = 0; i < e.BytesTransferred; i++) bytesList.Add(e.Buffer[i]);

			var bytes = bytesList.ToArray();


			Process(bytes);
			

			ReceiveRest(e);

			//Perf.IncOp();
		}

		void ReceiveRest(SocketAsyncEventArgs e)
		{
			e.SocketFlags = SocketFlags.None;
			for (int i = 0; i < e.Buffer.Length; i++) e.Buffer[i] = 0;
			e.SetBuffer(0, e.Buffer.Length);

			var willRaiseEvent = e.AcceptSocket.ReceiveAsync(e);
			if (!willRaiseEvent) Received(e);
		}

		void Process(byte[] bytes)
		{
			var text = Encoding.UTF8.GetString(bytes);
			Debug.WriteLine(text);
			
		}

		//____________________________________________________________________________
		// This method is called by I/O Completed() when an asynchronous send completes.
		// If all of the data has been sent, then this method calls StartReceive
		//to start another receive op on the socket to read any additional
		// data sent from the client. If all of the data has NOT been sent, then it
		//calls StartSend to send more data.
	
	}
}
