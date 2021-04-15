#pragma warning disable IDE1006

using GLTech2.Properties;
using System;
using System.Runtime.InteropServices;

namespace GLTech2
{
    internal unsafe class Camera : Element
    {
        public Camera()
        {
            UpdateRelative();
        }

        internal RenderStruct* unmanaged;
        private readonly Random random = new Random();

        //Public value
        private float rotation = 0f;
        private protected override Vector AbsolutePosition
        {
            get => unmanaged->camera_position;
            set => unmanaged->camera_position = value;
        }

        private protected override Vector AbsoluteNormal
        {
            get => throw new NotImplementedException(); // Public value
            set
            {
                throw new NotImplementedException();
            }
        }

        public void Dispose()
        {
            unmanaged->Free();
            Marshal.FreeHGlobal((IntPtr)unmanaged);
        }
    }
}