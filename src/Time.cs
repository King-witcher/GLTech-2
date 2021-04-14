using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GLTech2
{
    public static class Time // Can be changed.
    {
        // Accessed by Renderer.
        internal static double renderTime = 0f; // Must be 0 after stopping rendering.
        internal static float fixedTime = 0f;

        private static Stopwatch sceneStopwatch = new Stopwatch();
        private static Stopwatch frameStopwatch = new Stopwatch();

        public static float DeltaTime => GetTime(frameStopwatch);
        public static float Elapsed => GetTime(sceneStopwatch);
        internal static float FixedTime => fixedTime; // Test
        public static double RenderTime => renderTime;


        internal static void Start()
        {
            sceneStopwatch.Start();
            frameStopwatch.Start();
        }

        internal static void NewFrame()
        {
            frameStopwatch.Restart();
        }


        internal static void Reset()
        {
            sceneStopwatch.Reset();
            frameStopwatch.Reset();

            renderTime = 0f;
            fixedTime = 0f;
        }


        private static float GetTime(Stopwatch sw) => ((float)sw.ElapsedTicks) / Stopwatch.Frequency;
    }
}
