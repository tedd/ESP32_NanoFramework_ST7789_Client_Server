using System;
using Windows.Devices.Spi;

namespace Tedd.House1.Client.Esp32.Drivers.ST7789
{

    public enum PixelFormat : byte
    {
        RGB_444_12Bit = 0b011,
        RGB_565_16Bit = 0b101,
        RGB_666_18Bit = 0b110
    }

    public class St7789Image
    {
        public readonly byte[] Data;
        public readonly PixelFormat PixelFormat;
        public readonly int Width;
        public readonly int Height;

        public St7789Image(int width, int height, PixelFormat pixelFormat)
        {
            Width = width;
            Height = height;
            PixelFormat = pixelFormat;

            var pixelCount = width * height;
            if (pixelFormat == PixelFormat.RGB_444_12Bit)
                Data = new byte[(int)((pixelCount * 12 + 7) / 8)];
            else if (pixelFormat == PixelFormat.RGB_565_16Bit)
                Data = new byte[pixelCount + pixelCount];
            else if (pixelFormat == PixelFormat.RGB_666_18Bit)
                Data = new byte[(int)((pixelCount * 18 + 7) / 8)];
        }

        public RgbColor Get(int x, int y)
        {

            if (PixelFormat == PixelFormat.RGB_565_16Bit)
            {
                var i = y * (Width * 2) + (x * 2);
                return RgbColor.From16Bit((UInt16)((Data[i] << 8) | Data[i + 1]));
            }

            // TODO: Other pixel formats.
            throw new Exception("Color bit format not implemented.");
        }

        public void Set(int x, int y, RgbColor color)
        {
            if (PixelFormat == PixelFormat.RGB_565_16Bit)
            {
                var i = y * (Width * 2) + (x * 2);
                color.To16BitBytes(ref Data[i], ref Data[i + 1]);
                //var c = color.To16Bit();
                //Data[i] = (byte)((c >> 8) & 0xFF);
                //Data[i + 1] = (byte)(c & 0xFF);
                return;
            }

            // TODO: Other pixel formats.
            throw new Exception("Color bit format not implemented.");
        }

        public void DrawLine(int x1, int y1, int x2, int y2, RgbColor color, int lineWidth = 1)
        {

            if (x1 > x2)
            {
                Swap(ref x1, ref x2);
                Swap(ref y1, ref y2);
            }

            var dx = x2 - x1;
            var dy = y2 - y1;

            for (var x = x1; x < x2; x++)
            {
                var y = y1 + dy * (x - x1) / dx;
                Set(x, y, color);
            }

            return;
            //int dx = Abs(x2 - x1), sx = x1 < x2 ? 1 : -1;
            //int dy = Abs(y2 - y1), sy = y1 < y2 ? 1 : -1;
            //int err = (dx > dy ? dx : -dy) / 2, e2;
            //var lws = (int)(lineWidth / 2) * -1;
            //var lwe = lineWidth + lws;
            //for (; ; )
            //{
            //    if (lineWidth == 1)
            //        Set(x1, y1, color);
            //    else if (lineWidth > 1)
            //    {
            //        // Not very efficient
            //        for (var lwx = lws; lwx < lwe; lwx++)
            //        {
            //            var xlw = x1 + lwx;
            //            if (xlw > 0 && xlw < Width)
            //            {
            //                for (var lwy = lws; lwy < lwe; lwy++)
            //                {
            //                    var ylw = y1 + lwy;
            //                    if (ylw > 0 && ylw < Height)
            //                        Set(xlw, ylw, color);
            //                }
            //            }
            //        }
            //    }
            //    if (x1 == x2 && y1 == y2) break;
            //    e2 = err;
            //    if (e2 > -dx) { err -= dy; x1 += sx; }
            //    if (e2 < dy) { err += dx; y1 += sy; }
            //}
        }

        public void DrawRectangle(int x1, int y1, int x2, int y2, RgbColor color, int lineWidth = 1)
        {
            DrawLine(x1, y1, x2, y1, color, lineWidth);
            DrawLine(x2, y1, x2, y2, color, lineWidth);
            DrawLine(x2, y2, x1, y2, color, lineWidth);
            DrawLine(x1, y2, x1, y1, color, lineWidth);
        }

        public void DrawFilledRectangle(int x1, int y1, int x2, int y2, RgbColor color)
        {
            if (x1 > x2)
                Swap(ref x1, ref x2);
            if (y1 > y2)
                Swap(ref y1, ref y2);


            for (var x = x1; x < x2; x++)
            {
                for (var y = y1; y < y2; y++)
                {
                    Set(x, y, color);
                }
            }
        }

        public static int Abs(int i) => i < 0 ? i * -1 : i;
        public static void Swap(ref int i1, ref int i2)
        {
            var ti = i1;
            i1 = i2;
            i2 = ti;
        }

        public void Clear() => Array.Clear(Data, 0, Data.Length);

        //public unsafe void Fill(RgbColor color)
        //{
        //    if (PixelFormat == PixelFormat.RGB_565_16Bit)
        //    {
        //        var c = color.To16Bit();
        //        fixed (byte* p = Data)
        //        {
        //            for (var i = 0; i < Data.Length / 2; i++)
        //                *((UInt16*)(i)) = c;
        //        }

        //        return;
        //    }

        //    // TODO: Other pixel formats.
        //    throw new Exception("Color bit format not implemented.");
        //}
    }
}
