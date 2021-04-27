using System;
using System.Runtime.InteropServices;

namespace GLTech2.Internal
{
    unsafe struct PixelBuffer : IDisposable
    {
        internal int width;
        internal int height;
        internal uint* buffer;

        internal PixelBuffer(int width, int height)
        {
            this.width = width;
            this.height = height;
            buffer = (uint*)Marshal.AllocHGlobal(width * height * sizeof(uint));
        }

        public void Dispose()
        {
            Marshal.FreeHGlobal((IntPtr)buffer);
        }
    }
}
