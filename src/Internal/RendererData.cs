using System;
using System.Runtime.InteropServices;

namespace GLTech2
{
    [StructLayout(LayoutKind.Sequential)]
    internal unsafe struct RendererData
    {
        // Resposabillity of the Renderer
        internal volatile Int32* bitmap_buffer;
        internal int bitmap_height;
        internal int bitmap_width;

        // RenderingCache
        internal float* cache_angles;
        internal float cache_colHeight1;
        internal float* cache_cosines;


        internal float camera_HFOV;
        internal Vector camera_position;
        internal float camera_angle; //MUST be 0 <= x < 360
        internal Vector camera_normal;      // Not used yet

        // Responsabillity of the Renderer (current_scene)
        internal SceneData* scene;

        internal static RendererData* Create(int width, int height, SceneData* scene)
        {
            RendererData* result = (RendererData*)Marshal.AllocHGlobal(sizeof(RendererData));
            result->bitmap_height = height;
            result->bitmap_width = width;
            result->bitmap_buffer = (Int32*)Marshal.AllocHGlobal(sizeof(Int32) * width * height);
            result->camera_angle = 0f;
            result->camera_HFOV = 90f;
            result->scene = scene;
            result->camera_position = Vector.Origin;
            result->cache_angles = null;
            result->cache_cosines = null; //Atribuição possivelmente desnecessária.

            result->RefreshCaches();
            return result;
        }

        public void Free()
        {
            Marshal.FreeHGlobal((IntPtr)cache_angles); //Cossenos estão incluídos porque ângulos e cossenos são alocados contiguamente.
            Marshal.FreeHGlobal((IntPtr)bitmap_buffer);
        }

        internal void RefreshCaches()
        {
            const double TORAD = Math.PI / 180.0f;
            double tan = Math.Tan(TORAD * camera_HFOV / 2);

            cache_colHeight1 = bitmap_width / (float)(2 * tan);

            //Ambos os caches são alocados lado a lado para facilitar o chaching em nível de processador.
            if (cache_angles != null)
                Marshal.ReAllocHGlobal((IntPtr)cache_angles, (IntPtr)(2 * sizeof(float) * bitmap_width));
            else
                cache_angles = (float*)Marshal.AllocHGlobal(2 * sizeof(float) * bitmap_width);
            cache_cosines = cache_angles + bitmap_width;

            double step = 2 * tan / (bitmap_width - 1);
            for (int i = 0; i < bitmap_width; i++)
            {
                float angle = (float)(Math.Atan(i * step - tan) / TORAD);
                cache_angles[i] = angle;
                cache_cosines[i] = (float)(Math.Cos(TORAD * angle));
            }
        }
    }
}
