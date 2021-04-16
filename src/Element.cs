using System;
using System.Collections.Generic;

namespace GLTech2
{
    public abstract class Element : IDisposable
    {
        // Every element MUST call UpdateRelative() after construction. I have to fix it yet.
        private protected Element() { }

        private Element parent = null;
        private List<Behaviour> behaviours = new List<Behaviour>();
        private Vector relativePosition;
        private Vector relativeNormal;
        internal Scene scene;
        internal List<Element> childs = new List<Element>();

        public Scene Scene => scene; // Maybe not necessary.
        public int ChildCount => childs.Count;


        private protected abstract Vector AbsolutePosition { get; set; }
        private protected abstract Vector AbsoluteNormal { get; set; } //Provides rotation and scale of the object.


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

        public Element Parent
        {
            get => parent;
            set
            {
                if (value != null && scene != value.scene)
                {
                    if (Debug.DebugWarnings)
                    {
                        Console.WriteLine($"Cannot parent {this} to an element that is in other scene. Operation aborted.");
                        return;
                    }
                }

                if (parent != null)
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




        public void AttachChilds (IEnumerable<Element> elements)
        {
            foreach (Element el in elements)
                el.parent = this;
        }


        public void DetachChilds ()
        {
            foreach (Element child in childs)
            {
                child.Parent = null;
                childs.Remove(child);
            }
        }


        public void AddBehaviour<Behaviour>() where Behaviour : GLTech2.Behaviour, new()
        {
            if (ContainsBehaviour<Behaviour>() && Debug.DebugWarnings)
                Console.WriteLine($"Cannot add same behaviour twice. {typeof(Behaviour).Name} second instance will be ignored.");

            GLTech2.Behaviour behaviour = new Behaviour();
            behaviour.element = this;
            behaviours.Add(behaviour);
        }

        public bool ContainsBehaviour<Behaviour>() where Behaviour : GLTech2.Behaviour
        {
            foreach (var item in behaviours)
                if (item is Behaviour)
                    return true;
            return false;
        }

        public void RemoveBehaviour<Behaviour>() where Behaviour : GLTech2.Behaviour
        {
            foreach (var item in behaviours)
            {
                if (item is Behaviour)
                {
                    behaviours.Remove(item);
                    return;
                }
            }
        }

        public void RemoveAllBehaviours()
        {
            behaviours.Clear();
        }

        public void DetachChildren(Element element) // Not tested
        {
            foreach (Element child in childs)
                child.Parent = null;
        }

        public Element GetChild(int index)
        {
            return childs[index];
        }

        public void Translate(Vector direction)
        {
            Position += direction;
        }

        public void Rotate(float rotation)
        {
            Rotation += rotation;
        }

        // To be implemented by subclasses
        public virtual void Dispose() 
        {

        }

        internal void InvokeStart()
        {
            foreach (Behaviour behavior in behaviours)
            {
                behavior.Start();
            }
        }

        internal void InvokeUpdate()
        {
            foreach (Behaviour behavior in behaviours)
            {
                behavior.Update();
            }
        }
    }
}
