//See Material.cs before
    //See Texture.cs before
//See Vector.cs before

using System.Runtime.InteropServices;

namespace gLTech2
{
    [StructLayout(LayoutKind.Sequential)] 
    internal unsafe struct Sprite_
    {
        Material_ material;
        Vector position;
    }

    internal struct Sprite
    {
        readonly Material Material;
        readonly Vector Position;
    }
}
