using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GLTech2
{
    public abstract class PostProcessing
    {
        internal PostProcessing()
        {

        }

        internal abstract void Process(PixelBuffer buffer);
    }
}
