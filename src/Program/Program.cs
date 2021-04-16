using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GLTech2.Properties;
using GLTech2.StandardObjects;

namespace GLTech2
{
    static class Program
    {
        static void Main()
        {
            var bg = new Material(new GLBitmap(new System.Drawing.Bitmap(1, 1)));
            Scene scene = new Scene(bg);
            Element e = GetCylinder(Vector.Unit * 2, 1.5f, Resources.Wall);
            e.AddBehaviour<CountFPS>();
            scene.AddElement(e);

            Renderer.DisplayHeight = 900;
            Renderer.DisplayWidth = 1600;

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
