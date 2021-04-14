using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GLTech2
{
    public abstract class Behaviour
    {
        internal Element element;

        protected internal Behaviour() { } // Makes possible the creation of derived classes
        protected internal Element Element { get => element; } // Needed to make possible 
        protected internal virtual void Update(double rendertime, double frametime) { }
        protected internal virtual void Start() { }
        protected internal virtual void Activate() { }
        protected internal virtual void BeginCollide(Element colisor) { }
        protected internal virtual void EndCollide(Element colisor) { }
    }
}
