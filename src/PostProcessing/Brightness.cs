using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GLTech2.PostProcessing
{
    public unsafe sealed class Brightness : Effect
    {
        float factor;
        public float Factor
        {
            get => factor;
            set => factor = value;
        }

        public Brightness(float factor)
        {
            this.factor = factor;
        }

        internal override void Process(PixelBuffer target)
        {
            Parallel.For(0, target.width, (x) =>
            {
                for (int y = 0; y < target.height; y++)
                {
                    int cur = target.width * y + x;

                    RGB color = target.buffer[cur];
                    target.buffer[cur] = color * factor;
                }
            });
        }
    }
}
