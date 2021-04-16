#pragma warning disable IDE1006

using GLTech2.Properties;
using System;
using System.Runtime.InteropServices;

namespace GLTech2
{
    // Testar camera usando normal ao inv[es de angulo
    // Testar colocar camera como struct

    [StructLayout(LayoutKind.Sequential)]
    internal unsafe struct CameraData
    {
        internal float camera_angle; //MUST be 0 <= x < 360
        internal float camera_HFOV;
        internal Vector camera_position;

        static internal CameraData* Create(Vector position, float rotation, float fov)
        {
            CameraData* result = (CameraData*) Marshal.AllocHGlobal(sizeof(CameraData));
            result->camera_position = position;
            result->camera_angle = rotation;
            result->camera_HFOV = fov;
            return result;
        }

        static internal void Delete(CameraData* item)
        {
            Marshal.FreeHGlobal((IntPtr)item);
        }
    }

    public unsafe class Camera : Element
    {
        internal CameraData* unmanaged;

        public Camera(Vector position, float rotation = 0f, float fov = 90f)
        {
            unmanaged = CameraData.Create(position, rotation, fov);

            UpdateRelative();   // O cumulo ter que fazer isso
        }

        private protected override Vector AbsolutePosition
        {
            get => unmanaged->camera_position;
            set => unmanaged->camera_position = value;
        }

        private protected override Vector AbsoluteNormal
        {
            get
            {
                return new Vector(unmanaged->camera_angle);
            }
            set
            {
                unmanaged->camera_angle = value.Angle;
            }
        }

        public override void Dispose()
        {
            CameraData.Delete(unmanaged);
        }
    }
}