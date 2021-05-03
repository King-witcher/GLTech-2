using System;
using System.Runtime.InteropServices;

namespace GLTech2
{
    unsafe static class Utilities
    {

        public const double ToRad = Math.PI / 180f;

        public static void Clip<T>(ref T value, T min, T max) where T : struct, IComparable<T>
        {
            if (value.CompareTo(max) > 0)
                value = max;
            else if (value.CompareTo(min) < 0)
                value = min;
        }
    }
}
