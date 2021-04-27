using System;
using System.Runtime.InteropServices;

namespace GLTech2
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

        internal void Clone(PixelBuffer pb)
        {
            Buffer.MemoryCopy(pb.buffer, this.buffer, 4 * height * width, 4 * height * width);
        }

        public void Dispose()
        {
            Marshal.FreeHGlobal((IntPtr)buffer);
        }
    }
}
