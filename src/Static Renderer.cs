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
        static bool CppRendering { get; } = false;
        public static bool ParallelRendering { get; set; } = true;



        public static Scene ActiveScene => activeScene;

        private static float minframetime = 4;
        public static int MaxFps
        {
            get => (int)(1000f / minframetime);
            set
            {
                Clip(ref value, 1, 250);
                minframetime = 1000f / value;
            }
        }

        static bool doubleBuffering = true;
        public static bool DoubleBuffering
        {
            get => doubleBuffering;
            set => ChangeIfNotRunning("DoubleBuffering", ref doubleBuffering, value);
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
            set
            {
                ChangeIfNotRunning("FullScreen", ref fullScreen, value);
                if (fullScreen == true)
                {
                    CustomWidth = Screen.PrimaryScreen.Bounds.Width;
                    customHeight = Screen.PrimaryScreen.Bounds.Height;
                }
            }
        }

        static float fieldOfView = 90f;
        public static float FieldOfView
        {
            get => fieldOfView;
            set
            {
                Clip(ref value, 1f, 179f);
                ChangeIfNotRunning("FieldOfView", ref fieldOfView, value);
            }
        }

        static bool captureMouse = true;
        public static bool CaptureMouse
        {
            get => captureMouse;
            set => captureMouse = value;    // Revisar
        }


        public static bool IsRunning { get; private set; } = false;

        static void Clip<T> (ref T value, T min, T max) where T : struct, IComparable<T>
        {
            if (value.CompareTo(max) > 0)
                value = max;
            else if (value.CompareTo(min) < 0)
                value = min;
        }

        static void ChangeIfNotRunning<T>(string name, ref T obj, T value)
        {
            if (IsRunning)
                Debug.LogWarning(name + " cannot be modified while running.");
            else
                obj = value;
        }

        unsafe static RenderingCache* cache;
        static PixelBuffer outputBuffer;
        static Scene activeScene = null;
        public unsafe static void Run(Scene scene)
        {
            if (IsRunning)
                return;
            IsRunning = true;

            activeScene = scene;

            // Unmanaged buffer where the video will be put.
            outputBuffer = new PixelBuffer(CustomWidth, customHeight);

            // Create a whapper "Bitmap" that refers to the buffer.
            var sourceBitmap = new Bitmap(
                CustomWidth, CustomHeight,
                CustomWidth * sizeof(uint), PixelFormat.Format32bppRgb,
                (IntPtr)outputBuffer.uint0);

            var display = new Display(FullScreen, CustomWidth, CustomHeight, sourceBitmap);

            // We must define two booleans to communicate with the tread.
            // The first is necessary to send a stop request.
            // The second is necessary to be aware of when the renderer doesn't need our unmanaged resources and
            // then be able to realease them all.
            var stopRequest = false;
            var controlThreadRunning = true;

            // And then start the control thread, which is reponsible for distributing the buffer among the threads
            // and running the scene scripts.
            var controlThread = Task.Run(() => ControlTrhead(outputBuffer, in stopRequest, ref controlThreadRunning));

            // Finally passes control to the rendering screen and displays it.
            Application.Run(display);

            // Theese lines run after the renderer window is closed.
            stopRequest = true;

            // Wait for the control thread to stop using outputBuffer.
            while (controlThreadRunning)
                Thread.Yield();

            // Finally, dispose everythihng.
            display.Dispose();
            outputBuffer.Dispose();
            sourceBitmap.Dispose();

            IsRunning = false;
        }

        private static unsafe void ReloadCache()
        {
            if (cache != null)
                RenderingCache.Delete(cache);
            cache = RenderingCache.Create(CustomWidth, CustomHeight, FieldOfView);
        }

        private unsafe static void ControlTrhead(
            PixelBuffer outputBuffer,
            in bool cancellationRequest,
            ref bool controlThreadRunning)
        {

            // Caches numbers that will use repeatedly by the render.
            ReloadCache();

            // Buffer where the image will be rendered
            PixelBuffer activeBuffer;
            if (DoubleBuffering)
                activeBuffer = new PixelBuffer(outputBuffer.width, outputBuffer.height);
            else
                activeBuffer = outputBuffer;

            if (!DoubleBuffering && postProcessing.Count > 0)
                Debug.InternalLog("Renderer",
                    "The renderer has post processing effects but DoubleBuffering is disabled. " +
                    "The engine will display incompletely post processed frames and cause a probably unexpected " +
                    "behaviour.",
                    Debug.Options.Warning);

            if (DoubleBuffering && postProcessing.Count == 0)
                Debug.InternalLog("Renderer", 
                    "DoubleBuffering is enabled but no post processing effect is active. If you need " +
                    "more performance or less input lag, consider disabling DoubleBuffering.",
                    Debug.Options.Info);

            // Stopwatch that counts RenderTime.
            Stopwatch controlSW = new Stopwatch();

            // Initialize everything before rendering first frame.
            activeScene.InvokeStart();
            Time.Start();

            // While this variable is set to true, outputBuffer cannot be released.
            controlThreadRunning = true;

            while (!cancellationRequest)
            {
                controlSW.Restart();

                CLRRenderLegacy(activeBuffer, activeScene.unmanaged);
                PostProcess(activeBuffer);

                // Copies the working buffer to the original.
                if (DoubleBuffering)
                    outputBuffer.Copy(activeBuffer);

                Time.renderTime = (double)controlSW.ElapsedTicks / Stopwatch.Frequency;

                // This ensures that Time.DeltaTime won't be low enough to cause undefined physics behaviour.
                while (Time.DeltaTime * 1000 < minframetime)
                    Thread.Yield();

                Mouse.Measure();
                activeScene.InvokeUpdate();
                Time.Restart();
            }

            // Tells the main thread that outputBuffer is up to be released.
            controlThreadRunning = false;

            activeBuffer.Dispose();

            Time.Reset();
        }

        private static void LoadScene(Scene scene)
        {

        }

        private static List<Effect> postProcessing = new List<Effect>();
        private static void PostProcess(PixelBuffer target)
        {
            foreach (var effect in postProcessing)
                effect.Process(target);
        }

        public static void AddEffect(Effect postProcessing)
        {
            Renderer.postProcessing.Add(postProcessing);
        }

        public static void AddPostProcessing<P>() where P : Effect, new()
        {
            AddEffect(new P());
        }
    }
}
