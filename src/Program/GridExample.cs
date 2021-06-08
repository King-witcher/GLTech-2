using GLTech2.PrefabBehaviours;
using GLTech2.PrefabElements;
using System.Collections.Generic;

namespace GLTech2
{
    using GLTech2.Properties;
    partial class Program
    {
        static void GridExample()
        {
            using (PixelBuffer pb = new PixelBuffer(Resources.MapGrid))
            using (PixelBuffer white_buffer = new PixelBuffer(Resources.Bricks))
            using (PixelBuffer green_buffer = new PixelBuffer(Resources.Wood))
            using (PixelBuffer purple_buffer = new PixelBuffer(Resources.GrayHexagons))
            using (PixelBuffer background_buffer = new PixelBuffer(Resources.DoomSky))
            {
                Scene scene;
                {
                    Dictionary<RGB, Texture> dict = new Dictionary<RGB, Texture>();
                    {
                        Texture white = new Texture(white_buffer, 0, 2f);
                        Texture green = new Texture(green_buffer, 0, 1f);
                        Texture purple = new Texture(purple_buffer, 0, 2f);

                        dict[(255, 255, 255)] = white;
                        dict[(0, 192, 0)] = green;
                        dict[(128, 0, 255)] = purple;
                    }

                    scene = new Scene(pb, dict, 5000);

                    Texture background = new Texture(background_buffer);
                    scene.Background = background;
                }

                // Observer
                {
                    Observer pov = new Observer((6.5f, -5f), 270);

                    pov.AddBehaviour<DebugPosition>();
                    pov.AddBehaviour<NoclipController>();
                    pov.AddBehaviour(new MouseLook(2.2f));

                    scene.AddElement(pov);
                }

                // Renderer customization
                Renderer.FullScreen = true;
                Renderer.FieldOfView = 85f;
                Renderer.ParallelRendering = true;
                Renderer.DoubleBuffering = true;
                Renderer.CaptureMouse = true;

                // Run!
                Renderer.Run(scene);

                // Release scene
                scene.Dispose();
            }
        }
    }
}
