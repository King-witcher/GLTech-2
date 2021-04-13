
using System;
using System.Runtime.InteropServices;

namespace GLTech2
{
    internal unsafe sealed class Sprite : Element
    {
        internal SpriteData* unmanaged;

        public override Vector Position
        {
            get => unmanaged->position;
            set => unmanaged->position = value;
        }

        public override float Rotation { get; set; }

        public Sprite(Vector position, Material material) =>
            unmanaged = SpriteData.Alloc(position, material);

        public override void Dispose() =>
            Marshal.FreeHGlobal((IntPtr)unmanaged);
    }
}
