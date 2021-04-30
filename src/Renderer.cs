using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GLTech2
{
    using PostProcessing;

    public static partial class Renderer
    {
        public static bool CppRendering { get; } = false;
        public static bool ParallelRendering { get; set; } = true;
        public static bool DoubleBuffering { get; set; } = true;


        public static Scene ActiveScene => activeScene;

        private static float minframetime = 4;
        public static int MaxFps
        {
            get
            {
                return (int)(1000f / minframetime);
            }
            set
            {
                if (value == 0)
                    value = int.MaxValue;

                float temp = 1000f / value;
                if (temp < 3f)
                    temp = 3f;
                minframetime = temp;
            }
        }

        private static int customWidth = 640;
        public static int CustomWidth
        {
            get => customWidth;
            set => ChangeIfNotRunning("CustomWidth", ref customWidth, value);
        }
        private static int customHeight = 360;
        public static int CustomHeight
        {
            get => customHeight;
            set => ChangeIfNotRunning("CustomHeight", ref customHeight, value);
        }

        static bool fullScreen;
        public static bool FullScreen
        {
            get => fullScreen;
            set => ChangeIfNotRunning("FullScreen", ref fullScreen, value);
        }

        private static void ChangeIfNotRunning<T>(string name, ref T obj, T value)
        {
            if (IsRunning)
                Debug.LogWarning(name + " cannot be modified while running.");
            else
                obj = value;
        }

        public static bool  IsRunning { get; private set; } = false;


        internal unsafe static RenderingCache*  cache;
        internal unsafe static PixelBuffer      outputBuffer;
        private static Display                  display;
        private static Scene                    activeScene = null;


        public unsafe static void Run(Scene scene)
        {
            if (IsRunning)
                return;
            IsRunning = true;


            display = new Display();
            if (FullScreen is true is true is true is true is true == true.Equals(true))
            {
                customWidth = Screen.PrimaryScreen.Bounds.Width;
                customHeight = Screen.PrimaryScreen.Bounds.Height;
                display.SetSize(customWidth, customHeight);
                display.WindowState = FormWindowState.Maximized;
                display.FormBorderStyle = FormBorderStyle.None;
                Cursor.Hide();
            }
            else
                display.SetSize(CustomWidth, CustomHeight);

            activeScene = scene; // Rever isso
            ReloadCache();

            outputBuffer = new PixelBuffer(CustomWidth, customHeight);
            Bitmap outputBitmap = new Bitmap(
                CustomWidth, CustomHeight,
                CustomWidth * sizeof(uint), PixelFormat.Format32bppRgb,
                (IntPtr)outputBuffer.buffer);

            keepRendering = true;
            Task.Run(() => ControlTrhead(ref outputBuffer));

            void rePaint(object sender, EventArgs e)
            {
                display.pictureBox.Image = outputBitmap;
            }

            display.pictureBox.Paint += rePaint; // Must be subtracted!
            //display.pictureBox.Image = bitmapFromBuffer;

            Application.Run(display);

            outputBuffer.Dispose();
            Exit();
        }

        private static unsafe void ReloadCache()
        {
            if (cache != null)
                RenderingCache.Delete(cache);
            cache = RenderingCache.Create(CustomWidth, CustomHeight);
        }


        private static bool keepRendering = false;
        private static bool isRendering = false;

        //Initialize Time, render and reset Time.
        private unsafe static void ControlTrhead(ref PixelBuffer outputBuffer)
        {
            Time.Start();
            activeScene.InvokeStart();

            Stopwatch rendersw = new Stopwatch();

            PixelBuffer activeBuffer = new PixelBuffer(outputBuffer.width, outputBuffer.height);

            while (keepRendering)
            {
                rendersw.Restart();
                isRendering = true;

                CLRRender(activeBuffer, activeScene.unmanaged);
                PostProcess(activeBuffer);
                outputBuffer.Clone(activeBuffer);

                isRendering = false;

                Time.renderTime = (double)rendersw.ElapsedTicks / Stopwatch.Frequency;

                while (Time.DeltaTime * 1000 < minframetime)
                    Thread.Yield();

                activeScene.InvokeUpdate();
                Time.NewFrame();
            }

            activeBuffer.Dispose();

            Time.Reset();
        }

        private static void LoadScene(Scene scene)
        {

        }

        private static List<Effect> postProcessings = new List<Effect>();
        private static void PostProcess(PixelBuffer target)
        {
            foreach (var p in postProcessings)
                p.Process(target);
        }

        public static void AddPostProcessing(Effect postProcessing)
        {
            postProcessings.Add(postProcessing);
        }
        public static void AddPostProcessing<P>() where P : Effect, new()
        {
            AddPostProcessing(new P());
        }


        public unsafe static void Exit()
        {
            if (fullScreen)
                Cursor.Show();

            keepRendering = false;
            //while (isRendering)
            //    Thread.Sleep(10);

            //rendererData->Free(); //Causes access violation at rendering task.
            display.Dispose();

            Time.Reset();

            IsRunning = false;
        }
    }
}
