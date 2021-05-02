using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GLTech2
{
    static class Utilities
    {
        public static void Clip<T>(ref T value, T min, T max) where T : struct, IComparable<T>
        {
            if (value.CompareTo(max) > 0)
                value = max;
            else if (value.CompareTo(min) < 0)
                value = min;
        }
    }
}
