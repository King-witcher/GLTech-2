//See Texture32.cs before

using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security;

namespace GLTech2
{
    [StructLayout(LayoutKind.Sequential)]
    public unsafe struct Material
    {
        internal float hoffset;
        internal float hrepeat;
        internal GLBitmapData texture; //Propositalmente salvo por valor
        internal float voffset; //Ainda não implementado
        internal float vrepeat; //Ainda não implmenetado

        public float HorizontalOffset
        {
            get => hoffset;
            set => hoffset = value;
        }

        public float HorizontalRepeat
        {
            get => hrepeat;
            set => hrepeat = value;
        }

        public Material(GLBitmap texture, float hoffset = 0f, float hrepeat = 1f, float voffset = 0f, float vrepeat = 1f)
        {
            this.hoffset = hoffset;
            this.hrepeat = hrepeat;
            this.texture = *texture.unmanaged;
            this.voffset = voffset;
            this.vrepeat = vrepeat;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal unsafe Int32 MapPixel(float hratio, float vratio)
        {
            int x = (int)(texture.width * (hrepeat * hratio + hoffset)) % texture.width;
            int y = (int)(texture.height * vratio) % texture.height;
            return texture.buffer[texture.width * y + x];
        }

        //Estou deixando isso para facilitar a utilização de uma determinada texture em vários materiais sem necessidade de criar
        //inúmeros overloads para métodos. Dessa forma, é possível utilizar um Texture32 em qualquer lugar que se possa utilizar um
        //Material. Note que o mesmo não é válido para Texture32 com Bitmap, para evitar más práticas de gerenciamento de memória
        //e replicações desnecessárias, pois Texture32 possui recursos não gerenciados e o usuário precisará de sua referência para
        //liberá-los.

        public static implicit operator Material(GLBitmap texture) =>
            new Material(texture);
    }
}
