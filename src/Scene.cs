//See Wall.cs before
    //See Vector.cs before
    //See Material.cs before
        //See Texture32.cs before
//See Sprite.cs before

using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace GLTech2
{

    public unsafe sealed partial class Scene : IDisposable
    {
        internal SceneData* unmanaged;
        private List<Element> elements = new List<Element>();

        public Scene(Material background, int maxWalls = 512, int maxSprities = 512) =>
            unmanaged = SceneData.Alloc(maxWalls, maxSprities, background);

        public int MaxWalls => unmanaged->wall_max;
        public int WallCount => unmanaged->wall_count;



        public void AddElement(Element element)    // Must be refactored
        {
            if (element is null)
                throw new ArgumentNullException("Cannot add null elements.");

            if (element.scene != null && GlobalSettings.DebugWarnings)
            {
                Console.WriteLine($"\"{element}\" is already bound to scene {element.scene}. Adding operation will be aborted.");
                return;
            }


            if (element is Wall)
                AddWall(element as Wall);
            else if (element is Sprite)
                AddSprite(element as Sprite);

            elements.Add(element);
            element.scene = this;

            foreach (var item in element.childs)    // possible stack overflow and I dont matter.
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

        public void Dispose()   //must change
        {
            unmanaged->Dispose();
            Marshal.FreeHGlobal((IntPtr)unmanaged);
            elements.Clear();
        }

        internal void InvokeStart() // not implemented yet
        {
            //foreach (var element in Elements)
                //element.Start();
        }

        internal void InvokeUpdate()
        {
            foreach (var element in elements)
            {
                element.CallUpdate();
            }
        }
    }
}
