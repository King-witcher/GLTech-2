using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace GLTech2
{

    public unsafe class Texture : IDisposable
    {
        internal TextureData* unmanaged;

        public int Height => unmanaged->height;
        public int Width => unmanaged->width;
        public IntPtr Scan0 => (IntPtr) unmanaged->buffer;
        public PixelFormat PixelFormat => PixelFormat.Format32bppArgb;

        public Texture(Bitmap texture) =>
            unmanaged = TextureData.Create(texture);
        public void Dispose()
        {
            TextureData.Delete(unmanaged);
        }

        public static implicit operator Texture(Bitmap bitmap)
        {
            return new Texture(bitmap);
        }

        public static implicit operator Bitmap(Texture texture)
        {
            return new Bitmap(texture.Width, texture.Height, 4 * texture.Width, texture.PixelFormat, texture.Scan0);
        }

        ~Texture()
        {
            Marshal.FreeHGlobal((IntPtr)unmanaged);
        }
    }
}