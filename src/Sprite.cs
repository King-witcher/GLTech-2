
using System;
using System.Runtime.InteropServices;

namespace GLTech2
{
    internal unsafe sealed class Sprite : Element
    {
        internal SpriteData* unmanaged;

        private protected override Vector IsolatedPosition
        {
            get => unmanaged->position;
            set => unmanaged->position = value;
        }

        private protected override float IsolatedRotation { get; set; }

        public Sprite(Vector position, Material material) =>
            unmanaged = SpriteData.Alloc(position, material);

        public override void Dispose() =>
            Marshal.FreeHGlobal((IntPtr)unmanaged);
    }
}
