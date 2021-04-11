using System;
using System.Runtime.InteropServices;

namespace GLTech2
{
    public abstract unsafe class MapElement : IDisposable
    {
        internal void* unmanaged;
        private bool isBound;
        private bool disposed;

        public bool IsBound
        {
            get => isBound;
            internal set => isBound = value;
        }

        internal void Bind(void* newAddress)
        {
            if (isBound)
                throw new InvalidOperationException("MapElement already bound.");
            if (disposed)
                throw new ObjectDisposedException("MapElement");;
            unmanaged = newAddress;
            isBound = true;
        }

        public virtual void Dispose()
        {
            if (disposed)
                return;
            Marshal.FreeHGlobal((IntPtr)unmanaged);
            disposed = true;
        }
    }
}
