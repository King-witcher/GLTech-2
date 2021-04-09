// dllmain.cpp : Define o ponto de entrada para o aplicativo DLL.
// Claramente não sou programador c++; não é difícil notar a diferença de qualidade de código
// Mas continua sendo útil ter isso daqui.
#include "PCH.h"
#include <iostream>

BOOL APIENTRY DllMain( HMODULE hModule,
                       DWORD  ul_reason_for_call,
                       LPVOID lpReserved
                     )
{
    switch (ul_reason_for_call)
    {
    case DLL_PROCESS_ATTACH:
    case DLL_THREAD_ATTACH:
    case DLL_THREAD_DETACH:
    case DLL_PROCESS_DETACH:
        break;
    }
    return TRUE;
}

pixel Background(int line, int height)
{
    if (line < height >> 1)
        return -14803426;
    else
        return -12171706;
}

void NativeRender(Camera_& camera)
{
    for (int ray_id = 0; ray_id < camera.bitmap_width; ray_id++)
    {
        Ray ray = Ray(camera.camera_position, camera.camera_angle + camera.cache_angles[ray_id]);

        //Cast the ray towards every wall.
        float nearest_dist, nearest_ratio;
        Wall_* nearest = ray.NearestWall(camera.map, nearest_dist, nearest_ratio);
        if (nearest_dist != FLT_MAX)
        {
            float columnHeight = (camera.cache_colHeight1 / (camera.cache_cosines[ray_id] * nearest_dist));
            float fullColumnRatio = camera.bitmap_height / columnHeight;
            float topIndex = -(fullColumnRatio - 1.0f) / 2.0f;
            for (int line = 0; line < camera.bitmap_height; line++)
            {
                float vratio = topIndex + fullColumnRatio * line / camera.bitmap_height;
                if (vratio < 0.0f || vratio >= 1.0f)
                {
                    camera.bitmap_buffer[camera.bitmap_width * line + ray_id] = Background(line, camera.bitmap_height);
                }
                else
                {
                    int pixel = nearest->material.MapPixel(nearest_ratio, vratio);
                    camera.bitmap_buffer[camera.bitmap_width * line + ray_id] = pixel;
                }
            }
        }
        else
        {
            for (int line = 0; line < camera.bitmap_height; line++)
                camera.bitmap_buffer[camera.bitmap_width * line + ray_id] = Background(line, camera.bitmap_height);
        }
}
}