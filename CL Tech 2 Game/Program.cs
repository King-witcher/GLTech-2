﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GLTech2;
using Game.Properties;


namespace Game
{
    class Program
    {
        static void Main(string[] args)
        {

            var bg = new Material(new GLBitmap(Resources.Universe));
            Scene scene = new Scene(bg);

            Vector[] cylinder = Vector.GetPolygon(Vector.Origin, 6f, 9);
            var wallmaterial = new Material(new GLBitmap(Resources.Wall), 0, 2f);

            var cil = GetCylinder(new Vector(0, 0), 4f, wallmaterial);
            cil.AddBehaviour<Movement>();
            scene.AddElement(cil);

            Renderer.ParallelRendering = false;
            Renderer.CppRendering = false;
            Renderer.DisplayHeight = 900;
            Renderer.DisplayWidth = 1600;
            Renderer.MaxFps = 1000;

            Renderer.Run(scene);

            //Console.ReadKey();
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
