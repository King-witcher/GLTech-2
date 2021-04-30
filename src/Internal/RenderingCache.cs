using System;
using System.Runtime.InteropServices;

namespace GLTech2
{
    /// <summary>
    /// Stores important cache data to the renderer that can be shared with native modes.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    internal unsafe struct RenderingCache : IDisposable
    {
        internal float* angles;
        internal float colHeight1;
        internal float* cosines;

        internal float FOV;

        internal static RenderingCache* Create(int width, int height, float FOV = 90f)
        {
            RenderingCache* result = (RenderingCache*)Marshal.AllocHGlobal(sizeof(RenderingCache));
            result->FOV = FOV;
            result->angles = null;
            result->cosines = null; //Atribuição possivelmente desnecessária.

            result->RefreshCaches(width, height);
            return result;
        }

        private void RefreshCaches(int width, int height)
        {
            const double TORAD = Math.PI / 180.0f;
            double tan = Math.Tan(TORAD * FOV / 2);

            colHeight1 = width / (float)(2 * tan);

            //Ambos os caches são alocados lado a lado para facilitar o chaching em nível de processador.
            if (angles != null)
                Marshal.ReAllocHGlobal((IntPtr)angles, (IntPtr)(2 * sizeof(float) * width));
            else
                angles = (float*)Marshal.AllocHGlobal(2 * sizeof(float) * width);
            cosines = angles + width;

            double step = 2 * tan / (width - 1);
            for (int i = 0; i < width; i++)
            {
                float angle = (float)(Math.Atan(i * step - tan) / TORAD);
                angles[i] = angle;
                cosines[i] = (float)(Math.Cos(TORAD * angle));
            }
        }

        public void Dispose()
        {
            Marshal.FreeHGlobal((IntPtr)angles);
        }
    }
}
