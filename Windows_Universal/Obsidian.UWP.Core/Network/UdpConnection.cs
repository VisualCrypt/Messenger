using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Networking;
using Windows.Networking.Connectivity;
using Windows.Networking.Sockets;
using Windows.Storage.Streams;
using Obsidian.Applications.Services.Interfaces;
using Obsidian.Common;
using Obsidian.Cryptography.Api.Infrastructure;
using Obsidian.Cryptography.NoTLS;
using Obsidian.Cryptography.TLS;

namespace Obsidian.UWP.Core.Network
{
    public class UdpConnection : IUdpConnection
    {
        readonly AsyncLock _lock = new AsyncLock();
        readonly ILog _log;
        readonly Response<List<IEnvelope>> _successResponse;

        public UdpConnection(Container container)
        {
            _log = container.Get<ILog>();
            _successResponse = new Response<List<IEnvelope>>();
            _successResponse.SetSuccess();
        }

        DatagramSocket _udpClient;
        CancellationTokenSource _cts;
        Func<byte[], Transport, Task<string>> _receiver;



        public bool IsConnected { get; private set; }

        public async Task<bool> ConnectAsync(string remoteDnsHost, int remotePort, Func<byte[], Transport, Task<string>> receiver = null)
        {
            try
            {
                _udpClient = new DatagramSocket();
                _udpClient.MessageReceived += OnMessageReceived;
                var hostName = new HostName(remoteDnsHost.Trim());
                await _udpClient.ConnectAsync(hostName, remotePort.ToString());
                _cts = new CancellationTokenSource();
                IsConnected = true;
                _receiver = receiver;
                return true;
            }
            catch (Exception e)
            {
                DisconnectPrivate();
                _log.Exception(e);
                return false;
            }
        }




        bool ConnectedToInternet()
        {
            var sw = new Stopwatch();
            sw.Start();
            var isConnected = NetworkInformation.GetInternetConnectionProfile()?.GetNetworkConnectivityLevel() == NetworkConnectivityLevel.InternetAccess;
            sw.Stop();
            Debug.WriteLine("CONNECTION CHECK TOOK " + sw.ElapsedMilliseconds);
            return isConnected;
        }




        async void OnMessageReceived(DatagramSocket sender, DatagramSocketMessageReceivedEventArgs args)
        {
            try
            {
                using (var dataReader = args.GetDataReader())
                {
                    byte[] readBuffer = new byte[dataReader.UnconsumedBufferLength];
                    dataReader.ReadBytes(readBuffer);
                    string s = await _receiver(readBuffer, Transport.UDP);
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
            }
        }






        async Task SendBatch(IList<IBuffer> packetsToSend)
        {
            // More efficient way to send packets in Windows 10, using the new behavior of OutputStream.FlushAsync().
            int i = 0;

            // 18.09.2016: _udpClient can be null here, especially after resume from sleep
            // Fix: add explicit Disconnect in ChatWorker and _appState.IsUdpConnected = false; to trigger a recycle.
            var outputStream = _udpClient.OutputStream;

            var pendingWrites = new IAsyncOperationWithProgress<uint, uint>[packetsToSend.Count];

            foreach (IBuffer packet in packetsToSend)
            {
                pendingWrites[i++] = outputStream.WriteAsync(packet);
                // Do not modify any buffer's contents until the pending writes are complete.
            }

            // Wait for all pending writes to complete. This step enables batched sends on the output stream.
            await outputStream.FlushAsync();

        }



        public async Task DisconnectAsync()
        {
            using (await _lock.LockAsync())
                DisconnectPrivate();
        }

        public async Task<Response<List<IEnvelope>>> SendRequestAsync(byte[] request)
        {
            using (await _lock.LockAsync())
            {
                try
                {
                    if (!ConnectedToInternet())
                    {
                        throw new Exception("UDP: NOT CONNECTED");
                    }

                    var buffers = new List<IBuffer> { request.AsBuffer() };
                    await SendBatch(buffers);
                    return _successResponse;
                }
                catch (Exception e)
                {
                    DisconnectPrivate();
                    _log.Exception(e);
                    var response = new Response<List<IEnvelope>>();
                    response.SetError(e.Message);
                    return response;
                }
            }
        }

        void DisconnectPrivate()
        {
            IsConnected = false;
            _cts?.Cancel();
            _udpClient?.Dispose();
            _udpClient = null;
            _cts?.Dispose();
            _cts = null;
        }
    }
}

