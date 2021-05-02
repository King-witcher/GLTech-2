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
            Texture skybox = (Texture)Resources.DoomSky;
            Texture carvedWall = (Texture)Resources.CarvedWall;
            Texture bricks = (Texture)Resources.Bricks;
            Texture wood = (Texture)Resources.Wood;

            // Create materials
            Material skybox_mat = new Material(
                texture: skybox,
                hoffset: 0f,
                hrepeat: 1f);
            Material carvedWall_mat = new Material(
                texture: carvedWall,
                hoffset: 0f,
                hrepeat: 1f);
            Material bricks_mat = new Material(
                texture: bricks,
                hoffset: 0f,
                hrepeat: 6.283f);
            Material wood_mat = new Material(
                texture: wood,
                hoffset: 0f,
                hrepeat: 2f);

            // Create scene
            Scene scene = new Scene(skybox_mat);

            // Create scene elements
            Element sqware = new RegularPolygon(new Vector(-0.5f, 0f), 4, .354f, wood_mat);
            Element cylinder = new RegularPolygon(new Vector(0.5f, 0f), 100, .5f, bricks_mat);
            Element tri = new RegularPolygon(new Vector(0f, 0.866f), 3, .385f, carvedWall_mat);
            Empty center = new Empty(0, 0.2868f);
            sqware.Parent = center;
            cylinder.Parent = center;
            tri.Parent = center;
            Observer pov = new Observer(Vector.Backward, 0);

            scene.AddElement(center);
            scene.AddElement(pov);

            // Add behaviours
            sqware.AddBehaviour(new Rotate { Speed = 18f });
            cylinder.AddBehaviour(new Rotate { Speed = 18f });
            tri.AddBehaviour(new Rotate { Speed = 18f });
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
            var antiAliasing = new FXAA(Renderer.CustomWidth, Renderer.CustomHeight, 70);
            Renderer.AddPostProcessing(antiAliasing);
            //antiAliasing.EdgeDettection = true;

            // Run!
            Renderer.Run(scene);

            // Release everything
            scene.Dispose();
            bricks.Dispose();
            carvedWall.Dispose();
            Console.WriteLine("Press any key to close.");
            Console.ReadKey();
        }
    }
}
