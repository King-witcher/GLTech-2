using System;
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
            Vector[] cylinder = Vector.GetPolygon(Vector.Origin, 6f, 200);
            var wallmaterial = new Material(new Texture32(Resources.Wall));
            var bg = new Material(new Texture32(Resources.Universe));

            Wall[] walls = Wall.CreatePolygon(wallmaterial, cylinder);
            Scene scene = new Scene(bg);
            scene.AddWalls(walls);
            Renderer.ParallelRendering = true;
            Renderer.DisplayHeight = 900;
            Renderer.DisplayWidth = 1600;

            Renderer.Run(scene, null, null);
        }
    }
}
