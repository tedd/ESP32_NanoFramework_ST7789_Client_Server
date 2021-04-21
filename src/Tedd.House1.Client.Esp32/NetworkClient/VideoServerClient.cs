using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace Tedd.House1.Client.Esp32.NetworkClient
{
    public class VideoServerClient
    {
        private Socket _socket;
        private byte[] _buffer = new byte[1024*2];

        public delegate void DataReceivedDelegate(VideoServerClient sender, byte[] data, int len);

        public event DataReceivedDelegate DataReceived;
        private Thread SocketReadThread;
        private bool _socketConnected = false;
        public void Connect(string address, int port)
        {
            try
            {
                Log($"Resolving {address}");
                var hostEntry = Dns.GetHostEntry(address);
                Log($"Resolved {address} to {hostEntry.AddressList[0]}");
                var ep = new IPEndPoint(hostEntry.AddressList[0], port);
                Thread.Sleep(2000);
                _socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                Thread.Sleep(2000);
                // connect socket
                Log($"Connecting to {ep.Address} port {ep.Port}");
                _socket.Connect(ep);
                Thread.Sleep(2000);
                Log($"Connected.");
                _socketConnected = true;
                Log("Starting receive thread");
                SocketReadThread = new Thread(ReceiveThreadLoop);
                SocketReadThread.Start();
                
            }
            catch (SocketException ex)
            {
                Console.WriteLine($"** Socket exception occurred: {ex.Message} error code {ex.ErrorCode}!**");
                Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"** Exception occurred: {ex.Message}!**");
                Close();
            }
            finally
            {
                
            }
            //_socket.Dispose();
        }

        public void Close()
        {
            Console.WriteLine("Closing socket");
            _socket?.Close();
            SocketReadThread?.Abort();
        }

        private void ReceiveThreadLoop()
        {
            int len;
            while (_socketConnected)
            {
                if (_socket.Available == 0)
                {
                    Thread.Sleep(1);
                    continue;
                }
                // trying to read from socket
                len = _socket.Receive(_buffer);

                //Log($"Read {len} bytes");

                if (len == 0)
                    throw new Exception("Connection closed.");

                if (len > 0)
                {
                    // we have data!
                    // output as string
                    DataReceived?.Invoke(this, _buffer, len);
                }
            }
        }

        public void RequestImage()
        {
            _buffer[0] = 1;
            _buffer[1] = 10;
            //Log($"Sending new image request");
            var len = _socket.Send(_buffer, 0, 2, SocketFlags.None);
            //Log($"Sent {len} bytes");
        }

        private void Log(string line)
        {
            Console.WriteLine(line);
        }
    }
}

