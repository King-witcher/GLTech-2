using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GLTech2
{
    public static partial class Renderer
    {
        public static bool  CppRendering { get; set; } = false;
        public static bool  ParallelRendering { get; set; } = true;
        [Obsolete]
        public static bool  AdapterMode { get; set; } = false;
        public static int   DisplayWidth { get; set; } = 1600;
        public static int   DisplayHeight { get; set; } = 900;
        public static bool  IsRunning { get; private set; } = false;


        private static Camera                   camera;
        private static Display                  display;
        private static Action<double, double>   updateMethod;
        private unsafe static RenderStruct*     rendererData;
        private static object                   locker = new object();


        static Renderer()
        {
        }


        public unsafe static void Run(Scene scene, Action start = null, Action<double, double> update = null)
        {
            if (IsRunning)
                return;
            IsRunning = true;

            display = new Display(() => { });
            display.SetSize(DisplayWidth, DisplayHeight);

            camera = new Camera(scene, display.pictureBox, update, DisplayWidth, DisplayHeight);
            rendererData = camera.unmanaged;

            updateMethod = update;


            if (AdapterMode)
            {
                camera.StartRendering();
                display.ShowDialog();
            }
            else
            {
                keepRendering = true;
                Task.Run(LoopRender);
                display.ShowDialog();
            }

            display.Dispose();
            IsRunning = false;
        }


        private static bool keepRendering = false;
        private unsafe static void LoopRender()
        {
            Stopwatch sw = new Stopwatch();
            while (keepRendering)
            {
                sw.Restart();

                if (CppRendering)
                    NativeRender(rendererData);
                else
                    CLRRender();

                double render = sw.Elapsed.Ticks / (double)Stopwatch.Frequency;
                while (sw.ElapsedMilliseconds < 7) //Don't let the framerate go higher than 143 fps.
                    Thread.Yield();
                double total = sw.Elapsed.Ticks / (double)Stopwatch.Frequency;
                updateMethod?.Invoke(render, total);
            }
        }


        public static void Stop()
        {
            keepRendering = false;
            Application.Exit(); // Not tested
        }
    }
}
