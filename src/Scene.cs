//See Wall.cs before
//See Vector.cs before
//See Material.cs before
//See Texture32.cs before
//See Sprite.cs before

using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace GLTech2
{
    public unsafe sealed partial class Scene : IDisposable
    {
        internal SceneData* unmanaged;
        private Observer activeObserver = new Observer(Vector.Unit, 0);    //Provisional
        private List<Element> elements = new List<Element>();

        public Scene(Material background, int maxWalls = 512, int maxSprities = 512) =>
            unmanaged = SceneData.Alloc(maxWalls, maxSprities, background);


        public Observer ActiveObserver
        {
            get => activeObserver;
            set
            {
                if (value is null || value.scene == null)   // null pointer
                {
                    activeObserver = value;
                    unmanaged->activeObserver = value.unmanaged;
                }
                else if (Debug.DebugWarnings)
                    Console.WriteLine("Can\'t set a camera that is not in this scene.");
            }
        }


        public int MaxWalls => unmanaged->wall_max;
        public int WallCount => unmanaged->wall_count;


        public void AddElement(Element element)
        {
            if (element is null)
                throw new ArgumentNullException("Cannot add null elements.");

            if (element.scene != null && Debug.DebugWarnings)
            {
                Console.WriteLine($"\"{element}\" is already bound to scene {element.scene}. Adding operation will be aborted.");
                return;
            }

            if (element.Parent != null && element.Parent.scene != this)       // Must be tested
            {
                element.Parent = null;
            }


            if (element is Wall)
                UnmanagedAddWall(element as Wall);
            else if (element is Sprite)
                UnmanagedAddSprite(element as Sprite);
            else if (element is Observer)
                UnmanagedAddObserver(element as Observer);

            elements.Add(element);
            element.scene = this;

            foreach (var item in element.childs)    // possible stack overflow and I dont care if the user tries to force it xD
                AddElement(item);
        }

        public void AddElements(IEnumerable<Element> elements)
        {
            foreach (Element item in elements)
            {
                if (item is null)
                    break;

                AddElement(item);
            }
        }

        private void UnmanagedAddWall(Wall w)
        {
            if (unmanaged->wall_count >= unmanaged->wall_max)
                throw new IndexOutOfRangeException("Wall limit reached.");
            unmanaged->Add(w.walldata);
        }
        private void UnmanagedAddSprite(Sprite s) => throw new NotImplementedException();

        private void UnmanagedAddObserver(Observer p)
        {
            ActiveObserver = p;
        }

        public void Dispose()   // must change
        {
            unmanaged->Dispose();
            Marshal.FreeHGlobal((IntPtr)unmanaged);
            elements.Clear();
        }

        internal void InvokeStart()
        {
            foreach (var element in elements)
                element.InvokeStart();
        }

        internal void InvokeUpdate()
        {
            foreach (var element in elements)
                element.InvokeUpdate();
        }
    }
}
