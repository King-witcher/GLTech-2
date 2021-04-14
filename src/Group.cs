using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GLTech2
{
    internal sealed class Group : Element
    {
        private protected override Vector IsolatedPosition { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        private protected override float IsolatedRotation { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    }
}
