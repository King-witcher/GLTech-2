using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GLTech2.PostProcessment
{
    public abstract class PostProcessing
    {
        internal unsafe abstract void Process(PixelBuffer target);
    }
}
