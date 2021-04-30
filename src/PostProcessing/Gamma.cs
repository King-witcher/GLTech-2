using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GLTech2.PostProcessing
{
    internal unsafe sealed class Gamma : Effect
    {
        Random rnd = new Random(5);
        internal override void Process(PixelBuffer target)
        {
            Parallel.For(0, target.width, (x) =>
            {
                for (int y = 0; y < target.height; y++)
                {
                    int cur = target.width * y + x;
                }
            });

            byte gamma(byte pixel1)
            {
                double f = pixel1 / 255.0;
                f = Math.Pow(f, 2);
                return (byte)(255.0 * f);
            }
        }
    }
}
