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
            PixelBuffer pb = new PixelBuffer(Resources.MapGrid);
            Dictionary<RGB, Texture> dict = new Dictionary<RGB, Texture>();

            PixelBuffer white_buffer = new PixelBuffer(Resources.Bricks);
            Texture white = new Texture(white_buffer, 0, 2f);

            PixelBuffer green_buffer = new PixelBuffer(Resources.Wood);
            Texture green = new Texture(green_buffer, 0, 1f);

            PixelBuffer purple_buffer = new PixelBuffer(Resources.GrayHexagons);
            Texture purple = new Texture(purple_buffer, 0, 2f);

            PixelBuffer background_buffer = new PixelBuffer(Resources.DoomSky);
            Texture background = new Texture(background_buffer);

            dict[(255, 255, 255)] = white;
            dict[(0, 192, 0)] = green;
            dict[(128, 0, 255)] = purple;
            Scene scene = new Scene(pb, dict, 5000);
            scene.Background = background;

            // Observer
            {
                Observer pov = new Observer((6.5f, -5f), 270);

                pov.AddBehaviour<DebugPosition>();
                pov.AddBehaviour<NoclipController>();
                var mouseLook = new MouseLook();
                mouseLook.Sensitivity = 2.2f;
                pov.AddBehaviour(mouseLook);

                scene.AddElement(pov);
            }

            Renderer.FullScreen = true;
            Renderer.FieldOfView = 85;
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
