using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using System.Runtime.CompilerServices;

namespace GLTech2
{
    [StructLayout(LayoutKind.Sequential)]
    internal unsafe struct TextureData
    {
        internal uint* buffer;
        internal int height;
        internal int width;

        // Theese are stored as float due to small optimizations.
        internal float height_float;
        internal float width_float;

        private TextureData(uint* buffer, int width, int height)
        {
            this.buffer = buffer;
            this.height = height;
            this.width = width;
            this.height_float = height;
            this.width_float = width;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static TextureData* Create(Bitmap bitmap) // Possibly optimizable
        {
            TextureData* result = (TextureData*)Marshal.AllocHGlobal(sizeof(TextureData));

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
            result->width_float = bitmap.Width;
            result->height_float = bitmap.Height;
            return result;
        }

        internal static void Delete(TextureData* item)
        {
            Marshal.FreeHGlobal((IntPtr)item->buffer);
            Marshal.FreeHGlobal((IntPtr)item);
        }

        public static implicit operator TextureData(Texture bmp) =>
            *bmp.unmanaged;
    }
}
