using System;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading;
using System.Threading.Tasks;
using Windows.Networking;
using Windows.Networking.Sockets;
using Windows.Storage.Streams;
using Obsidian.Applications.Services.Interfaces;
using Obsidian.Common;
using Obsidian.Cryptography.Api.Infrastructure;
using Obsidian.Cryptography.NoTLS;
using Obsidian.Cryptography.TLS;

namespace Obsidian.UWP.Core.Network
{
    public class TcpConnection : ITcpConnection
    {
        readonly ILog _log;

        public TcpConnection(Container container)
        {
            _log = container.Get<ILog>();
        }

        StreamSocket _streamSocket;
        DataWriter _dataWriter;
        CancellationTokenSource _cts;

        public bool IsConnected { get; private set; }

        public async Task<bool> ConnectAsync(string remoteDnsHost, int remotePort, Func<byte[], Transport, Task<string>> receiver = null)
        {
            try
            {
                _streamSocket = new StreamSocket();
                _cts = new CancellationTokenSource();
				// https://docs.microsoft.com/en-us/uwp/api/windows.networking.sockets.streamsocketcontrol#Windows_Networking_Sockets_StreamSocketControl_KeepAlive
				_streamSocket.Control.KeepAlive = true;
                await _streamSocket.ConnectAsync(new HostName(remoteDnsHost), remotePort.ToString());
                _dataWriter = new DataWriter(_streamSocket.OutputStream);

                IsConnected = true;
                return true;
            }
            catch (Exception e)
            {
                DisconnectPrivate();
                _log.Exception(e);
                return false;
            }
        }


        public async Task DisconnectAsync()
        {
            await Task.Delay(0);
            DisconnectPrivate();
        }

        void DisconnectPrivate()
        {
            IsConnected = false;
            _cts?.Cancel();

            // To reuse the socket with another data writer, the application must detach the stream from the
            // current writer before disposing it.
            _dataWriter?.DetachStream();
            _dataWriter?.Dispose();
            _dataWriter = null;

            _streamSocket?.Dispose();
            _streamSocket = null;
            _cts?.Dispose();
            _cts = null;
        }






        SemaphoreSlim sem = new SemaphoreSlim(1, 1);

        public async Task<Response<List<IEnvelope>>> SendRequestAsync(byte[] request)
        {
            var response = new Response<List<IEnvelope>>();

            try
            {
                //await sem.WaitAsync(); // this definitely deadlocks sometimes

                if (!IsConnected)
                {
                    response.SetError("SocketError.OperationAborted");
                    _log.Debug("SocketError.OperationAborted");
                    return response;
                }
				
                _dataWriter.WriteBuffer(request.AsBuffer());
                await _dataWriter.StoreAsync();
                await _dataWriter.FlushAsync(); // perhaps release the semaphore here and not after await ReceivePackets?

                var bufferSize = 4096;
                var reader = new TLSEnvelopeReaderBuffer { Buffer = new byte[bufferSize], Payload = null };

                using (var socketStream = new SocketStream(_streamSocket)) // SocketStream is a class I have created
                {
                    var receivedPackets = await TLSEnvelopeReader.ReceivePackets(reader,socketStream, _cts.Token);
                    response.Result = receivedPackets;
                    response.SetSuccess();
                } // does this something to the underlying socket...? No! Calls Stream.Dispose which calls stream.Close

            }
            catch (Exception e)
            {
                response.SetError("SocketError.OperationAborted");
                _log.Exception(e);
                DisconnectPrivate();
            }
            finally
            {
               // sem.Release();
            }
            return response;
        }
    }
}

