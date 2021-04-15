using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace GLTech2
{

    public unsafe class GLBitmap : IDisposable
    {
        internal GLBitmapData* unmanaged;

        public int Height => unmanaged->height;
        public int Width => unmanaged->width;
        public IntPtr Scan0 => (IntPtr) unmanaged->buffer;
        public PixelFormat PixelFormat => PixelFormat.Format32bppRgb;

        public GLBitmap(Bitmap bitmap) =>
            unmanaged = GLBitmapData.Alloc(bitmap);
        public void Dispose()
        {
            unmanaged->Dispose();
            Marshal.FreeHGlobal((IntPtr)unmanaged);
        }

        public static explicit operator GLBitmap(Bitmap bitmap)
        {
            return new GLBitmap(bitmap);
        }

        public static implicit operator Bitmap(GLBitmap texture)
        {
            return new Bitmap(texture.Width, texture.Height, 4 * texture.Width, texture.PixelFormat, texture.Scan0);
        }

        ~GLBitmap()
        {
            Marshal.FreeHGlobal((IntPtr)unmanaged);
        }
    }
}