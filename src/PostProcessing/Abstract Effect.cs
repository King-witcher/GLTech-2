using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GLTech2.PostProcessing
{
    public abstract class Effect
    {
        internal abstract void Process(PixelBuffer target);
    }
}
