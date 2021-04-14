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
        private protected Element() { }


        private protected abstract Vector IsolatedPosition { get; set; }
        private protected abstract float IsolatedRotation { get; set; }
        public Vector Position
        {
            get => IsolatedPosition;
            set
            {
                Vector delta = value - IsolatedPosition;
                IsolatedPosition = value;
                foreach (Element child in childs)
                    child.Position += delta;
            }
        }
        public float Rotation
        {
            get => IsolatedRotation;
            set
            {
                float delta = value - IsolatedRotation;
                IsolatedRotation = value;
                foreach (Element child in childs)
                    child.Rotation += delta;
            }
        }


        private List<Behaviour> behaviors = new List<Behaviour>();
        private List<Element> childs = new List<Element>();


        public void AddBehavior<T>() where T : Behaviour, new()
        {
            Behaviour behaviour = new T();
            behaviour.element = this;
            behaviors.Add(behaviour);
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
            foreach (Behaviour behavior in behaviors)
            {
                if (behavior is null)
                {
                    throw new CannotUnloadAppDomainException();
                }
                behavior.Start();
            }
        }
        internal void Update()
        {
            foreach (Behaviour behavior in behaviors)
            {
                behavior.Update();
            }
        }
    }
}
