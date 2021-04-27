﻿using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace GLTech2
{
    partial class Renderer
    {
        //Don't know how to pinvoke fromm current directory =/
        //[DllImport(@"D:\GitHub\GLTech-2\bin\Release\glt2_nat.dll", CallingConvention = CallingConvention.Cdecl)]
        //private unsafe static extern void NativeRender(RendererData* camera);

        private unsafe static void CLRRender(PixelBuffer target, SceneData* scene)        // Must be changed
        {
            //Caching frequently used values.
            uint* buffer = target.buffer;
            int width = target.width;
            int height = target.height;
            Material background = scene->background;

            if (ParallelRendering)
            {
                Parallel.For(0, width, Loop);
            }
            else
                for (int i = 0; i < width; i++)
                    Loop(i);

            void Loop(int ray_id)
            //for (int ray_id = 0; ray_id < rendererData->bitmap_width; ray_id++)
            {
                //Caching
                float ray_cos = rendererData->cache_cosines[ray_id];
                float ray_angle = rendererData->cache_angles[ray_id] + scene->activeObserver->rotation;
                Ray ray = new Ray(scene->activeObserver->position, ray_angle);

                //Cast the ray towards every wall.
                WallData* nearest = ray.NearestWall(scene, out float nearest_dist, out float nearest_ratio);
                if (nearest_ratio != 2f)
                {
                    float columnHeight = (rendererData->cache_colHeight1 / (ray_cos * nearest_dist)); //Wall column size in pixels
                    float fullColumnRatio = height / columnHeight;
                    float topIndex = -(fullColumnRatio - 1f) / 2f;
                    for (int line = 0; line < height; line++)
                    {
                        //Critical performance impact.
                        float vratio = topIndex + fullColumnRatio * line / height;
                        if (vratio < 0f || vratio >= 1f)
                        {
                            //PURPOSELY REPEATED CODE!
                            float background_hratio = ray_angle / 360 + 1; //Temporary bugfix to avoid hratio being < 0
                            float screenVratio = (float)line / height;
                            float background_vratio = (1 - ray_cos) / 2 + ray_cos * screenVratio;
                            uint color = background.MapPixel(background_hratio, background_vratio);
                            buffer[width * line + ray_id] = color;
                        }
                        else
                        {
                            uint pixel = nearest->material.MapPixel(nearest_ratio, vratio);
                            buffer[width * line + ray_id] = pixel;
                        }
                    }
                }
                else
                {
                    for (int line = 0; line < height; line++)
                    {
                        //Critical performance impact.
                        //PURPOSELY REPEATED CODE!
                        float background_hratio = ray_angle / 360 + 1;
                        float screenVratio = (float)line / height;
                        float background_vratio = (1 - ray_cos) / 2 + ray_cos * screenVratio;
                        uint color = background.MapPixel(background_hratio, background_vratio);
                        buffer[width * line + ray_id] = color;
                    }
                }
            }
        }

        private unsafe static void FXAA(PixelBuffer target, int threshold)
        {
            PixelBuffer temp = new PixelBuffer(target.width, target.height);
            temp.Clone(target);

            Parallel.For(1, target.height, (i) =>
            {
                for (int j = 1; j < target.width; j++)
                {
                    int cur = target.width * i + j;
                    int up = target.width * (i - 1) + j;
                    int left = target.width * i + j - 1;

                    int differenceV = difference(
                        target.buffer[cur],
                        target.buffer[up]);

                    int differenceH = difference(
                        target.buffer[cur],
                        target.buffer[left]);

                    if (differenceV > 12)
                        temp.buffer[target.width * i + j] = mix(target.buffer[up], target.buffer[cur]);
                   else if (differenceH > 12)
                        temp.buffer[target.width * i + j] = mix(target.buffer[left], target.buffer[cur]);

                }
            });

            target.Clone(temp);
            temp.Dispose();

            int difference(uint pixel1, uint pixel2)
            {
                int B = (int)pixel1 % 256 - (int)pixel2 % 256;
                pixel1 >>= 8;
                pixel2 >>= 8;
                int G = (int)pixel1 % 256 - (int)pixel2 % 256;
                pixel1 >>= 8;
                pixel2 >>= 8;
                int R = (int)pixel1 % 256 - (int)pixel2 % 256;
                return (int) (Math.Sqrt(B * B + G * G + R * R));
            }

            uint mix(uint pixel1, uint pixel2)
            {
                uint res = 0;
                for (int i = 0; i < 4; i++)
                {
                    res += (pixel1 % 256 + pixel2 % 256) / 2 << (8 * i);
                    pixel1 >>= 8;
                    pixel2 >>= 8;
                }
                return res;
            }
        }

        private unsafe static void PostProcess(PixelBuffer target)
        {
            Random rnd = new Random();
            Parallel.For(0, target.height, (i) =>
            {
                for (int j = 0; j < target.width; j++)
                {
                    uint* pixel = target.buffer + (j + target.width * i);
                    *pixel = darker(*pixel);
                }
            });

            uint darker(uint color)
            {
                uint result = 0u;
                uint newColor;
                uint currentColor;

                for (int i = 0; i < 4; i++)
                {
                    currentColor = (color % 256u);

                    newColor = currentColor / 2;

                    result += newColor << 8 * i;
                    color >>= 8;
                }

                return result;
            }
        }
    }
}
