using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace GLTech2
{
    [StructLayout(LayoutKind.Sequential)]
    internal unsafe struct WallData
    {
        internal Vector geom_direction;
        internal Vector geom_start;
        internal Material material; // Yes, by value.

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static WallData* Alloc(Vector start, Vector end, Material material)
        {
            WallData* result = (WallData*)Marshal.AllocHGlobal(sizeof(WallData));
            result->material = material;
            result->geom_direction = end - start;
            result->geom_start = start;
            return result;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static WallData* Alloc(Vector start, float angle, float length, Material material)
        {
            WallData* result = (WallData*)Marshal.AllocHGlobal(sizeof(WallData));
            Vector dir = new Vector(angle) * length;
            result->material = material;
            result->geom_direction = dir;
            result->geom_start = start;
            return result;
        }
    }
}
