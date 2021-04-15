using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using System.Runtime.CompilerServices;

namespace GLTech2
{
    [StructLayout(LayoutKind.Sequential)]
    internal unsafe struct GLBitmapData : IDisposable
    {
        internal Int32* buffer;
        internal int height;
        internal int width;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static GLBitmapData* Alloc(Bitmap bitmap) // Possibly optimizable
        {
            GLBitmapData* result = (GLBitmapData*)Marshal.AllocHGlobal(sizeof(GLBitmapData));

            Rectangle rect = new Rectangle(0, 0, bitmap.Width, bitmap.Height);
            using (var clone = bitmap.Clone(rect, PixelFormat.Format32bppArgb) ??
                throw new ArgumentException("Bitmap parameter cannot be null."))
            {
                var bmpdata = clone.LockBits(rect, ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb);
                int bmpsize = bmpdata.Stride * bmpdata.Height;
                result->buffer = (Int32*)Marshal.AllocHGlobal(bmpsize);
                Buffer.MemoryCopy((void*)bmpdata.Scan0, result->buffer, bmpsize, bmpsize);
                clone.UnlockBits(bmpdata);
            }
            result->width = bitmap.Width;
            result->height = bitmap.Height;
            return result;
        }

        public void Dispose() =>
            Marshal.FreeHGlobal((IntPtr)buffer);

        public static implicit operator GLBitmapData(GLBitmap tex) =>
            *tex.unmanaged;
    }
}
