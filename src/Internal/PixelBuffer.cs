using System;
using System.Runtime.InteropServices;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace GLTech2
{
    [StructLayout(LayoutKind.Explicit)]
    unsafe struct PixelBuffer : IDisposable
    {
        [FieldOffset(0)]
        internal int width;
        [FieldOffset(4)]
        internal int height;
        [FieldOffset(8)]
        internal uint* uint0;
        [FieldOffset(8)]
        internal RGB* rgb0;

        internal RGB this[int x, int y]
        {
            get => rgb0[x + width * y];
            set => rgb0[x + width * y] = value;
        }

        internal PixelBuffer(int width, int height)
        {
            if (width <= 0 || height <= 0)
                throw new ArgumentOutOfRangeException();

            this.width = width;
            this.height = height;
            rgb0 = null;
            uint0 = (uint*)Marshal.AllocHGlobal(width * height * sizeof(uint));
        }

        internal void Copy(PixelBuffer pb)
        {
            Buffer.MemoryCopy(pb.uint0, this.uint0, 4 * height * width, 4 * height * width);
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

        public void Dispose()
        {
            Marshal.FreeHGlobal((IntPtr)uint0);
        }
    }
}
