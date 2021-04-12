//See Material.cs before.
    //See Texture.cs before.
//See Vector.cs before.
using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace GLTech2
{
    [StructLayout(LayoutKind.Sequential)] 
    internal unsafe struct Sprite_
    {
        internal Vector position;
        internal Material_ material;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static Sprite_* Alloc(Vector position, Material_* material)
        {
            Sprite_* result = (Sprite_*)Marshal.AllocHGlobal(sizeof(Sprite_));
            result->position = position;
            result->material = *material;
            return result;
        }

        public static implicit operator Sprite_(Sprite sprite) =>
            *sprite.unmanaged;
    }

    internal unsafe sealed class Sprite : IDisposable
    {
        internal Sprite_* unmanaged;

        public Vector Position
        {
            get => unmanaged->position;
            set => unmanaged->position = value;
        }

        public float Rotation { get; set; }

        public Sprite(Vector position, Material material = null) =>
            unmanaged = Sprite_.Alloc(position, material.unmanaged);

        public void Dispose() =>
            Marshal.FreeHGlobal((IntPtr)unmanaged);
    }
}
