using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GLTech2
{
    // Test
    internal class NullElement : Element
    {
        private protected override Vector IsolatedPosition { get => Vector.Origin; set { } }
        private protected override float IsolatedRotation { get => 0f; set { } }
    }
}
