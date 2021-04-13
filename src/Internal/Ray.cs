//See Vector.cs before
//See Wall.cs before
//See Vector.cs before
//See Material.cs before
//See Texture32.cs before

#define DEVELOPMENT

using System;
using System.Threading;
using System.Runtime.InteropServices;

namespace GLTech2
{
    internal unsafe struct Ray
    {
        private readonly Vector direction;
        private readonly Vector start;

        public Ray(Vector start, Vector direction)
        {
            this.start = start;
            this.direction = direction;
        }

        public Ray(Vector start, float angle)
        {
            this.start = start;
            direction = new Vector(angle);
        }

        // 0 < distance <= infinity
        // 0 <= split < 1
        internal void GetCollisionData(WallData* wall, out float distance, out float split)
        {
            // Medium performance impact.
            float
                drx = wall->geom_direction.x,
                dry = wall->geom_direction.y;

            float det = direction.x * dry - direction.y * drx; // Caching can only be used here
            if (det == 0) // Parallel
            {
                distance = float.PositiveInfinity;
                split = 0f;
                return;
            }

            float spldet = direction.x * (start.y - wall->geom_start.y) - direction.y * (start.x - wall->geom_start.x);
            float dstdet = wall->geom_direction.x * (start.y - wall->geom_start.y) - wall->geom_direction.y * (start.x - wall->geom_start.x);
            float spltmp = spldet / det;
            float dsttmp = dstdet / det;
            if (spltmp < 0 || spltmp >= 1 || dsttmp <= 0) // dsttmp = 0 means column height = x/0.
            {
                split = 0;
                distance = float.PositiveInfinity;
                return;
            }
            split = spltmp;
            distance = dsttmp;
        }

        private WallData* NearestWallTest(SceneData* map, out float nearest_dist, out float nearest_hratio)
        {
            nearest_dist = float.PositiveInfinity;
            nearest_hratio = float.PositiveInfinity;
            WallData* nearest = null;
            WallData** pptr = map->walls;
            WallData* ptr;

            while(*pptr != null)
            {
                //Medium performance impact.
                GetCollisionData(*pptr, out float cur_dist, out float cur_ratio);
                if (cur_dist < nearest_dist)
                {
                    nearest_hratio = cur_ratio;
                    nearest_dist = cur_dist;
                    nearest = *pptr;
                }
                pptr++;
            }
            return nearest;
        }

        internal WallData* NearestWall(SceneData* map, out float nearest_dist, out float nearest_ratio)
        {

            nearest_dist = float.PositiveInfinity;
            nearest_ratio = float.PositiveInfinity;
            int wallcount = map->wall_count;
            WallData** cur = map->walls;
            WallData* nearest = null;

            for (int i = 0; i < wallcount; i++)
            {
                //walls cannot be null.
                GetCollisionData(*cur, out float cur_dist, out float cur_ratio);
                if (cur_dist < nearest_dist)
                {
                    nearest_ratio = cur_ratio;
                    nearest_dist = cur_dist;
                    nearest = *cur;
                }
                cur++;
            }
            return nearest;
        }
    }
}
