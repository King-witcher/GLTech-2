using System;
using System.Collections.Generic;

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
        public float Rotation           // SUBOPTIMAL AND NOT TESTED.
        {
            get => IsolatedRotation;
            set
            {
                float deltar = value - IsolatedRotation;

                IsolatedRotation = value;
                foreach (Element child in childs)       //Ferra tudo se a rotação for lenta
                {
                    Vector childpos = child.Position;
                    Vector distance = childpos - this.Position;
                    distance *= new Vector(deltar);
                    childpos += distance;
                    child.Position = childpos;
                    child.Rotation += deltar;
                }
            }
        }

        // Position and rotation relative to parent.
        private Vector relative_position;
        private float relative_rotation;
        private Element parent = null;
        public Element Parent
        {
            get => parent;
            set
            {
                if (this.parent != null)
                {
                    parent.OnMove -= UpdateRealOnes;
                }

                if (value != null)
                {
                    value.OnMove += UpdateRealOnes;
                }

                this.parent = value;
            }
        }

        private void UpdateRelativeOnes()   // A bit suboptimal
        {
            relative_rotation = IsolatedRotation - parent.IsolatedRotation;


        }

        private void UpdateRealOnes()   // Not optimized, a bit redundant.
        {

        }

        internal event Action OnMove;

        public int ChildCount => childs.Count;

        private List<Behaviour> behaviours = new List<Behaviour>(); // Rever desempenho disso
        private List<Element> childs = new List<Element>(); // Rever desempenho disso
        internal Scene scene;

        public void AddBehaviour<T>() where T : Behaviour, new()
        {
            if (ContainsBehaviour<T>())
                throw new InvalidOperationException("Cannot add same behaviour twice.");

            Behaviour behaviour = new T();
            behaviour.element = this;
            behaviours.Add(behaviour);
        }

        public bool ContainsBehaviour<T>() where T : Behaviour
        {
            foreach (var item in behaviours)
                if (item is T)
                    return true;
            return false;
        }

        public void RemoveBehaviour<T>() where T : Behaviour
        {
            foreach (var item in behaviours)
            {
                if (item is T)
                {
                    behaviours.Remove(item);
                    return;
                }
            }
        }

        public void AddChild(Element child)
        {
            if (child.parent != null)
                child.parent.RemoveChild(child);

            this.childs.Add(child);
            child.parent = this;
        }
        public void AddChilds(params Element[] childs)
        {
            foreach (var item in childs)
            {
                AddChild(item);
            }
        }

        public Element GetChild(int index)
        {
            return childs[index];
        }

        public void RemoveChild(Element child)
        {
            childs.Remove(child);
            child.parent = null;
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
            foreach (Behaviour behavior in behaviours)
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
            foreach (Behaviour behavior in behaviours)
            {
                behavior.Update();
            }
        }
    }
}
