using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GLTech2
{
    public sealed class Empty : Element
    {
        public Empty(Vector pos)
        {
            AbsolutePosition = pos;
            UpdateRelative();
        }
        public Empty(float x, float y) : this(new Vector(x, y))
        {
        }

        private protected override Vector AbsolutePosition { get; set; }
        private protected override Vector AbsoluteNormal { get; set; } = new Vector(0, 1);
    }
}
