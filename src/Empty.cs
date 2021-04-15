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
        public Empty (Vector pos)
        {
            IsolatedPosition = pos;
        }

        private protected override Vector IsolatedPosition { get; set; }
        private protected override float IsolatedRotation { get; set; } = 0f;
    }
}
