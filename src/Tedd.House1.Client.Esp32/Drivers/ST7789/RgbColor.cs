using System;

namespace Tedd.House1.Client.Esp32.Drivers.ST7789
{
    public struct RgbColor
    {
        public Byte R;
        public Byte G;
        public Byte B;

        public static RgbColor FromRGB(Byte r, Byte g, Byte b) => new RgbColor() { R = r, G = g, B = b };


        public static RgbColor From16Bit(UInt16 color) => new RgbColor()
        {
            R = (Byte)(color >> (16 - 5)),
            G = (Byte)((color >> (16 - 5 - 6)) & 0b111111),
            B = (Byte)(color & 0b11111)
        };

        public UInt16 To16Bit()
        {
            return (UInt16)(
                     ((R / 8) << (16 - 5))
                   | ((G / 4) << (16 - 5 - 6))
                   | ((B / 8)));
        }
        public void To16BitBytes(ref byte b1, ref byte b2)
        {
            var u = To16Bit();
            b1 = (byte)(u >> 8);
            b2 = (byte)(u & 0xFF);
        }
    }
}