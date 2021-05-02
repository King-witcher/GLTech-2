using GLTech2.PostProcessing;
using GLTech2.PrefabElements;
using GLTech2.Properties;
using GLTech2.StandardBehaviours;
using System;

namespace GLTech2
{
    static class Program
    {
        static void Main()
        {
            Example();
        }

        static void Example()
        {
            Console.WriteLine("Press any key to start.");
            Console.ReadKey();
            Console.Write("\b \b");

            // Load textures frmo bitmaps
            Texture universe = (Texture)Resources.Universe;
            Texture colortest = (Texture)Resources.ColorTest;
            Texture metal = (Texture)Resources.Metal;
            Texture brick = (Texture)Resources.Wall;

            // Create materials
            Material universe_mat = new Material(
                texture: universe,
                hoffset: 0f,
                hrepeat: 3f);
            Material colortestCylinder = new Material(
                texture: colortest,
                hoffset: 0f,
                hrepeat: 6f);
            Material metal_mat = new Material(
                texture: metal,
                hoffset: 0f,
                hrepeat: 2f);
            Material brick_mat = new Material(
                texture: brick,
                hoffset: 0f,
                hrepeat: 2f);

            // Create scene
            Scene scene = new Scene(universe_mat);

            // Create scene elements
            Element cyl = new RegularPolygon(new Vector(-0.5f, 0f), 4, 0.5f, colortestCylinder);
            Element cyl2 = new RegularPolygon(new Vector(0.5f, 0f), 100, 0.5f, metal_mat);
            Element cyl3 = new RegularPolygon(new Vector(0f, 0.866f), 3, 0.5f, brick_mat);
            Empty center = new Empty(0, 0.2868f);
            cyl.Parent = center;
            cyl2.Parent = center;
            cyl3.Parent = center;
            Observer pov = new Observer(Vector.Backward, 0);

            scene.AddElement(center);
            scene.AddElement(pov);

            // Add behaviours
            cyl.AddBehaviour(new Rotate { Speed = 18f });
            cyl2.AddBehaviour(new Rotate { Speed = 18f });
            cyl3.AddBehaviour(new Rotate { Speed = 18f });
            center.AddBehaviour(new Rotate { Speed = -6f });
            pov.AddBehaviour<DebugFps>();
            pov.AddBehaviour<NoclipMovement>();

            var mouseLook = new MouseLook();
            mouseLook.Sensitivity = 2.31f;
            pov.AddBehaviour(mouseLook);


            // Setup Renderer
            Renderer.FullScreen = true;
            Renderer.FieldOfView = 85;

            // Add post processing effects
            var antiAliasing = new FXAA(Renderer.CustomWidth, Renderer.CustomHeight, 12);
            //Renderer.AddPostProcessing(antiAliasing);
            //AA.EdgeDettection = true;

            // Run!
            Renderer.Run(scene);

            // Release everything
            scene.Dispose();
            metal.Dispose();
            colortest.Dispose();
            Console.WriteLine("Press any key to close.");
            Console.ReadKey();
        }
    }
}
