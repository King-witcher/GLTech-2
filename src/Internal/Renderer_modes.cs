using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GLTech2
{
    partial class Renderer
    {
        //Don't know how to pinvoke fromm current directory =/
        [DllImport(@"D:\GitHub\GLTech-2\bin\Release\glt2_nat.dll", CallingConvention = CallingConvention.Cdecl)]
        private unsafe static extern void NativeRender(RenderStruct* camera);

        private unsafe static void CLRRender()
        {
            //Caching frequently used values.
            int display_width = rendererData->bitmap_width;
            int display_height = rendererData->bitmap_height;
            Material background = rendererData->scene->background;

            if (ParallelRendering)
                Parallel.For(0, display_width, Loop);
            else
                for (int i = 0; i < display_width; i++)
                    Loop(i);

            void Loop(int ray_id)
            //for (int ray_id = 0; ray_id < rendererData->bitmap_width; ray_id++)
            {
                //Caching
                float ray_cos = rendererData->cache_cosines[ray_id];
                float ray_angle = rendererData->cache_angles[ray_id] + rendererData->camera_angle;
                Ray ray = new Ray(rendererData->camera_position, ray_angle);

                //Cast the ray towards every wall.
                WallData* nearest = ray.NearestWall(rendererData->scene, out float nearest_dist, out float nearest_ratio);
                if (nearest_dist != float.PositiveInfinity)
                {
                    float columnHeight = (rendererData->cache_colHeight1 / (ray_cos * nearest_dist)); //Wall column size in pixels
                    float fullColumnRatio = display_height / columnHeight;
                    float topIndex = -(fullColumnRatio - 1f) / 2f;
                    for (int line = 0; line < display_height; line++)
                    {
                        //Critical performance impact.
                        float vratio = topIndex + fullColumnRatio * line / display_height;
                        if (vratio < 0f || vratio >= 1f)
                        {
                            //PURPOSELY REPEATED CODE!
                            float background_hratio = ray_angle / 360 + 1; //Temporary bugfix to avoid hratio being < 0
                            float screenVratio = (float)line / display_height;
                            float background_vratio = (1 - ray_cos) / 2 + ray_cos * screenVratio;
                            int color = background.MapPixel(background_hratio, background_vratio);
                            rendererData->bitmap_buffer[display_width * line + ray_id] = color;
                        }
                        else
                        {
                            int pixel = nearest->material.MapPixel(nearest_ratio, vratio);
                            rendererData->bitmap_buffer[display_width * line + ray_id] = pixel;
                        }
                    }
                }
                else
                {
                    for (int line = 0; line < display_height; line++)
                    {
                        //Critical performance impact.
                        //PURPOSELY REPEATED CODE!
                        float background_hratio = ray_angle / 360 + 1;
                        float screenVratio = (float)line / display_height;
                        float background_vratio = (1 - ray_cos) / 2 + ray_cos * screenVratio;
                        int color = background.MapPixel(background_hratio, background_vratio);
                        rendererData->bitmap_buffer[display_width * line + ray_id] = color;
                    }
                }
            }
        }
    }
}
