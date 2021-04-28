using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace GLTech2.PostProcessment
{
    public sealed unsafe class FXAA : PostProcessing, IDisposable
    {
        public FXAA(int width, int height, int threshold)
        {
            pixelBuffer = new PixelBuffer(width, height);
            if (threshold > 255)
                this.sqrThreshold = 255 * 255 * 3;
            else if (threshold < 0)
                this.sqrThreshold = 0;
            else
                this.sqrThreshold = threshold * threshold * 3;
        }

        private PixelBuffer pixelBuffer;
        private int sqrThreshold;

        internal override void Process(PixelBuffer target)
        {
            if (target.width != pixelBuffer.width || target.height != pixelBuffer.height)
                return;

            pixelBuffer.Clone(target);

            Parallel.For(1, target.height, (i) =>
            {
                for (int j = 1; j < target.width; j++)
                {
                    int cur = target.width * i + j;
                    int up = target.width * (i - 1) + j;
                    int left = target.width * i + j - 1;

                    int differenceV = dist(
                        target.buffer[cur],
                        target.buffer[up]);

                    int differenceH = dist(
                        target.buffer[cur],
                        target.buffer[left]);

                    if (differenceV >= sqrThreshold)
                        pixelBuffer.buffer[target.width * i + j] = avg(target.buffer[up], target.buffer[cur]);
                    else if (differenceH >= sqrThreshold)
                        pixelBuffer.buffer[target.width * i + j] = avg(target.buffer[left], target.buffer[cur]);

                }
            });

            target.Clone(pixelBuffer);
            return;


            int dist(uint pixel1, uint pixel2)
            {
                int sum = 0;
                int tmp;

                tmp = (byte)pixel1 - (byte)pixel2;
                tmp *= tmp;
                sum += tmp;
                pixel1 >>= 8;
                pixel2 >>= 8;
                tmp = (byte)pixel1 - (byte)pixel2;
                tmp *= tmp;
                sum += tmp;
                pixel1 >>= 8;
                pixel2 >>= 8;
                tmp = (byte)pixel1 - (byte)pixel2;
                tmp *= tmp;
                sum += tmp;
                pixel1 >>= 8;
                pixel2 >>= 8;

                return sum;
            }

            uint avg(uint pixel1, uint pixel2)
            {
                uint res = 0;
                for (int i = 0; i < 3; i++)
                {
                    res += (uint)((byte)pixel1 + (byte)pixel2) / 2 << (8 * i);
                    pixel1 >>= 8;
                    pixel2 >>= 8;
                }
                res += 0xff000000;
                return res;
            }
        }

        public void Dispose()
        {
            pixelBuffer.Dispose();
        }
        
        ~FXAA()
        {
            Dispose();
        }
    }
}
