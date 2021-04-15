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

        }

        internal RenderStruct* unmanaged;
        private readonly Random random = new Random();

        //Public value
        private float rotation = 0f;
        private protected override Vector IsolatedPosition
        {
            get => unmanaged->camera_position;
            set => unmanaged->camera_position = value;
        }

        private protected override float IsolatedRotation
        {
            get => rotation; // Public value
            set
            {
                rotation = value;
                //Ensures the angle restriction
                if (value >= 0)
                    unmanaged->camera_angle = value % 360;
                else
                    unmanaged->camera_angle = value % 360 + 1;
            }
        }

        public void Dispose()
        {
            unmanaged->Free();
            Marshal.FreeHGlobal((IntPtr)unmanaged);
        }
    }
}