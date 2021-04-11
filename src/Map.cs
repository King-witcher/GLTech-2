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
    internal unsafe struct Map_ : IDisposable
    {
        internal Sprite_** sprities;
        internal int sprite_count;
        internal int sprite_max;
        internal Wall_** walls;
        internal int wall_count;
        internal int wall_max;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static Map_* Alloc(int maxWalls, int masSprities)
        {
            Map_* result = (Map_*)Marshal.AllocHGlobal(sizeof(Map_));
            result->sprities = (Sprite_**)Marshal.AllocHGlobal(masSprities * sizeof(Sprite_*)); // Not implemented yet
            result->walls = (Wall_**)Marshal.AllocHGlobal(maxWalls * sizeof(Wall_*));
            result->sprite_count = 0;
            result->sprite_max = masSprities;
            result->wall_count = 0;
            result->wall_max = maxWalls;
            return result;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Dispose()
        {
            Marshal.FreeHGlobal((IntPtr)sprities);
            Marshal.FreeHGlobal((IntPtr)walls);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal void Add(Wall_* wall) => walls[wall_count++] = wall;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal void Add(void* sprite) => throw new NotImplementedException();
    }

    public unsafe sealed class Map : IDisposable
    {
        internal Map_* unmanaged;

        public Map(int maxWalls = 512, int maxSprities = 512) => unmanaged = Map_.Alloc(maxWalls, maxSprities);

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
