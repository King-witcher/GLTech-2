using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace GLTech2
{
    [StructLayout(LayoutKind.Sequential)]
    internal unsafe struct Texture32_ : IDisposable
    {
        internal Int32* buffer;
        internal int height;
        internal int width;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static Texture32_* Alloc(Bitmap bitmap)
        {
            Texture32_* result = (Texture32_*)Marshal.AllocHGlobal(sizeof(Texture32_));

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

        public static implicit operator Texture32_ (Texture32 tex) =>
            *tex.unmanaged;
    }

    public unsafe class Texture32 : IDisposable
    {
        internal Texture32_* unmanaged;

        public Texture32(Bitmap bitmap) =>
            unmanaged = Texture32_.Alloc(bitmap);

        [Obsolete]
        public int this[int x, int y]
        {
            get
            {
                if (x < 0 || y < 0 || x > unmanaged->width || y > unmanaged->height)
                    throw new IndexOutOfRangeException();
                return unmanaged->buffer[unmanaged->width * y + x];
            }
            set
            {
                if (x < 0 || y < 0 || x > unmanaged->width || y > unmanaged->height)
                    throw new IndexOutOfRangeException();
                unmanaged->buffer[unmanaged->width * y + x] = value;
            }
        }

        public void Dispose()
        {
            unmanaged->Dispose();
            Marshal.FreeHGlobal((IntPtr)unmanaged);
        }
    }
}