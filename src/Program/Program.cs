﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using GLTech2.Properties;
using GLTech2.PrefabElements;
using GLTech2.PostProcessing;

namespace GLTech2
{
    static class Program
    {
        static void Main()
        {
            Scene scene = new Scene(new Material((Texture) Resources.Universe, 0, 3));

            Material mat = new Material((Texture) Resources.metal, 0, 5);

            Element penta = new RegularPolygon(Vector.Origin, 5, 2f, mat);
            scene.AddElement(penta);

            Observer pov = new Observer(Vector.Origin, 0);
            scene.AddElement(pov);

            Element tri = new RegularPolygon(Vector.Origin * 3, 3, -2f, mat);
            //scene.AddElement(tri);

            pov.AddBehaviour<CountFPS>();

            Renderer.DisplayHeight = 900;
            Renderer.DisplayWidth = 1600;
            Renderer.FullScreen = false;
            Renderer.ParallelRendering = true;

            Renderer.AddPostProcessing(new FXAA(1600, 900));
            Renderer.AddPostProcessing(new Gamma());

            Renderer.Run(scene);
        }

        static Empty GetCylinder(Vector position, float radius, Material material)
        {
            Empty empty = new Empty(position);
            Vector[] verts = Vector.GetPolygon(position, radius, 64);
            Wall[] walls = Wall.CreatePolygon(material, verts);
            foreach (Wall wall in walls)
            {
                wall.Parent = empty;
            }
            return empty;
        }
    }
}
