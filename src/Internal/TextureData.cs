using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using System.Runtime.CompilerServices;

namespace GLTech2
{
    [StructLayout(LayoutKind.Explicit)]
    internal unsafe struct TextureData
    {
        [FieldOffset(0)]
        internal int width;
        [FieldOffset(4)]
        internal int height;


        // Theese are stored as float due to small optimizations.
        [FieldOffset(8)]
        internal float height_float;
        [FieldOffset(12)]
        internal float width_float;

        // Union
        [FieldOffset(16)]
        internal uint* uint0;
        [FieldOffset(16)]
        public RGB* rgb0;
        public RGB this[int x, int y]
        {
            get => rgb0[x + width * y];
            set => rgb0[x + width * y] = value;
        }

        private TextureData(uint* buffer, int width, int height)
        {
            this.rgb0 = null;
            this.uint0 = buffer; //Changes rgb0

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
                result->uint0 = (UInt32*)Marshal.AllocHGlobal(bmpsize);
                Buffer.MemoryCopy((void*)bmpdata.Scan0, result->uint0, bmpsize, bmpsize);
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
            Marshal.FreeHGlobal((IntPtr)item->uint0);
            Marshal.FreeHGlobal((IntPtr)item);
        }

        public static implicit operator TextureData(Texture bmp) =>
            *bmp.unmanaged;
    }
}
