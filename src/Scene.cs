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
        private Camera current_camera;
        private List<Element> elements = new List<Element>();

        public Scene(Material background, int maxWalls = 512, int maxSprities = 512) =>
            unmanaged = SceneData.Alloc(maxWalls, maxSprities, background);


        public Camera CurrentCamera
        {
            get => current_camera;
            set
            {
                if (value.scene == this)
                {
                    current_camera = value;

                    //Provisional
                    OnChangeCamera?.Invoke();
                }
                else if (GlobalSettings.DebugWarnings)
                    Console.WriteLine("Can\'t set a camera that is not in this scene.");
            }
        }

        //Provisional
        internal event Action OnChangeCamera;

        public int MaxWalls => unmanaged->wall_max;
        public int WallCount => unmanaged->wall_count;


        public void AddElement(Element element)
        {
            if (element is null)
                throw new ArgumentNullException("Cannot add null elements.");

            if (element.scene != null && GlobalSettings.DebugWarnings)
            {
                Console.WriteLine($"\"{element}\" is already bound to scene {element.scene}. Adding operation will be aborted.");
                return;
            }

            if (element.Parent.scene != this)       // Must be tested
            {
                element.Parent = null;
            }


            if (element is Wall)
                AddWall(element as Wall);
            else if (element is Sprite)
                AddSprite(element as Sprite);

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

        private void AddWall(Wall w)
        {
            if (unmanaged->wall_count >= unmanaged->wall_max)
                throw new IndexOutOfRangeException("Wall limit reached.");
            unmanaged->Add(w.walldata);
        }
        private void AddSprite(Sprite s) => throw new NotImplementedException();

        public void SetCamera(Camera cam)
        {
            CurrentCamera = cam;
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
