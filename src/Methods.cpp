#include "PCH.h"

pixel Material_::MapPixel(float hratio, float vratio)
{
    int x = (int)(texture.width * (hrepeat * hratio + hoffset)) % texture.width;
    int y = (int)(texture.height * vratio) % texture.height;
    return texture.buffer[texture.width * y + x];
}

Ray::Ray(Vector start, float angle) : Start(start)
{
    Vector direction;
    direction.X = sin(TORAD * angle);
    direction.Y = cos(TORAD * angle);
    Direction = direction;
}

void Ray::GetCollisionData(Wall_* wall, float& distance, float& split)
{
    float det = this->Direction.X * wall->geom_direction.Y - Direction.Y * wall->geom_direction.X;
    if (det == 0)
    {
        split = distance = FLT_MAX;
        return;
    }

    float spldet = Direction.X * (Start.Y - wall->geom_start.Y) - Direction.Y * (Start.X - wall->geom_start.X);
    float dstdet = wall->geom_direction.X * (Start.Y - wall->geom_start.Y) - wall->geom_direction.Y * (Start.X - wall->geom_start.X);
    float spltmp = spldet / det;
    float dsttmp = dstdet / det;
    if (spltmp < 0 || spltmp >= 1 || dsttmp < 0)
    {
        split = distance = FLT_MAX;
        return;
    }
    split = spltmp;
    distance = dsttmp;
}

Wall_* Ray::NearestWall(Map_* map, float& nearest_dist, float& nearest_ratio)
{
    nearest_dist = FLT_MAX;
    nearest_ratio = FLT_MAX;
    int wallcount = map->wall_count;
    Wall_** cur = map->walls;
    Wall_* nearest = NULL;

    for (int i = 0; i < wallcount; i++)
    {
        if (cur == NULL) break;
        float cur_dist, cur_ratio;
        GetCollisionData(*cur, cur_dist, cur_ratio);
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