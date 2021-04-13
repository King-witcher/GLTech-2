using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GLTech2
{
    public static class Renderer
    {
        public static int DisplayWidth { get; set; } = 1600;
        public static int DisplayHeight { get; set; } = 900;
        public static bool ParallelRendering { get; set; } = false;
        public static bool CppRendering { get; set; } = false;
        public static bool IsRunning { get; private set; } = false;

        private static Camera cameraAdaptee;
        private static Display display;
        private static CameraAdapter camera;

        public static void Run(Scene2 scene, Action control)
        {
            if (IsRunning)
                return;
            IsRunning = true;

            display = new Display(control);
            display.SetSize(DisplayWidth, DisplayHeight);

            cameraAdaptee = new Camera(scene.Geometry, scene.Background, display.pictureBox, Update, DisplayWidth, DisplayHeight);
            cameraAdaptee.StartRendering();
            display.ShowDialog();
            display.Dispose();

            IsRunning = false;
        }

        public static void Update(double a, double b)
        {

        }

        public static void Stop()
        {
            cameraAdaptee.StopRendering();
        }

        private static void Render()
        {

        }

        internal static void UseCamera(CameraAdapter camera)
        {
            Renderer.camera = camera;
        }
    }
}
