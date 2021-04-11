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

pixel SkyboxBackground_Legacy(Camera_& camera, int ray_id, int line)
{
    float angle_deg = camera.camera_angle + camera.cache_angles[ray_id];
    angle_deg = fmod(angle_deg, 360);
    float hratio = angle_deg / 360 + 1;

    float screenVratio = line / (float)camera.bitmap_height;
    float cos = camera.cache_cosines[ray_id];
    float vratio = (1 - cos) / 2 + cos * screenVratio;

    return camera.skybox->MapPixel(hratio, vratio);
}

pixel SkyboxBackground(Material_ skybox, float ray_angle, float ray_cos, int line, int display_height)
{
    float hratio = ray_angle / 360 + 180;

    float screenVratio = (float) line / display_height;
    float vratio = (1 - ray_cos) / 2 + ray_cos * screenVratio;

    return skybox.MapPixel(hratio, vratio);
}

void NativeRender(Camera_& camera)
{
    int display_height = camera.bitmap_height;
    int display_width = camera.bitmap_width;
    Material_ skybox = *camera.skybox;
    pixel* buffer = camera.bitmap_buffer;

    for (int ray_id = 0; ray_id < camera.bitmap_width; ray_id++)
    {
        float ray_angle = camera.cache_angles[ray_id] + camera.camera_angle;
        float ray_cos = camera.cache_cosines[ray_id];

        Ray ray = Ray(camera.camera_position, ray_angle);

        //Cast the ray towards every wall.
        float nearest_dist, nearest_ratio;
        Wall_* nearest = ray.NearestWall(camera.map, nearest_dist, nearest_ratio);
        if (nearest_dist != FLT_MAX)
        {
            float columnHeight = (camera.cache_colHeight1 / (ray_cos * nearest_dist));
            float fullColumnRatio = display_height / columnHeight;
            float topIndex = -(fullColumnRatio - 1.0f) / 2.0f;
            for (int line = 0; line < display_height; line++)
            {
                float vratio = topIndex + fullColumnRatio * line / display_height;
                if (vratio < 0.0f || vratio >= 1.0f)
                {
                    buffer[display_width * line + ray_id] = SkyboxBackground(skybox, ray_angle, ray_cos, line, display_height);
                }
                else
                {
                    int pixel = nearest->material.MapPixel(nearest_ratio, vratio);
                    buffer[display_width * line + ray_id] = pixel;
                }
            }
        }
        else
        {
            for (int line = 0; line < display_height; line++)
            {
                buffer[display_width * line + ray_id] = SkyboxBackground(skybox, ray_angle, ray_cos, line, display_height);
            }
        }
    }
}