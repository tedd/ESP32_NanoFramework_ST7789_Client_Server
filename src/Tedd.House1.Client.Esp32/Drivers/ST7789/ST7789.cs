using System;
using System.Diagnostics;
using System.Threading;
using Windows.Devices.Gpio;
using Windows.Devices.Spi;
using Tedd.House1.Client.Esp32.Drivers.ST7789;
using CMD_TYPE = System.Byte;

namespace Tedd.House1.Client.Esp32
{
    public class ST7789
    {
        // https://www.rockbox.org/wiki/pub/Main/SonyNWZE370/ILI9163.pdf

        private SpiDevice _lcd;
        private GpioPin _reset;
        private GpioPin _dc;
        private PixelFormat _pixelFormat;

        private byte[] _buffer4k = new byte[_bufferSize];
        private const int _bufferSize = 2048;
        //private const int _bufferSize = 4096 - 10;
        
        ///<summary>Sets up an ST3389 display.</summary>
        /// <param name="spiNumber"></param>
        /// <param name="cs">CS/CE/Chip Select pin, used by SPI.</param>
        /// <param name="dc">AO/DC/Data Command pin. This is used to toggle command/data when writing to SPI.</param>
        /// <param name="reset">Hardware reset pin. Set to -1 to use software reset.</param>
        public ST7789(PixelFormat pixelFormat, int spiNumber, int cs = 18, int dc = 5, int reset = 4)
        {
            // ESP32 returns:
            // SPI's: SPI1,SPI2
            // For SPI1 the defaults are.
            // 65792 SPI1_MOSI: 23
            // 65793 SPI1_MISO: 25
            // 65794 SPI1_CLOCK: 19

            _pixelFormat = pixelFormat;

            var spiBus = $"SPI{spiNumber}";
            var settings = SpiDevice.GetBusInfo(spiBus);
            var connectionSettings = new SpiConnectionSettings(cs)
            {
                DataBitLength = 8,
                ClockFrequency = settings.MaxClockFrequency,
            };
            _lcd = SpiDevice.FromId(spiBus, connectionSettings);

            if (reset > 0)
            {
                _reset = GpioController.GetDefault().OpenPin(reset);
                _reset.SetDriveMode(GpioPinDriveMode.Output);
            }

            if (dc > 0)
            {
                _dc = GpioController.GetDefault().OpenPin(dc);
                _dc.SetDriveMode(GpioPinDriveMode.Output);
            }

            //Reset();

            Init();
        }

        [Conditional("DEBUG")]
        private void Log(string line, params CMD_TYPE[] args)
        {
            //var sb = "";
            //foreach (var a in args)
            //    sb += " " + a.ToString("X2");

            //Console.WriteLine("LOG: " + line + " " + sb);
        }

        private void Init()
        {
            Log("Init()");
            Reset();
            cmd(Constants.Commands.exit_sleep_mode);                               // Exit Sleep
            cmd(Constants.Commands.set_gamma_curve, 0x04);               // Set Default Gamma
            cmd(Constants.Commands.FrameRateControl, 0x0e, 0x10);        // Set Frame Rate
            //cmd(Constants.Commands.Power_Control1, 0x08, 0);             // Set VRH1[4:0] & VC[2:0] for VCI1 & GVDD
            //cmd(Constants.Commands.Power_Control2, 0x05);                // Set BT[2:0] for AVDD & VCL & VGH & VGL
            //cmd(Constants.Commands.VCOM_Control, 0x38, 0x40);            // Set VMH[6:0] & VML[6:0] for VOMH & VCOML

            //cmd(Constants.Commands.set_pixel_format, (byte)_pixelFormat); //Set Color Format: 6=18-bit (262K), 5=16 bit (65K), 3=12 bit (4K)
            cmd(Constants.Commands.set_pixel_format, (byte)_pixelFormat); //Set Color Format: 6=18-bit (262K), 5=16 bit (65K), 3=12 bit (4K)
            cmd(Constants.Commands.set_address_mode, (byte)(Constants.MemoryAccessOrder_Normal | Constants.MemoryAccessOrder_RGB)); // Memory_Access_Control: Rotation and RGB mode

            /*
             * 128X128 memory base (GM=’001’)
             * (Parameter range: 0 ≦ XS[15:0] ≦ XE[15:0] 127(007Fh):MV=”0”)
             * (Parameter range: 0 ≦ XS[15:0] ≦ XE[15:0] 127(007Fh):MV=”1”)
             */
            cmd(Constants.Commands.set_column_address, 0, 0, 0, 128);
            /*
             * 128X128 memory base (GM=’001’)
             * (Parameter range: 0 ≦ YS[15:0] ≦ YE[15:0] ≦ 127(007Fh)):MV=”0”
             * (Parameter range: 0 ≦ YS[15:0] ≦ YE[15:0] ≦ 127(007Fh)):MV=”1”
             */
            cmd(Constants.Commands.set_page_address, 0, 0, 0, 128);

            cmd(Constants.Commands.Display_Inversion_Control, 0);    // display inversion

            cmd(Constants.Commands.GAM_R_SEL, 1);                    // Enable Gamma bit
            cmd(Constants.Commands.Positive_Gamma_Correction_Setting, 0x3f, 0x22, 0x20, 0x30, 0x29, 0x0c, 0x4e, 0xb7, 0x3c, 0x19, 0x22, 0x1e, 0x02, 0x01, 0x00);
            cmd(Constants.Commands.Negative_Gamma_Correction_Setting, 0x00, 0x1b, 0x1f, 0x0f, 0x16, 0x13, 0x31, 0x84, 0x43, 0x06, 0x1d, 0x21, 0x3d, 0x3e, 0x3f);

            cmd(Constants.Commands.set_display_on);                            // Display On
            cmd(Constants.Commands.write_memory_start,0,0);                    // reset frame ptr

        }

