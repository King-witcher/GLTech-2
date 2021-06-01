using GLTech2.PostProcessing;
using GLTech2.PrefabElements;
using GLTech2.Properties;
using System.Media;
using GLTech2.StandardBehaviours;
using System;

namespace GLTech2
{
    internal static class Program
    {
        static void Main()
        {
            Example();
        }

        static void Example()
        {
            SoundPlayer sp = new SoundPlayer(Resources.d_e1m1);

            Console.WriteLine("Press any key to start.");
            Console.ReadKey();
            Console.Write("\b \b");

            // Load textures frmo bitmaps
            PixelBuffer skybox = (PixelBuffer)Resources.DoomSky;
            PixelBuffer carvedWall = (PixelBuffer)Resources.CarvedWall;
            PixelBuffer bricks = (PixelBuffer)Resources.Bricks;
            PixelBuffer wood = (PixelBuffer)Resources.Wood;

            // Create materials
            Texture skybox_mat = new Texture(
                buffer: skybox,
                hoffset: 0f,
                hrepeat: 1f);
            Texture carvedWall_mat = new Texture(
                buffer: carvedWall,
                hoffset: 0f,
                hrepeat: 1f);
            Texture bricks_mat = new Texture(
                buffer: bricks,
                hoffset: 0f,
                hrepeat: 4f);
            Texture wood_mat = new Texture(
                buffer: wood,
                hoffset: 0f,
                hrepeat: 2f);

            // Create scene
            Scene scene = new Scene(skybox_mat);

            // Create scene elements
            Element sqware = new RegularPolygon(new Vector(-0.5f, 0f), 4, .354f, wood_mat);
            Element cylinder = new RegularPolygon(new Vector(0.5f, 0f), 100, .318f, bricks_mat);
            Element tri = new RegularPolygon(new Vector(0f, 0.866f), 3, .385f, carvedWall_mat);
            Empty center = new Empty(0, 0.2868f);
            sqware.Parent = center;
            cylinder.Parent = center;
            tri.Parent = center;
            Observer pov = new Observer(Vector.Backward, 0);

            scene.AddElement(center);
            scene.AddElement(pov);

            // Add behaviours
            sqware.AddBehaviour(new Rotate { Speed = 180f });
            cylinder.AddBehaviour(new Rotate { Speed = 180f });
            tri.AddBehaviour(new Rotate { Speed = 180f });
            center.AddBehaviour(new Rotate { Speed = -20f });
            pov.AddBehaviour<DebugFps>();
            pov.AddBehaviour<NoclipController>();

            var mouseLook = new MouseLook();
            mouseLook.Sensitivity = 2.2f;
            pov.AddBehaviour(mouseLook);


            // Setup Renderer
            Renderer.FullScreen = true;
            //Renderer.CustomHeight = 900;
            //Renderer.CustomWidth = 1600;
            Renderer.FieldOfView = 85;
            Renderer.ParallelRendering = true;
            Renderer.DoubleBuffering = true;

            // Add post processing effects
            //antiAliasing.EdgeDettection = true;

            // Run!
            //sp.Play();
            Renderer.Run(scene);
            sp.Stop();

            // Release everything
            scene.Dispose();
            bricks.Dispose();
            carvedWall.Dispose();
            Console.WriteLine("Press any key to close.");
            Console.ReadKey();
            Console.Write("\b \b");
            Console.WriteLine("Closing...");
        }
    }
}
