using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace GLTech2
{
    [StructLayout(LayoutKind.Explicit)]
    public unsafe struct PixelBuffer
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

        private PixelBuffer(uint* buffer, int width, int height)
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
        public PixelBuffer(Bitmap source)
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

        internal PixelBuffer(int width, int height)
        {
            if (width <= 0 || height <= 0)
                throw new ArgumentOutOfRangeException();

            this.width = width;
            this.height = height;
            this.width_float = width;
            this.height_float = height;
            rgb0 = null;
            uint0 = (uint*)Marshal.AllocHGlobal(width * height * sizeof(uint));
        }

        internal void FastCopyFrom(PixelBuffer buffer)
        {
            Buffer.MemoryCopy(buffer.uint0, this.uint0, 4 * height * width, 4 * height * width);
        }

        public void CopyFrom(PixelBuffer buffer)
		{
            if (width != buffer.width || height != buffer.height)
                throw new ArgumentException("Buffers must have the same size.");
            Buffer.MemoryCopy(buffer.uint0, this.uint0, 4 * height * width, 4 * height * width);
        }

        [MethodImpl(methodImplOptions: MethodImplOptions.AggressiveInlining)]
        public void Foreach(Func<RGB, RGB> transformation)
        {
            int height = this.height;
            int width = this.width;
            uint* buffer = this.uint0;

            Parallel.For(0, width, (x) =>
            {
                for (int y = 0; y < height; y++)
                {
                    int cur = width * y + x;
                    buffer[cur] = transformation(buffer[cur]);
                }
            });
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
        public static explicit operator PixelBuffer(Bitmap bitmap)
        {
            return new PixelBuffer(bitmap);
        }

        /// <summary>
        ///     Explicitly casts from Texture to System.Drawing.Bitmap.
        /// </summary>
        /// <param name="texture"></param>
        public static explicit operator Bitmap(PixelBuffer texture)
        {
            return new Bitmap(texture.Width, texture.Height, 4 * texture.Width, texture.PixelFormat, texture.Scan0);
        }

        internal static void Delete(PixelBuffer* item)
        {
            Marshal.FreeHGlobal((IntPtr)item->uint0);
            Marshal.FreeHGlobal((IntPtr)item);
        }
    }
}