        public void DisplayOn() => cmd(Constants.Commands.set_display_on);
        public void DisplayOff() => cmd(Constants.Commands.set_display_off);
        public void DisplayEnterSleep() => cmd(Constants.Commands.enter_sleep_mode);
        public void DisplayExitSleep() => cmd(Constants.Commands.exit_sleep_mode);


        public void DrawPixel(int x, int y, int c)
        {
            cmd(Constants.Commands.set_column_address, 0, (CMD_TYPE)x, 0, (CMD_TYPE)(x + 1));
            cmd(Constants.Commands.set_page_address, 0, (CMD_TYPE)y, 0, (CMD_TYPE)(y + 1));
            cmd(Constants.Commands.write_memory_start, (CMD_TYPE)(c >> 8), (CMD_TYPE)c);
        }
        public void DrawRaw(int x, int y, int width, int height, CMD_TYPE[] c)
        {
            cmd(Constants.Commands.set_column_address, 0, (CMD_TYPE)x, 0, (CMD_TYPE)(x + width));
            cmd(Constants.Commands.set_page_address, 0, (CMD_TYPE)y, 0, (CMD_TYPE)(y + height));
            cmd(Constants.Commands.write_memory_start, c);
        }


        public void DrawBegin(int x, int y, int width, int height)
        {
            cmd(Constants.Commands.set_column_address, 0, (CMD_TYPE)x, 0, (CMD_TYPE)(x + width));
            cmd(Constants.Commands.set_page_address, 0, (CMD_TYPE)y, 0, (CMD_TYPE)(y + height));
            cmd(Constants.Commands.write_memory_start);
        }

        public void DrawContinue(byte[] data, int len)
        {
            WriteData(_buffer4k, len);
        }


        private void Reset()
        {
            // Chip reset pin (“Low Active”).
            // This signal low will reset the device and must be applied to properly initialize the chip.

            Log("Reset()");
            if (_reset != null)
            {
                _reset.Write(GpioPinValue.Low);
                Thread.Sleep(10);
                _reset.Write(GpioPinValue.High);
            }
            else
            {
                cmd(0x01);
            }

        }


        private void cmd(CMD_TYPE command, params CMD_TYPE[] args)
        {
            Log($"cmd({command.ToString("X2")})", args);
            // Set command
            _dc.Write(GpioPinValue.Low);
            _lcd.Write(new CMD_TYPE[] { command });
            WriteData(args, args.Length);
        }

    
        private void WriteData(byte[] args, int len)
        {
            _dc.Write(GpioPinValue.High);
            if (args == null || args.Length == 0) return;
            if (len == _bufferSize)
            {
                _lcd.Write(args);
                return;
            }

            var pos = 0;
            var remaining = len;
            //while (remaining >= 0)
            //{
            //Array.Copy(args, pos, _buffer4k, 0, _bufferSize);
            while (remaining >= _bufferSize)
            {
                var bc = _bufferSize < remaining ? _bufferSize : remaining;
                Array.Copy(args, pos, _buffer4k, 0, bc);
                pos += _bufferSize;
                remaining -= _bufferSize;
                _lcd.Write(_buffer4k);
            }

            if (remaining > 0)
            {
                var rb = new byte[remaining];
                Array.Copy(args, pos, rb, 0, remaining);
                _lcd.Write(rb);
            }

        }

        public void DrawImage(St7789Image image)
        {
            DrawRaw(0, 0, image.Width, image.Height, image.Data);
        }

    }
}