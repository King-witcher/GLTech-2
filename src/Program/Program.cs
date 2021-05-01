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

            Texture background = (Texture)Resources.Black;
            Scene scene = new Scene(new Material(background, 0, 3));

            Texture metal = (Texture) Resources.Test;
            Material mat = new Material(metal, 0, 5);

            Element penta = new RegularPolygon(Vector.Origin, 50, 2f, mat);
            scene.AddElement(penta);
            penta.AddBehaviour<Rotate>(); // Why isnt it working?

            Observer pov = new Observer(Vector.Origin, 0);
            scene.AddElement(pov);

            //Element tri = new RegularPolygon(Vector.Origin * 3, 3, -2f, mat);
            //scene.AddElement(tri);

            pov.AddBehaviour<DebugFps>();
            pov.AddBehaviour<NoclipMovement>();
            var mouseLook = new MouseLook();
            mouseLook.Sensitivity = 2.31f;
            pov.AddBehaviour(mouseLook);

            Renderer.CustomHeight = 900;
            Renderer.CustomWidth = 1600;
            Renderer.FullScreen = true;
            Renderer.ParallelRendering = true;

            var AA = new GLTXAA(Renderer.CustomWidth, Renderer.CustomHeight, 200);
            AA.EdgeDettection = false;
            Renderer.AddPostProcessing(AA);

            Renderer.Run(scene);
            scene.Dispose();
            metal.Dispose();
            background.Dispose();
            Console.WriteLine("Press any key to close.");
            Console.ReadKey();
        }
    }
}
