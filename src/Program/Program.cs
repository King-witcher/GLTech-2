using System;
using System.Drawing;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using GLTech2.Properties;
using GLTech2.PrefabElements;
using GLTech2.PostProcessing;
using GLTech2.StandardBehaviours;

namespace GLTech2
{
    static class Program
    {
        static void Main()
        {
            Console.WriteLine("Press any key to start.");
            Console.ReadKey();
            Console.Write("\b \b");

            Texture background = (Texture)Resources.Universe;
            Scene scene = new Scene(new Material(background, 0, 3));

            Texture metal = (Texture) Resources.metal;
            Material mat = new Material(metal, 0, 5);

            Element penta = new RegularPolygon(Vector.Origin, 5, 2f, mat);
            scene.AddElement(penta);

            Observer pov = new Observer(Vector.Origin, 0);
            scene.AddElement(pov);

            //Element tri = new RegularPolygon(Vector.Origin * 3, 3, -2f, mat);
            //scene.AddElement(tri);

            //pov.AddBehaviour<CountFPS>();
            pov.AddBehaviour<TestKeyboard>();
            var mouseLook = new MouseLook();
            mouseLook.Sensitivity = 2.31f;
            pov.AddBehaviour(mouseLook);

            Renderer.CustomHeight = 900;
            Renderer.CustomWidth = 1600;
            Renderer.FullScreen = false;
            Renderer.ParallelRendering = true;

            var fxaa = new FXAA(1600, 900);
            Renderer.AddPostProcessing(fxaa);

            Renderer.Run(scene);
            scene.Dispose();
            metal.Dispose();
            background.Dispose();
            Console.WriteLine("Press any key to close.");
            Console.ReadKey();
        }
    }
}
