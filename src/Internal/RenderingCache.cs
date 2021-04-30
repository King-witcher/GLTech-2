using System;
using System.Runtime.InteropServices;

namespace GLTech2
{
    [StructLayout(LayoutKind.Sequential)]
    internal unsafe struct RenderingCache : IDisposable
    {
        internal float* cache_angles;
        internal float cache_colHeight1;
        internal float* cache_cosines;


        internal float camera_HFOV;

        internal static RenderingCache* Create(int width, int height)
        {
            RenderingCache* result = (RenderingCache*)Marshal.AllocHGlobal(sizeof(RenderingCache));
            result->camera_HFOV = 90f;
            result->cache_angles = null;
            result->cache_cosines = null; //Atribuição possivelmente desnecessária.

            result->RefreshCaches(width, height);
            return result;
        }

        private void RefreshCaches(int width, int height)
        {
            const double TORAD = Math.PI / 180.0f;
            double tan = Math.Tan(TORAD * camera_HFOV / 2);

            cache_colHeight1 = width / (float)(2 * tan);

            //Ambos os caches são alocados lado a lado para facilitar o chaching em nível de processador.
            if (cache_angles != null)
                Marshal.ReAllocHGlobal((IntPtr)cache_angles, (IntPtr)(2 * sizeof(float) * width));
            else
                cache_angles = (float*)Marshal.AllocHGlobal(2 * sizeof(float) * width);
            cache_cosines = cache_angles + width;

            double step = 2 * tan / (width - 1);
            for (int i = 0; i < width; i++)
            {
                float angle = (float)(Math.Atan(i * step - tan) / TORAD);
                cache_angles[i] = angle;
                cache_cosines[i] = (float)(Math.Cos(TORAD * angle));
            }
        }

        public void Dispose()
        {
            Marshal.FreeHGlobal((IntPtr)cache_angles);
        }
    }
}
