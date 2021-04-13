﻿using System.Runtime.InteropServices;
using System.Runtime.CompilerServices;

namespace GLTech2
{
    [StructLayout(LayoutKind.Sequential)]
    internal unsafe struct SpriteData
    {
        internal Vector position;
        internal Material material;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static SpriteData* Alloc(Vector position, Material material)
        {
            SpriteData* result = (SpriteData*)Marshal.AllocHGlobal(sizeof(SpriteData));
            result->position = position;
            result->material = material;
            return result;
        }

        public static implicit operator SpriteData(Sprite sprite) =>
            *sprite.unmanaged;
    }
}