using System;
using System.Runtime.InteropServices;
using System.Runtime.CompilerServices;

namespace GLTech2
{
    [StructLayout(LayoutKind.Sequential)]
    unsafe struct POVData
    {
        internal Vector position;
        internal float rotation; //MUST be 0 <= x < 360

        static internal POVData* Create(Vector position, float rotation) // a little bit optimizable
        {
            POVData* result = (POVData*)Marshal.AllocHGlobal(sizeof(POVData));
            result->position = position;
            result->rotation = rotation;
            return result;
        }

        static internal void Delete(POVData* item)
        {
            Marshal.FreeHGlobal((IntPtr)item);
        }
    }

    public unsafe class Observer : Element, IDisposable
    {
        internal POVData* unmanaged;

        public Observer(Vector position, float rotation)
        {
            unmanaged = POVData.Create(Position, rotation);

            UpdateRelative();
        }

        private protected override Vector AbsolutePosition
        {
            get => unmanaged->position;
            set => unmanaged->position = value;
        }

        private protected override Vector AbsoluteNormal
        {
            get => new Vector(unmanaged->rotation);
            set => unmanaged->rotation = value.Angle;
        }

        public override void Dispose()
        {
            POVData.Delete(unmanaged);
            unmanaged = null;
        }
    }
}
