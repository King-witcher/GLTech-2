using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using System.Runtime.CompilerServices;

namespace GLTech2
{
    [StructLayout(LayoutKind.Explicit)]
    public unsafe struct TextureBuffer
    {
        [FieldOffset(0)]
        public int width;
        [FieldOffset(4)]
        public int height;

        // Theese are stored as float due to small optimizations.
        [FieldOffset(8)]
        internal float height_float;
        [FieldOffset(12)]
        internal float width_float;

        // Union
        [FieldOffset(16)]
        public uint* uint0;
        [FieldOffset(16)]
        public RGB* rgb0;

        public RGB this[int x, int y]
        {
            get => rgb0[x + width * y];
            set => rgb0[x + width * y] = value;
        }

        private TextureBuffer(uint* buffer, int width, int height)
        {
            this.rgb0 = null;
            this.uint0 = buffer; //Changes rgb0

            this.height = height;
            this.width = width;
            this.height_float = height;
            this.width_float = width;
        }

        /// <summary>
        ///     Gets the height of the image.
        /// </summary>
        public int Height => height;

        /// <summary>
        ///     Gets the witdh of the image.
        /// </summary>
        public int Width => width;

        /// <summary>
        ///     Gets an IntPtr that represents a pointer to the first pixel of the buffer.
        /// </summary>
        public IntPtr Scan0 => (IntPtr)uint0;
        public PixelFormat PixelFormat => PixelFormat.Format32bppArgb;

        /// <summary>
        ///     Gets a new instance of Texture equivalent to the specified bitmap.
        /// </summary>
        /// <remarks>
        ///     Instantiating a new Texture is not a boxing, but cloning operation.
        /// </remarks>
        /// <param name="source">Source</param>
        public TextureBuffer(Bitmap source)
        {

            Rectangle rect = new Rectangle(0, 0, source.Width, source.Height);
            using (var clone = source.Clone(rect, PixelFormat.Format32bppArgb) ??
                throw new ArgumentException("Bitmap parameter cannot be null."))
            {
                var bmpdata = clone.LockBits(rect, ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb);
                int bmpsize = bmpdata.Stride * bmpdata.Height;
                rgb0 = null; // Assigned next line
                uint0 = (UInt32*)Marshal.AllocHGlobal(bmpsize);
                Buffer.MemoryCopy((void*)bmpdata.Scan0, uint0, bmpsize, bmpsize);
                clone.UnlockBits(bmpdata);
            }
            width = source.Width;
            height = source.Height;
            width_float = source.Width;
            height_float = source.Height;
        }

        /// <summary>
        ///     Releases all unmanaged data.
        /// </summary>
        public void Dispose()
        {
            Marshal.FreeHGlobal(Scan0);
        }

        /// <summary>
        ///     Explicitly casts from System.Drawing.Bitmap to Texture.
        /// </summary>
        /// <param name="bitmap">Bitmap to be cast</param>
        public static explicit operator TextureBuffer(Bitmap bitmap)
        {
            return new TextureBuffer(bitmap);
        }

        /// <summary>
        ///     Explicitly casts from Texture to System.Drawing.Bitmap.
        /// </summary>
        /// <param name="texture"></param>
        public static explicit operator Bitmap(TextureBuffer texture)
        {
            return new Bitmap(texture.Width, texture.Height, 4 * texture.Width, texture.PixelFormat, texture.Scan0);
        }

        internal static void Delete(TextureBuffer* item)
        {
            Marshal.FreeHGlobal((IntPtr)item->uint0);
            Marshal.FreeHGlobal((IntPtr)item);
        }
    }
}
