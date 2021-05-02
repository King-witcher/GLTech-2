using System;
using System.Drawing;
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
        internal TextureData texture; //Propositalmente salvo por valor
        internal float voffset; //Ainda não implementado
        internal float vrepeat; //Ainda não implmenetado

        public Material(Texture texture, float hoffset = 0f, float hrepeat = 1f, float voffset = 0f, float vrepeat = 1f)
        {
            this.hoffset = hoffset;
            this.hrepeat = hrepeat;
            this.texture = *texture.unmanaged;
            this.voffset = voffset;
            this.vrepeat = vrepeat;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal unsafe uint MapPixel(float hratio, float vratio)      //Possibly optimizable
        {
            // Critical performance impact
            int x = (int)(texture.width_float * (hrepeat * hratio + hoffset)) % texture.width;
            int y = (int)(texture.height_float * vratio);//% texture.height;
            return texture.buffer[texture.width * y + x];
        }

        //Estou deixando isso para facilitar a utilização de uma determinada texture em vários materiais sem necessidade de criar
        //inúmeros overloads para métodos. Dessa forma, é possível utilizar um Texture32 em qualquer lugar que se possa utilizar um
        //Material. Note que o mesmo não é válido para Texture32 com Bitmap, para evitar más práticas de gerenciamento de memória
        //e replicações desnecessárias, pois Texture32 possui recursos não gerenciados e o usuário precisará de sua referência para
        //liberá-los.

        public static implicit operator Material(Texture texture) =>
            new Material(texture);

        public static implicit operator Material(Bitmap texture) =>
            new Material(new Texture(texture));
    }
}
