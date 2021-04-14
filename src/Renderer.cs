using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
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

        private static float minframetime = 7;
        private static int MaxFps
        {
            get
            {
                return (int)(1000f / minframetime);
            }
            set
            {
                float temp = 1000f / value;
                if (temp < 1f)
                    temp = 1f;
                minframetime = temp;
            }
        }

        private static int displayWidth = 640;
        public static int DisplayWidth
        {
            get => displayWidth;
            set
            {
                if (IsRunning)
                    throw new AccessViolationException("Render.DisplayWidth cannot be modified while running.");
                displayWidth = value;
            }
        }
        private static int displayHeight = 360;
        public static int DisplayHeight
        {
            get => displayHeight;
            set
            {
                if (IsRunning)
                    throw new AccessViolationException("Render.DisplayHeight cannot be modified while running.");
                displayHeight = value;
            }
        }
        public static bool  IsRunning { get; private set; } = false;
        public static Texture32 Screenshot
        {
            get
            {
                return new Texture32(bufferBitmap.Clone() as Bitmap);
            }
        }


        internal static Bitmap                  bufferBitmap;
        private static readonly int             pixelsize = 4;
        private static Display                  display;
        private static Action<double, double>   updateMethod;
        private unsafe static RenderStruct*     rendererData;
        private static Scene scene = null;


        unsafe static Renderer()
        {
            rendererData = RenderStruct.Alloc(DisplayWidth, DisplayHeight, null);

            bufferBitmap = new Bitmap(DisplayWidth, DisplayHeight, DisplayWidth * pixelsize, PixelFormat.Format32bppArgb, (IntPtr)rendererData->bitmap_buffer);
            var temp = bufferBitmap.LockBits(new Rectangle(0, 0, DisplayWidth, DisplayHeight), ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb);
            temp.Scan0 = (IntPtr)rendererData->bitmap_buffer;
            bufferBitmap.UnlockBits(temp);
        }


        public unsafe static void Run(Scene scene, Action start = null, Action<double, double> update = null)
        {
            if (IsRunning)
                return;
            IsRunning = true;

            Renderer.scene = scene;

            display = new Display(start);
            display.SetSize(DisplayWidth, DisplayHeight);

            rendererData = RenderStruct.Alloc(DisplayWidth, DisplayHeight, scene.unmanaged);
            //var camera = new Camera(scene, display.pictureBox, update, DisplayWidth, DisplayHeight);
            //rendererData = camera.unmanaged;

            bufferBitmap = new Bitmap(DisplayWidth, DisplayHeight, DisplayWidth * pixelsize, PixelFormat.Format32bppArgb, (IntPtr)rendererData->bitmap_buffer);
            var temp = bufferBitmap.LockBits(new Rectangle(0, 0, DisplayWidth, DisplayHeight), ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb);
            temp.Scan0 = (IntPtr)rendererData->bitmap_buffer;
            bufferBitmap.UnlockBits(temp);

            updateMethod = update;


            keepRendering = true;
            Task.Run(LoopRender);
            display.pictureBox.Paint += (a, aa) => { display.pictureBox.Image = bufferBitmap; };
            display.pictureBox.Image = new Bitmap(1, 1);
            display.ShowDialog();

            display.Dispose();
            IsRunning = false;
        }


        private static bool keepRendering = false;
        private unsafe static void LoopRender()
        {
            Stopwatch sw = new Stopwatch();
            scene.InvokeStart();
            while (keepRendering)
            {
                sw.Restart();

                if (CppRendering)
                    NativeRender(rendererData);
                else
                    CLRRender();

                double render = sw.Elapsed.Ticks / (double)Stopwatch.Frequency;
                while (sw.ElapsedMilliseconds < minframetime)
                    Thread.Yield();
                double frame = sw.Elapsed.Ticks / (double)Stopwatch.Frequency;
                updateMethod?.Invoke(frame, render);
                //scene.InvokeUpdate(frame, render);
            }
        }


        public static void Stop()
        {
            keepRendering = false;
            Application.Exit(); // Not tested
        }
    }
}
