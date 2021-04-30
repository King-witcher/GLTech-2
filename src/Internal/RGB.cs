using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GLTech2
{
    [StructLayout(LayoutKind.Explicit)]
    internal struct RGB
    {
        //Union
        [FieldOffset(0)]
        internal uint rgb;

        [FieldOffset(0)]
        internal byte b;

        [FieldOffset(1)]
        internal byte g;

        [FieldOffset(2)]
        internal byte r;

        [FieldOffset(3)]
        private byte a;

        public float Luminosity => (r + g + b) / (255f * 3f);
        public byte Luminosity256 => (byte)((r + g + b) / 3);

        public RGB Mix(RGB rgb, float factor)
        {
            ushort parcel1, parcel2;

            parcel1 = (ushort)(r * (1 - factor));
            parcel2 = (ushort)(rgb.r * factor);
            rgb.r = (byte)(parcel1 + parcel2);

            parcel1 = (ushort)(g * (1 - factor));
            parcel2 = (ushort)(rgb.g * factor);
            rgb.g = (byte)(parcel1 + parcel2);

            parcel1 = (ushort)(b * (1 - factor));
            parcel2 = (ushort)(rgb.b * factor);
            rgb.b = (byte)(parcel1 + parcel2);

            return rgb;
        }

        public RGB Mix(RGB rgb)
        {
            rgb.r = (byte)((r + rgb.r) >> 1);
            rgb.g = (byte)((g + rgb.g) >> 1);
            rgb.b = (byte)((b + rgb.b) >> 1);

            return rgb;
        }

        public static RGB operator *(RGB rgb, float factor)
        {
            ulong red = (ulong)(rgb.r * factor);
            if (red > 255)
                red = 255;
            ulong green = (ulong)(rgb.g * factor);
            if (green > 255)
                green = 255;
            ulong blue = (ulong)(rgb.b * factor);
            if (blue > 255)
                blue = 255;

            rgb.r = (byte) (red);
            rgb.g = (byte) (green);
            rgb.b = (byte) (blue);

            return rgb;
        }

        public static RGB operator /(RGB color, float divisor)
        {
            return color * (1 / divisor);
        }

        public static implicit operator uint(RGB rgb) => rgb.rgb;
        public static implicit operator RGB(uint rgb) => new RGB{rgb = rgb};
    }
}
