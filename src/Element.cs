using System;
using System.Collections.Generic;

namespace GLTech2
{
    public abstract class Element : IDisposable
    {
        private protected Element() { }

        private Vector relativePosition;
        private Vector relativeNormal;

        // 
        private protected abstract Vector AbsolutePosition { get; set; }
        private protected abstract Vector AbsoluteNormal { get; set; } //Rotation and scale of the object.



        internal event Action OnChange;

        // Gets and sets RELATIVE position.
        public Vector Position
        {
            get => relativePosition;
            set
            {
                relativePosition = value;
                UpdateAbsolute();
            }
        }

        // Gets and sets RELATIVE scale.
        public Vector Normal
        {
            get => relativeNormal;
            set
            {
                relativeNormal = value;
                UpdateAbsolute();
            }
        }

        // Gets and sets RELATIVE rotation through relative normal.
        public float Rotation
        {
            get
            {
                return relativeNormal.Angle;
            }
            set
            {
                relativeNormal.Angle = value;
                UpdateAbsolute();
            }
        }

        private Element parent = null; //Axis provider.
        public Element Parent
        {
            get => parent;
            set
            {
                if (value != null && scene != value.scene)      // Suboptimal
                {
                    throw new InvalidOperationException("Cannot parent elements in different scenes.");
                }

                if (this.parent != null)
                {
                    parent.OnChange -= UpdateAbsolute;
                    parent.childs.Remove(this);
                }

                if (value != null)
                {
                    value.OnChange += UpdateAbsolute;
                    value.childs.Add(this);
                }
                this.parent = value;
                UpdateRelative();
            }
        }

        // Update relative transform through parent and absolute transform.
        // Called when attaches 
        // Must be called after construction of every subclass.
        private protected void UpdateRelative()
        {
            if (parent is null)
            {
                relativePosition = AbsolutePosition;
                relativeNormal = AbsoluteNormal;
            }
            else
            {
                relativePosition = AbsolutePosition.Projection(parent.AbsolutePosition, parent.AbsoluteNormal);
                relativeNormal = AbsoluteNormal / parent.AbsoluteNormal;
            }
        }

        // Update absolute position through relative position and parent.
        // Called either when the parent or this element changes its position.
        private protected void UpdateAbsolute()
        {
            if (parent is null)
            {
                AbsolutePosition = relativePosition;
                AbsoluteNormal = relativeNormal;
            }
            else
            {
                AbsolutePosition = relativePosition.AsProjectionOf(parent.AbsolutePosition, parent.AbsoluteNormal);
                AbsoluteNormal = relativeNormal * parent.AbsoluteNormal;
            }
            OnChange?.Invoke();
        }

        public int ChildCount => childs.Count;

        private List<Behaviour> behaviours = new List<Behaviour>(); // Rever desempenho disso
        internal List<Element> childs = new List<Element>(); // Rever desempenho disso
        internal Scene scene;

        public void AddBehaviour<T>() where T : Behaviour, new()
        {
            if (ContainsBehaviour<T>())
                throw new InvalidOperationException("Cannot add same behaviour twice."); // Questionable

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

        public Element GetChild(int index)
        {
            return childs[index];
        }

        private void Push(Vector direction)
        {
            throw new NotImplementedException();
        }

        private void Rotate(float roation)
        {
            throw new NotImplementedException();
        }

        public virtual void Dispose()
        {

        }

        internal void CallStart()
        {
            foreach (Behaviour behavior in behaviours)
            {
                behavior.Start();
            }
        }
        internal void CallUpdate()
        {
            foreach (Behaviour behavior in behaviours)
            {
                behavior.Update();
            }
        }
    }
}
