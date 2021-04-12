using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GLTech2
{
    internal interface IMovable
    {
        Vector Position { get; set; }
        float Rotation { get; set; }
    }
}
