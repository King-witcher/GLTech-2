using System;
using System.Collections.Generic;
using System.Reflection;

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
            get
            {
                if (parent is null)
                    return AbsolutePosition;
                else
                    return relativePosition;
            }
            set
            {
                if (parent is null)
                    AbsolutePosition = value;
                else
                {
                    relativePosition = value;
                    UpdateAbsolute();
                }
            }
        }

        // Gets and sets RELATIVE scale.
        public Vector Normal
        {
            get
            {
                if (parent is null)
                    return AbsoluteNormal;
                else
                    return relativeNormal;
            }
            set
            {
                if (parent is null)
                    AbsoluteNormal = value;
                else
                {
                    relativeNormal = value;
                    UpdateAbsolute();
                }
            }
        }

        // Gets and sets RELATIVE rotation through relative normal.
        public float Rotation
        {
            get
            {
                if (parent is null)
                    return AbsoluteNormal.Angle;
                else
                    return relativeNormal.Angle;
            }
            set
            {
                if (parent is null)
                {
                    Vector newNormal = AbsoluteNormal;
                    newNormal.Angle = value;
                    AbsoluteNormal = newNormal;
                }
                else
                {
                    relativeNormal.Angle = value;
                    UpdateAbsolute();
                }
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



        public void Translate(Vector direction)
        {
            Position += direction * Normal;
        }

        public void Rotate(float rotation)
        {
            Rotation += rotation;
        }

        public void DetachChilds()
        {
            foreach (Element child in childs)
            {
                child.Parent = null;
                childs.Remove(child);
            }
        }

        public void AddBehaviour(Behaviour b)
        {
            if (b is null)
                return;

            if (ContainsBehaviour(b))
            {
                Debug.LogWarning($"Cannot add same behaviour twice. {typeof(Behaviour).Name} second instance will be ignored.");
                return;
            }

            if (b.element is null is false)
            {
                Debug.LogWarning($"Cannot add same behaviour instance to two different elements. Element without behaviour: {this}.");
                return;
            }

            behaviours.Add(b); // Isso pode estar obsoleto, visto que agora os métodos a serem chamados são salvos em eventos.
            b.element = this;

            Subscribe(b);
        }

        public void AddBehaviour<BehaviourType>() where BehaviourType : Behaviour, new()
        {
            AddBehaviour(new BehaviourType());
        }

        public bool ContainsBehaviour<BehaviourType>() where BehaviourType : Behaviour
        {
            foreach (var behaviour in behaviours)
                if (behaviour is BehaviourType)
                    return true;
            return false;
        }

        public bool ContainsBehaviour(Behaviour b)
        {
            foreach (var item in behaviours)
                if (item == b)
                    return true;
            return false;
        }

        public void RemoveBehaviour(Behaviour b)
        {
            behaviours.Remove(b);
            Unsubscribe(b);
        }

        public void RemoveBehaviour<BehaviourType>() where BehaviourType : Behaviour
        {
           foreach (var behaviour in behaviours.ToArray()) // Provisional solution
                if (behaviour is BehaviourType)
                    RemoveBehaviour(behaviour);
        }

        public void RemoveAllBehaviours()
        {
            foreach(Behaviour b in behaviours)
            {
                RemoveBehaviour(b);
            }
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

        internal void InvokeStart()
        {
            StartEvent?.Invoke();
        }

        internal void InvokeUpdate()
        {
            UpdateEvent?.Invoke();
        }

        // To be implemented by subclasses
        public virtual void Dispose()
        {

        }

        //Subscribe and unsubscribe a behaviour
        private void Subscribe(Behaviour b)
        {
            StartEvent += b.StartMethod;
            UpdateEvent += b.UpdateMethod;
        }
        private void Unsubscribe(Behaviour b)
        {
            StartEvent -= b.StartMethod;
            UpdateEvent -= b.UpdateMethod;
        }

        event Action StartEvent;
        event Action UpdateEvent;
    }
}
