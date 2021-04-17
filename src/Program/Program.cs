using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using GLTech2.Properties;
using GLTech2.StandardObjects;

namespace GLTech2
{
    static class Program
    {
        static void Main()
        {
            Scene scene = new Scene(new Material((GLBitmap) Resources.Universe, 0, 3));

            Material mat = Resources.Wall;

            Element cylinder = GetCylinder(Vector.Unit * 2, 0.5f, mat);
            scene.AddElement(cylinder);

            Observer pov = new Observer(Vector.Origin, 0);
            scene.AddElement(pov);

            //Element tri = new RegularPolygon(Vector.Unit * 3, 3, 2, mat);
            //scene.AddElement(tri);

            pov.AddBehaviour<CountFPS>();

            Renderer.DisplayHeight = 900;
            Renderer.DisplayWidth = 1600;
            Renderer.FullScreen = true;

            Renderer.CppRendering = false;
            Renderer.ParallelRendering = false;
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
