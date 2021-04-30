using System;
using System.Runtime.InteropServices;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

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

        internal static void Swap(ref PixelBuffer pb1, ref PixelBuffer pb2)
        {
            PixelBuffer tmp = pb1;
            pb1 = pb2;
            pb2 = tmp;
        }

        [MethodImpl(methodImplOptions:MethodImplOptions.AggressiveInlining)]
        internal void Foreach(Func<RGB, RGB> transformation)
        {
            int height = this.height;
            int width = this.width;
            uint* buffer = this.buffer;

            Parallel.For(0, width, (x) =>
            {
                for (int y = 0; y < height; y++)
                {
                    int cur = width * y + x;
                    buffer[cur] = transformation(buffer[cur]);
                }
            });
        }

        public void Dispose()
        {
            Marshal.FreeHGlobal((IntPtr)buffer);
        }
    }
}
