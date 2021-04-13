using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GLTech2
{
    public abstract class Behavior
    {
        public abstract void Update(double deltatime, double frametime);
        public abstract void Start();
    }
}
