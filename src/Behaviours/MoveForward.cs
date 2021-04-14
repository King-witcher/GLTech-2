using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GLTech2.Behaviours
{
    // Test
    public class MoveForward : Behaviour
    {
        static Vector direction = new Vector(0.707f, 0.707f);
        protected internal override void Update()
        {
            Element.Position += Time.DeltaTime * direction;
        }
    }
}
