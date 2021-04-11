//See Texture32.cs before

using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security;

namespace GLTech2
{
    [StructLayout(LayoutKind.Sequential)]
    internal unsafe struct Material_
    {
        internal float hoffset;
        internal float hrepeat;
        internal Texture32_ texture; //Propositalmente salvo por valor
        internal float voffset; //Ainda não implementado
        internal float vrepeat; //Ainda não implmenetado

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static Material_* Alloc(float hoffset, float hrepeat, float voffset, float vrepeat, Texture32_* texture)
        {
            Material_* mat = (Material_*)Marshal.AllocHGlobal(sizeof(Material_));
            mat->hoffset = hoffset;
            mat->hrepeat = hrepeat;
            mat->texture = *texture;
            mat->voffset = voffset;
            mat->vrepeat = vrepeat;
            return mat;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal unsafe Int32 MapPixel(float hratio, float vratio)
        {
            int x = (int)(texture.width * (hrepeat * hratio + hoffset)) % texture.width;
            int y = (int)(texture.height * vratio) % texture.height;
            return texture.buffer[texture.width * y + x];
        }

        public static implicit operator Material_ (Material mat)
        {
            return *mat.unmanaged;
        }

        public static implicit operator Material_ (Texture32_ tex)
        {
            var temp = new Material_();
            temp.hoffset = 0f;
            temp.hrepeat = 1f;
            temp.voffset = 0f;
            temp.vrepeat = 1f;
            temp.texture = tex;
            return temp;
        }
    }

    public unsafe class Material : IDisposable
    {
        [SecurityCritical]
        internal Material_* unmanaged;
        private Texture32 refTexture;

        public float HorizontalOffset
        {
            get => unmanaged->hoffset;
            set => unmanaged->hoffset = value;
        }

        public float HorizontalRepeat
        {
            get => unmanaged->hrepeat;
            set => unmanaged->hrepeat = value;
        }
        public Texture32 Texture
        {
            get => refTexture;
            set
            {
                refTexture = value;
                unmanaged->texture = *value.unmanaged;
            }
        }

        public Material(Texture32 texture, float hoffset = 0f, float hrepeat = 1f) =>
            unmanaged = Material_.Alloc(hoffset, hrepeat, 0f, 1f, texture.unmanaged);

        public void Dispose() =>
            Marshal.FreeHGlobal((IntPtr)unmanaged);

        //Estou deixando isso para facilitar a utilização de uma determinada texture em vários materiais sem necessidade de criar
        //inúmeros overloads para métodos. Dessa forma, é possível utilizar um Texture32 em qualquer lugar que se possa utilizar um
        //Material. Note que o mesmo não é válido para Texture32 com Bitmap, para evitar más práticas de gerenciamento de memória
        //e replicações desnecessárias, pois Texture32 possui recursos não gerenciados e o usuário precisará de sua referência para
        //liberá-los.

        public static implicit operator Material(Texture32 texture) =>
            new Material(texture);
    }
}
