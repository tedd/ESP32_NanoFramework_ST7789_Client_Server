using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace Tedd.House1.DeviceServer
{
    public class NetworkDeviceServer
    {
        private readonly int _port;
        private CancellationTokenSource _cancellationTokenSource;
        private List<VideoServerClient> _clients = new List<VideoServerClient>();
        public NetworkDeviceServer(int port)
        {
            _port = port;
            _cancellationTokenSource = new CancellationTokenSource();
        }

        public async Task StartAsync()
        {
            TcpListener listener = new TcpListener(_port);
            listener.Start();
            _cancellationTokenSource.Token.Register(listener.Stop);
            while (!_cancellationTokenSource.Token.IsCancellationRequested)
            {
                try
                {
                    var client = await listener.AcceptTcpClientAsync();
                    var clientTask = HandleClient(client, _cancellationTokenSource.Token)
                        .ContinueWith((antecedent) => client.Dispose())
                        .ContinueWith((antecedent) => Console.WriteLine("Client disposed."));
                }
                catch (ObjectDisposedException) when (_cancellationTokenSource.Token.IsCancellationRequested)
                {
                    Console.WriteLine("TcpListener stopped listening because cancellation was requested.");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error handling client: {ex.ToString()}");
                }
            }
        }

        public async Task StopAsync()
        {
            _cancellationTokenSource.Cancel();
        }

        private Task HandleClient(TcpClient client, in CancellationToken token)
        {
            var vsc = new VideoServerClient(client, token);
            lock (_clients)
                _clients.Add(vsc);

            return vsc.ProcessIncoming();
        }
    }
}
