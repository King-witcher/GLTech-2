using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GLTech2;

namespace Game
{
    public class Movement : Behaviour
    {
        static Vector direction = new Vector(1f, -1f);
        protected sealed override void Update()
        {
            Element.Rotation += Time.DeltaTime * 0.05f;
        }
    }
}
