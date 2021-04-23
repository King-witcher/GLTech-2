using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using System.Runtime.CompilerServices;

namespace GLTech2
{
    [StructLayout(LayoutKind.Sequential)]
    internal unsafe struct GLBitmapData
    {
        internal UInt32* buffer;
        internal int height;
        internal int width;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static GLBitmapData* Create(Bitmap bitmap) // Possibly optimizable
        {
            GLBitmapData* result = (GLBitmapData*)Marshal.AllocHGlobal(sizeof(GLBitmapData));

            Rectangle rect = new Rectangle(0, 0, bitmap.Width, bitmap.Height);
            using (var clone = bitmap.Clone(rect, PixelFormat.Format32bppArgb) ??
                throw new ArgumentException("Bitmap parameter cannot be null."))
            {
                var bmpdata = clone.LockBits(rect, ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb);
                int bmpsize = bmpdata.Stride * bmpdata.Height;
                result->buffer = (UInt32*)Marshal.AllocHGlobal(bmpsize);
                Buffer.MemoryCopy((void*)bmpdata.Scan0, result->buffer, bmpsize, bmpsize);
                clone.UnlockBits(bmpdata);
            }
            result->width = bitmap.Width;
            result->height = bitmap.Height;
            return result;
        }

        internal static void Delete(GLBitmapData* item)
        {
            Marshal.FreeHGlobal((IntPtr)item->buffer);
            Marshal.FreeHGlobal((IntPtr)item);
        }

        public static implicit operator GLBitmapData(GLBitmap bmp) =>
            *bmp.unmanaged;
    }
}
