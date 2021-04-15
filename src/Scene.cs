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

        private void AddElement(Element item)
        {
            if (item.scene is null) //Possible nullobject test
            {
                elements.Add(item);
                item.scene = this;
            }
            else
            {
                throw new InvalidOperationException($"\"{item}\" is already in a scene.");
            }
        }

        public void AddGroup(Group g)
        {
            elements.Add(g);
            g.scene = this;
        }

        internal void AddSprite(Sprite s) => throw new NotImplementedException();
        public void AddWall(Wall w)
        {
            if (unmanaged->wall_count >= unmanaged->wall_max)
                throw new IndexOutOfRangeException("Wall limit reached.");
            unmanaged->Add(w.walldata);
            elements.Add(w);
            w.scene = this;
        }

        public void AddWalls(params Wall[] walls)
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

        internal void InvokeStart()
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
