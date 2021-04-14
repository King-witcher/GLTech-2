using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GLTech2;
using GLTech2.Behaviours;
using Game.Properties;


namespace Game
{
    class Program
    {
        static void Main(string[] args)
        {
            var bg = new Material(new Texture32(Resources.Universe));
            Scene scene = new Scene(bg);

            Vector[] cylinder = Vector.GetPolygon(Vector.Origin, 6f, 200);
            var wallmaterial = new Material(new Texture32(Resources.Wall), 0, 2f);

            Wall[] walls = Wall.CreatePolygon(wallmaterial, cylinder);
            scene.AddWalls(walls);

            Wall bigwall = new Wall(new Vector(1, 2), new Vector(4, 1), wallmaterial);
            bigwall.AddBehavior<MoveForward>();
            scene.AddWall(bigwall);

            Renderer.ParallelRendering = true;
            Renderer.DisplayHeight = 900;
            Renderer.DisplayWidth = 1600;
            Renderer.MaxFps = 1000;

            Renderer.Run(scene, null, null);

            Console.ReadKey();
        }
    }
}
