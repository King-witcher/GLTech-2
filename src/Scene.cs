//See Wall.cs before
    //See Vector.cs before
    //See Material.cs before
        //See Texture32.cs before
//See Sprite.cs before

using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace GLTech2
{
    [StructLayout(LayoutKind.Sequential)]
    internal unsafe struct SceneData : IDisposable
    {
        internal Sprite_** sprities;
        internal int sprite_count;
        internal int sprite_max;
        internal WallData** walls;
        internal int wall_count;
        internal int wall_max;
        internal Material background;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static SceneData* Alloc(int maxWalls, int masSprities, Material background)
        {
            SceneData* result = (SceneData*)Marshal.AllocHGlobal(sizeof(SceneData));
            result->sprities = (Sprite_**)Marshal.AllocHGlobal(masSprities * sizeof(Sprite_*)); // Not implemented yet
            result->walls = (WallData**)Marshal.AllocHGlobal(maxWalls * sizeof(WallData*));
            result->sprite_count = 0;
            result->sprite_max = masSprities;
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
        internal void Add(WallData* wall) => walls[wall_count++] = wall;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal void Add(void* sprite) => throw new NotImplementedException();
    }

    public unsafe sealed class Scene : IDisposable
    {
        internal SceneData* unmanaged;
        //internal Material refBackground;

        public Scene(Material background, int maxWalls = 512, int maxSprities = 512)
        {
            //refBackground = background;
            unmanaged = SceneData.Alloc(maxWalls, maxSprities, background);
        }

        public int MaxWalls => unmanaged->wall_max;
        public int WallCount => unmanaged->wall_count;
        public Vector StartingPoint => Vector.Origin; //Precisa ser removido

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal void AddSprite(Sprite s) => throw new NotImplementedException();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void AddWall(Wall w)
        {
            if (unmanaged->wall_count >= unmanaged->wall_max)
                throw new IndexOutOfRangeException("Wall limit reached.");
            unmanaged->Add(w.unmanaged);
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

        public void Dispose()
        {
            unmanaged->Dispose();
            Marshal.FreeHGlobal((IntPtr)unmanaged);
        }
    }
}
