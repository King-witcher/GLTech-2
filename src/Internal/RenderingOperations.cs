using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace GLTech2.Internal
{
    internal static class RenderingOperations
    {

        private static bool keepRendering = false;
        private static bool isRendering = false;

        //Initialize Time, render and reset Time.
        internal unsafe static void ControlTrhead(PixelBuffer outputBuffer, Scene activeScene)
        {
            Time.Start();
            activeScene.InvokeStart();

            Stopwatch swtest = new Stopwatch();
            while (keepRendering)
            {
                swtest.Restart();
                isRendering = true;
                //if (CppRendering)
                //    NativeRender(rendererData);
                //else
                //CLRRender(rendererData->bitmap_buffer);
                //PostProcess();
                isRendering = false;

                Time.renderTime = (double)swtest.ElapsedTicks / Stopwatch.Frequency;

                //while (Time.DeltaTime * 1000 < minframetime)
                    //Thread.Yield();

                activeScene.InvokeUpdate();
                Time.NewFrame();
            }

            Time.Reset();
        }
    }
}
