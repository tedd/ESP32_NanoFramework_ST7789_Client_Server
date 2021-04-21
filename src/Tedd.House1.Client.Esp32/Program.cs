using System;
using System.Threading;
using Windows.Devices.Gpio;
using Windows.Devices.Spi;
using nanoFramework.Hardware.Esp32;
using Tedd.House1.Client.Esp32.Drivers;
using Tedd.House1.Client.Esp32.Drivers.ST7789;
using Tedd.House1.Client.Esp32.NetworkClient;
using Tedd.House1.Client.Esp32.WifiClient;

namespace Tedd.House1.Client.Esp32
{
    public class Program
    {
        private static Random _random = new Random();
        private static Light _blueLight;
        private static Light _redLight;
        private static Light _yellowLight;
        private static Light _greenLight;

        public static void Main()
        {
            Console.WriteLine("Starting...");
            //NetworkHelpers.SetupAndConnectNetwork(false);

            _blueLight = new Light(32);
            _redLight=new Light(33);
            _yellowLight=new Light(25);
            _greenLight=new Light(26);

            // Starting with red light on
            _redLight.On();
            _yellowLight.On();
            _greenLight.On();
            _blueLight.Off();

            //Console.WriteLine("Waiting for network up and IP address...");

            //NetworkHelpers.IpAddressAvailable.WaitOne();


            // Browse our samples repository: https://github.com/nanoframework/samples
            // Check our documentation online: https://docs.nanoframework.net/
            // Join our lively Discord community: https://discord.gg/gCyBu8T
            //Configuration.SetPinFunction(33, DeviceFunction.COM2_RX);


            //var sc = SpiDevice.GetBusInfo("SPI0");

            //var sbi = new SpiBusInfo("SPI0");
            //Console.WriteLine($"Spi deviceID:{sc.MaxClockFrequency}");
            var allSpi = SpiDevice.GetDeviceSelector();
            Console.WriteLine($"SPI's: {allSpi}");
            foreach (var df in new[] { DeviceFunction.SPI1_MOSI, DeviceFunction.SPI1_MISO, DeviceFunction.SPI1_CLOCK })
                Console.WriteLine($"{df.ToString()}: {Configuration.GetFunctionPin(df)}");

            // Set up display
            var st7789 = new ST7789(PixelFormat.RGB_565_16Bit, 1);
            var width = 128;
            var height = 128;
            var image = new St7789Image(width, height, PixelFormat.RGB_565_16Bit);
            image.Clear();
            //fullScreenBitmap.Flush();
            //image.DrawRectangle(Color.White, 0, 0, fullScreenBitmap.Height - 20, 320, 22, 0, 0, Color.White,0, fullScreenBitmap.Height - 20, Color.White, 0, fullScreenBitmap.Height, Bitmap.OpacityOpaque);
            //fullScreenBitmap.DrawText("Test", DisplayFont, Color.Black, 0, fullScreenBitmap.Height - 20);
            image.DrawLine(64, 64, 127, 127, RgbColor.FromRGB(255, 0, 0));
            image.DrawLine(127, 0, 0, 127, RgbColor.FromRGB(0, 255, 0));
            image.DrawLine(127, 0, 127, 127, RgbColor.FromRGB(0, 0, 255));
            st7789.DrawImage(image);

            // Connect to wifi
            var wifiConfig = new WifiConfig("KongeIoT", "GammaQW123");
            wifiConfig.SetIpv4("10.1.1.80", "255.255.255.0", "10.1.1.1", "1.1.1.4");
            var wifi = new WifiClient.WifiClient(wifiConfig);
            wifi.StateChanged += Wifi_StateChanged;
            wifi.Connect();

            // Connect to server
            var vsc = new VideoServerClient();
            int framePos = 0;
            //bool inTransfer = false;
            //bool doneCopying = false;
            var imageReceived = new ManualResetEvent(false);
            int fp;
            var rCount = 0;
            vsc.DataReceived += (sender, data, len) =>
            {
                rCount++;
                //Console.WriteLine($"Recvd[{rCount}]: {len}, total: {framePos + len} > {128*128*2} = {framePos + len - 128*128*2}");
                if (len == 0)
                    return;

                if (_blueLight.Status)
                    _blueLight.Off();

                //st7789.DrawContinue(data, len);
                //st7789.DrawContinue( data, len);
                fp = framePos;
                //doneCopying = false;
                Array.Copy(data, 0, image.Data, fp, len);

                framePos += len;
                if (framePos > 128 * 128 * 2)
                    throw new Exception("Received more image data than expected.");
                if (framePos == 128 * 128 * 2)
                {
                    framePos = 0;
                    //inTransfer = false;
                    imageReceived.Set();
                }

               // doneCopying = true;


            };
            vsc.Connect("10.1.1.6", 4000);

            while (true)
            {
                //Thread.Sleep(1);
                //if (inTransfer)
                //    continue;
                //inTransfer = true;
                //st7789.DrawBegin(0,0,128,128);
                st7789.DrawImage(image);
                imageReceived.Reset();
                vsc.RequestImage();
                imageReceived.WaitOne();
                _blueLight.On();
                // Wait for copy to finish before we draw
                //while (!doneCopying) { }
            }

            //// 128X128 memory base (GM=’001’)
            //// 120X128X18 - bit memory can be written by this command.
            ////    Memory range(0000h, 0000h) -> (007Fh,007Fh) 

            //// for (var height = 1; height < 128; height++)
            ////{
            //var width = 128;
            //var height = 128;
            ////var bytes = new byte[width * height * 2];
            ////for (var i = 0; i < bytes.Length; i++)
            ////bytes[i] = 255;
            ////_random.NextBytes(bytes);
            ////st7789.DrawRaw(0, 0, width, height, bytes);
            ////}
            ////var DisplayFont = Resource.GetFont(Resource.FontResources.SegoeUIRegular12);
            //var image = new St7789Image(width, height, PixelFormat.RGB_565_16Bit);
            //image.Clear();
            ////fullScreenBitmap.Flush();
            ////image.DrawRectangle(Color.White, 0, 0, fullScreenBitmap.Height - 20, 320, 22, 0, 0, Color.White,0, fullScreenBitmap.Height - 20, Color.White, 0, fullScreenBitmap.Height, Bitmap.OpacityOpaque);
            ////fullScreenBitmap.DrawText("Test", DisplayFont, Color.Black, 0, fullScreenBitmap.Height - 20);
            //image.DrawLine(64, 64, 127, 127, RgbColor.FromRGB(255, 0, 0));
            //image.DrawLine(127, 0, 0, 127, RgbColor.FromRGB(0, 255, 0));
            //image.DrawLine(127, 0, 127, 127, RgbColor.FromRGB(0, 0, 255));

            ////image.DrawFilledRectangle(0, 0, 127, 20, RgbColor.FromRGB(0,255,255));
            ////image.DrawRectangle(0, 0, 10, 10, RgbColor.FromRGB(0, 0, 255), 5);
            ////image.DrawRectangle(0, 10, 127, 127, RgbColor.FromRGB(0, 0, 255), 1);
            ////image.DrawRectangle(0 + 30, 0 + 30, 127 - 30, 127 - 30, RgbColor.FromRGB(0, 255, 255), 1);

            //for (var y = 0; y < 127; y++)
            //{
            //    for (var x = 0; x < 127; x++)
            //    {


            //        var b = y - 128;
            //        if (b < 0)
            //            b += 255;
            //        var co = RgbColor.FromRGB((byte)(255-x), (byte)y, (byte)(x));
            //        image.Set(x, y, co);
            //    }
            //    st7789.DrawImage(image);
            //}

            //Thread.Sleep(1000);

            ////image.Fill(RgbColor.FromRGB(255, 0, 0));
            //for (var i = 0; i < width * height * 2; i++)
            //{
            //    //image.Clear();
            //    //image.DrawLine(0,0,width-i,i, RgbColor.FromRGB(255,255,0));
            //    image.Data[i] = 0xF0;
            //    st7789.DrawImage(image);
            //}

            //st7789.DrawImage(image);

            ////for (var x = 0; x < 127; x++)
            ////for (var y = 0; y < 127; y++)
            ////st7789.SetPixel(x, y, 0 - 1);
            ////for (var x = 0; x < 128; x++)
            ////    st7789.SetPixel(x, x, -1);



            //var led = GpioController.GetDefault().OpenPin(33);
            //led.SetDriveMode(GpioPinDriveMode.Output);
            //var on = true;
            //for (; ; )
            //{
            //    on = !on;
            //    //if (on)
            //    //    st7789.DisplayOn();
            //    //else
            //    //    st7789.DisplayOff();
            //    led.Write(on ? GpioPinValue.High : GpioPinValue.Low);
            //    Thread.Sleep(500);
            //}
        }

        private static void Wifi_StateChanged(WifiClient.WifiClient sender, WifiClient.WifiClient.NetworkState oldState, WifiClient.WifiClient.NetworkState newState)
        {
            _redLight.Status = newState == WifiClient.WifiClient.NetworkState.None || newState == WifiClient.WifiClient.NetworkState.WifiScanning;
            _greenLight.Status = newState == WifiClient.WifiClient.NetworkState.Connected;
            _yellowLight.Status = !_redLight.Status && !_greenLight.Status;
        }
    }
}
