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
    [StructLayout(LayoutKind.Sequential)]
    internal unsafe struct SceneData : IDisposable
    {
        internal SpriteData** sprities; //obsolete
        internal int sprite_count;
        internal int sprite_max;
        internal WallData** walls;
        internal int wall_count;
        internal int wall_max;
        internal Material background;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static SceneData* Alloc(int maxWalls, int maxSprities, Material background)
        {
            int total_positions = maxWalls + maxSprities + 1;
            SceneData* result = (SceneData*)Marshal.AllocHGlobal(sizeof(SceneData));
            result->sprities = null;
            //result->sprities = (Sprite_**)Marshal.AllocHGlobal(maxSprities * sizeof(Sprite_*)); // Not implemented yet
            result->walls = (WallData**)Marshal.AllocHGlobal(total_positions * sizeof(void*));
            *result->walls = null;
            result->sprite_count = 0;
            result->sprite_max = maxSprities;
            result->wall_count = 0;
            result->wall_max = maxWalls;
            result->background = background;
            return result;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Dispose()
        {
            Marshal.FreeHGlobal((IntPtr)sprities);
            Marshal.FreeHGlobal((IntPtr)walls);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal void Add(WallData* wall)
        {
            walls[wall_count++] = wall;
            walls[wall_count] = null;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal void Add(void* sprite) => throw new NotImplementedException();
    }

    public unsafe sealed partial class Scene : IDisposable
    {
        internal SceneData* unmanaged;

        public Scene(Material background, int maxWalls = 512, int maxSprities = 512) =>
            unmanaged = SceneData.Alloc(maxWalls, maxSprities, background);


        public int MaxWalls => unmanaged->wall_max;
        public int WallCount => unmanaged->wall_count;




        private List<Element> elements = new List<Element>();

        private void AddElement(Element element)    // Must be refactored
        {
#pragma warning disable CS0612
            if (element is Wall)
            {
                AddWall(element as Wall);
                return;
            }
            else if (element is Empty)
            {
                AddEmpty(element as Empty);
                return;
            }

#pragma warning restore CS0612
            if (element.scene is null)
            {
                elements.Add(element);
                element.scene = this;

                foreach (var item in element.childs)
                    AddElement(item);                   // May cause infinite recursion.
            }
            else
            {
                throw new InvalidOperationException($"\"{element}\" is already in a scene.");
            }
        }

        public void AddEmpty(Empty empty)   // Must be refactored
        {
            elements.Add(empty);
            empty.scene = this;

            foreach (var item in empty.childs)
                AddElement(item);
        }

        internal void AddSprite(Sprite s) => throw new NotImplementedException();
        [Obsolete]
        public void AddWall(Wall w)     // Must be refactored
        {
            if (unmanaged->wall_count >= unmanaged->wall_max)
                throw new IndexOutOfRangeException("Wall limit reached.");
            unmanaged->Add(w.walldata);
            elements.Add(w);
            w.scene = this;
        }
        [Obsolete]
        public void AddWalls(params Wall[] walls)   // Must be refactored
        {
            foreach (Wall wall in walls)
            {
                if (wall == null)
                    break;
                AddWall(wall);
            }
        }


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
                element.Update();
            }
        }
    }
}
