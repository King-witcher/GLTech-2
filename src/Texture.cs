using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace GLTech2
{
    /// <summary>
    ///     Stores a 32-bits-per-pixel buffer that represents an image.
    /// </summary>
    public unsafe class Texture : IDisposable
    {
        internal TextureData* unmanaged;

        /// <summary>
        ///     Gets the height of the image.
        /// </summary>
        public int Height => unmanaged->height;

        /// <summary>
        ///     Gets the witdh of the image.
        /// </summary>
        public int Width => unmanaged->width;

        /// <summary>
        ///     Gets an IntPtr that represents a pointer to the first pixel of the buffer.
        /// </summary>
        public IntPtr Scan0 => (IntPtr) unmanaged->uint0;
        public PixelFormat PixelFormat => PixelFormat.Format32bppArgb;

        /// <summary>
        ///     Gets a new instance of Texture equivalent to the specified bitmap.
        /// </summary>
        /// <remarks>
        ///     Instantiating a new Texture is not a boxing, but cloning operation.
        /// </remarks>
        /// <param name="source">Source</param>
        public Texture(Bitmap source) =>
            unmanaged = TextureData.Create(source);

        /// <summary>
        ///     Releases all unmanaged data.
        /// </summary>
        public void Dispose()
        {
            TextureData.Delete(unmanaged);
        }

        /// <summary>
        ///     Explicitly casts from System.Drawing.Bitmap to Texture.
        /// </summary>
        /// <param name="bitmap">Bitmap to be cast</param>
        public static explicit operator Texture(Bitmap bitmap)
        {
            return new Texture(bitmap);
        }

        /// <summary>
        ///     Explicitly casts from Texture to System.Drawing.Bitmap.
        /// </summary>
        /// <param name="texture"></param>
        public static explicit operator Bitmap(Texture texture)
        {
            return new Bitmap(texture.Width, texture.Height, 4 * texture.Width, texture.PixelFormat, texture.Scan0);
        }

        ~Texture()
        {
            Marshal.FreeHGlobal((IntPtr)unmanaged);
        }
    }
}