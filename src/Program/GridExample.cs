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

            dict[(255, 255, 255)] = white;
            Scene scene = new Scene(pb, dict, 5000);

            // Observer
            {
                Observer pov = new Observer(Vector.Backward, 0);

                pov.AddBehaviour<DebugFps>();
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
