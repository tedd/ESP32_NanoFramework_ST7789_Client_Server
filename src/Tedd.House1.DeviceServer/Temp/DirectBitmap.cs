//using System;
//using System.Drawing;
//using System.Runtime.InteropServices;

//namespace Tedd.House1.DeviceServer.Temp
//{
//    public class DirectBitmap : IDisposable
//    {
//        public Bitmap Bitmap { get; private set; }
//        public byte[] Bits { get; private set; }
//        public bool Disposed { get; private set; }
//        public int Height { get; private set; }
//        public int Width { get; private set; }

//        protected GCHandle BitsHandle { get; private set; }

//        public DirectBitmap(int width, int height)
//        {
//            Width = width;
//            Height = height;
//            Bits = new byte[width * height*2];
//            BitsHandle = GCHandle.Alloc(Bits, GCHandleType.Pinned);
//            Bitmap = new Bitmap(width, height, width * 4, System.Drawing.Imaging.PixelFormat.Format16bppRgb565, BitsHandle.AddrOfPinnedObject());
            
//        }

//        //public void SetPixel(int x, int y, RgbColor color)
//        //{
//        //    int index = x + (y * Width);
            

//        //    Bits[index] = color.;
//        //}

//        //public Color GetPixel(int x, int y)
//        //{
//        //    int index = x + (y * Width);
//        //    int col = Bits[index];
//        //    Color result = Color.FromArgb(col);

//        //    return result;
//        //}

//        public void Dispose()
//        {
//            if (Disposed) return;
//            Disposed = true;
//            Bitmap.Dispose();
//            BitsHandle.Free();
//        }
//    }
//}