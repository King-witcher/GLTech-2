//See Vector.cs before
//See Wall.cs before
    //See Vector.cs before
    //See Material.cs before
        //See Texture32.cs before

#define DEVELOPMENT

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

        public Vector Direction => direction;
        public Vector Start => start;

        //Optimizable
        internal void GetCollisionData(WallData* wall, out float distance, out float split)
        {
            float det = Direction.X * wall->geom_direction.Y - Direction.Y * wall->geom_direction.X;
            if (det == 0)
            {
                split = distance = float.PositiveInfinity;
                return;
            }

            float spldet = Direction.X * (Start.Y - wall->geom_start.Y) - Direction.Y * (Start.X - wall->geom_start.X);
            float dstdet = wall->geom_direction.X * (Start.Y - wall->geom_start.Y) - wall->geom_direction.Y * (Start.X - wall->geom_start.X);
            float spltmp = spldet / det;
            float dsttmp = dstdet / det;
            if (spltmp < 0 || spltmp >= 1 || dsttmp < 0)
            {
                split = distance = float.PositiveInfinity;
                return;
            }
            split = spltmp;
            distance = dsttmp;
        }

        //Optimizable
        internal WallData* NearestWall(SceneData* map, out float nearest_dist, out float nearest_ratio)
        {
            nearest_dist = float.PositiveInfinity;
            nearest_ratio = float.PositiveInfinity;
            int wallcount = map->wall_count;
            WallData** cur = map->walls;
            WallData* nearest = null;

            for (int i = 0; i < wallcount; i++)
            {
                if (cur == null) break;
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
