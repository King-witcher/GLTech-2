using System.Drawing;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace GLTech2
{
    /// <summary>
    ///     Contains information about how a texture should be offset, resized and allows to add visual aspect to a visible object.
    /// </summary>
    /// <remarks>
    /// <para>
    ///     Material is a fleyweight object that refers to a texture and stores data about offset and resizing of the same.
    /// </para>
    /// </remarks>
    [StructLayout(LayoutKind.Sequential)]
    public unsafe struct Texture
    {
        internal float hoffset;
        internal float hrepeat;
        internal TextureBuffer texture; //Propositalmente salvo por valor
        internal float voffset; //Ainda não implementado
        internal float vrepeat; //Ainda não implmenetado

        /// <summary>
        ///     Gets a new instance of material given its texture.
        /// </summary>
        /// <param name="texture">A texture</param>
        /// <param name="hoffset">Horizontal offset of the texture</param>
        /// <param name="hrepeat">How many times the texture is repeated horizontally</param>
        /// <param name="voffset">Not implemented yet</param>
        /// <param name="vrepeat">Not implemented yet</param>
        public Texture(TextureBuffer texture, float hoffset = 0f, float hrepeat = 1f, float voffset = 0f, float vrepeat = 1f)
        {
            this.hoffset = hoffset;
            this.hrepeat = hrepeat;
            this.texture = texture;
            this.voffset = voffset;
            this.vrepeat = vrepeat;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal unsafe uint MapPixel(float hratio, float vratio)      //Possibly optimizable
        {
            // Critical performance impact
            int x = (int)(texture.width_float * (hrepeat * hratio + hoffset)) % texture.width;
            int y = (int)(texture.height_float * vratio);//% texture.height;
            return texture.uint0[texture.width * y + x];
        }

        //Estou deixando isso para facilitar a utilização de uma determinada texture em vários materiais sem necessidade de criar
        //inúmeros overloads para métodos. Dessa forma, é possível utilizar um Texture32 em qualquer lugar que se possa utilizar um
        //Material. Note que o mesmo não é válido para Texture32 com Bitmap, para evitar más práticas de gerenciamento de memória
        //e replicações desnecessárias, pois Texture32 possui recursos não gerenciados e o usuário precisará de sua referência para
        //liberá-los.

        /// <summary>
        ///     Implicitly casts a texture instance to a Material.
        /// </summary>
        /// <param name="texture">Texture to be cast</param>
        public static implicit operator Texture(TextureBuffer texture) =>
            new Texture(texture);

        /// <summary>
        ///     Explicitly casts a System.Drawing.Bitmap to a Material.
        /// </summary>
        /// <param name="bitmap">Bitmap to be cast</param>
        /// <remarks>
        ///     Note that casting bitmaps directly to Materials will anonymously create a new Texture object that uses unmanaged resources each time it's done, but you won't be able to dispose them.
        /// </remarks>
        public static explicit operator Texture(Bitmap bitmap) =>
            new Texture(new TextureBuffer(bitmap));
    }
}
