using System;
using System.Buffers;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.IO.Pipelines;
using System.Linq;
using System.Net.Mime;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Tedd.House1.DeviceServer.Temp;
using Tedd.RandomUtils;
using PixelFormat = Tedd.House1.DeviceServer.Temp.PixelFormat;

namespace Tedd.House1.DeviceServer
{
    public static class ImageFolderHelper
    {
        public static List<St7789Image> ReadImages()
        {
            var dir = @"C:\Temp\Rick";
            var files = Directory.GetFiles(dir).OrderBy(n => n).Take(200).ToList();
            var ret = new List<St7789ImageContainer>(files.Count);
            Parallel.ForEach(files, file =>
            //foreach (var file in files)
            {
                var stImage = new St7789Image(128, 128, PixelFormat.RGB_565_16Bit);
                lock (ret)
                    ret.Add(new St7789ImageContainer() { Filename = file,Image= stImage});
                using var image = Bitmap.FromFile(file);
                using var bmp = new Bitmap(image, new Size(128,128));
                //using var bmp = Bitmap.FromFile(file);

                //var BtmpDt = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height), ImageLockMode.ReadWrite,
                //    bmp.PixelFormat);
                //IntPtr pointer = BtmpDt.Scan0;
                //int size = Math.Abs(BtmpDt.Stride) * bmp.Height;
                ////byte[] pixels = new byte[size];
                ////Marshal.Copy(pointer, pixels, 0, size);
                //Marshal.Copy(pointer, stImage.Data, 0, stImage.Data.Length);
                ////for (int b = 0; b < pixels.Length; b++)
                ////{
                ////    pixels[b] = 255;// do something here 
                ////}
                ////for (var x = 0; x < 128; x++)
                ////{
                ////    for (var y = 0; y < 128; y++)
                ////    {
                ////        var i = (y * bmp.Width) + x;
                ////        var p = pixels[i];
                ////        var c = RgbColor.FromRGB((byte)(p>>24), (byte)(p >> 16), (byte)(p >> 8));
                ////                stImage.Set(x, y, c);
                ////    }
                ////}

                ////Marshal.Copy(pixels, 0, pointer, size);
                //bmp.UnlockBits(BtmpDt);

                for (var x = 0; x < 128; x++)
                {
                    for (var y = 0; y < 125; y++)
                    {
                        var color = bmp.GetPixel(x, y);
                        var c = RgbColor.FromRGB(color.R, color.G, color.B);
                        stImage.Set(x, y, c);
                    }
                }


            });
            return ret.OrderBy(s=>s.Filename).Select(s=>s.Image).ToList();
        }
    }

    public class St7789ImageContainer
    {
        public string Filename;
        public St7789Image Image;
    }

    public class VideoServerClient
    {
        private readonly TcpClient _client;
        private readonly CancellationToken _token;
        //public St7789Image Image = new St7789Image(128, 128, PixelFormat.RGB_565_16Bit);
        private bool ClientReadyToReceiveImage;

        private static List<St7789Image> _images;
        private int _currentImage;



        public VideoServerClient(TcpClient client, in CancellationToken token)
        {
            _client = client;
            _token = token;

            if (_images == null)
                _images = ImageFolderHelper.ReadImages();
        }

        public async Task ProcessIncoming()
        {
            await ProcessLinesAsync(_client.Client);
            _client.Close();
            _client.Dispose();
        }

        private Task ProcessLinesAsync(Socket socket)
        {
            var pipe = new Pipe();
            var writing = FillPipeAsync(socket, pipe.Writer);
            var reading = ReadPipeAsync(pipe.Reader);
            var sending = SendImageAsync(socket);

            return Task.WhenAll(reading, writing, sending);
        }

        private async Task SendImageAsync(Socket socket)
        {
            while (!_token.IsCancellationRequested)
            {
                if (!ClientReadyToReceiveImage)
                {
                    await Task.Delay(10);
                    continue;
                }

                var image = _images[_currentImage];
                 //image = _images[20];
                //Image.DrawFilledRectangle(0, 0, 127, 127, RgbColor.From16Bit(ConcurrentRandom.NextUInt16()));

                //var rom = Image.Data.AsMemory();
                var rom = image.Data.AsMemory();
                var len = await socket.SendAsync(rom, SocketFlags.None, _token);
                Console.WriteLine($"Sent {len} bytes.");
                ClientReadyToReceiveImage = false;
            }
        }

        public async Task FillPipeAsync(Socket socket, PipeWriter writer)
        {
            const int minimumBufferSize = 512;

            while (!_token.IsCancellationRequested)
            {
                // Allocate at least 512 bytes from the PipeWriter
                Memory<byte> memory = writer.GetMemory(minimumBufferSize);
                try
                {
                    int bytesRead = await socket.ReceiveAsync(memory, SocketFlags.None);
                    if (bytesRead == 0)
                    {
                        break;
                    }
                    // Tell the PipeWriter how much was read from the Socket
                    writer.Advance(bytesRead);
                }
                catch (Exception ex)
                {
                    LogError(ex);
                    break;
                }

                // Make the data available to the PipeReader
                var result = await writer.FlushAsync();

                if (result.IsCompleted)
                {
                    break;
                }
            }

            // Tell the PipeReader that there's no more data coming
            writer.Complete();
        }

        private void LogError(Exception exception)
        {
            Console.WriteLine("Exception: " + exception.ToString());
        }

        private async Task ReadPipeAsync(PipeReader reader)
        {
            while (!_token.IsCancellationRequested)
            {
                ReadResult result = await reader.ReadAsync();

                ReadOnlySequence<byte> buffer = result.Buffer;
                SequencePosition? position = null;

                do
                {
                    // Look for a EOL in the buffer
                    position = buffer.PositionOf((byte)'\n');

                    if (position != null)
                    {
                        // Process the line
                        await ProcessLine(buffer.Slice(0, position.Value));

                        // Skip the line + the \n character (basically position)
                        buffer = buffer.Slice(buffer.GetPosition(1, position.Value));
                    }
                }
                while (position != null);

                // Tell the PipeReader how much of the buffer we have consumed
                reader.AdvanceTo(buffer.Start, buffer.End);

                // Stop reading if there's no more data coming
                if (result.IsCompleted)
                {
                    break;
                }
            }

            // Mark the PipeReader as complete
            reader.Complete();
        }

        private async Task ProcessLine(ReadOnlySequence<byte> slice)
        {
 //           await Task.Delay(1000);

            _currentImage++;
            if (_currentImage >= _images.Count)
                _currentImage = 0;
            ClientReadyToReceiveImage = true;

            // Process line
            var sb = new StringBuilder();
            sb.Append("RECV: ");
            foreach (var s in slice.ToArray())
            {
                sb.Append(s.ToString("X2"));
            }

            Console.WriteLine(sb.ToString());
        }


    }
}