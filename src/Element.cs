using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GLTech2
{
    public abstract class Element : IDisposable
    {
        public abstract Vector Position { get; set; }
        public abstract float Rotation { get; set; }


        private List<Behavior> behaviors = new List<Behavior>();
        private List<Element> childs = new List<Element>();


        public Element()
        {
        }

        public void AddBehavior(Behavior behavior)
        {
            behaviors.Add(behavior);
        }


        public void Push(Vector direction)
        {
            throw new NotImplementedException();
        }


        public void Rotate(float roation)
        {
            throw new NotImplementedException();
        }


        public virtual void Dispose()
        {

        }


        internal void Start()
        {
            foreach(Behavior behavior in behaviors)
                behavior.Start();
        }

        internal void Update(double deltatime, double frametime)
        {
            foreach (Behavior behavior in behaviors)
                behavior.Update(deltatime, frametime);
        }
    }
}
