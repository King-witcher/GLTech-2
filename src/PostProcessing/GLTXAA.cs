using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace GLTech2.PostProcessing
{
    //Incomplete
    internal sealed unsafe class GLTXAA : Effect, IDisposable
    {
        public GLTXAA(int width, int height, int threshold = 70)
        {
            previousFrame = new PixelBuffer(width, height);
            temporaryBuffer = new PixelBuffer(width, height);
            if (threshold > 255)
                this.sqrThreshold = 255 * 255 * 3;
            else if (threshold < 0)
                this.sqrThreshold = 0;
            else
                this.sqrThreshold = threshold * threshold * 3;
        }

        private PixelBuffer previousFrame;
        private PixelBuffer temporaryBuffer;
        private int sqrThreshold;

        public bool EdgeDettection { get; set; } = false;

        internal override void Process(PixelBuffer target)
        {
            if (target.width != previousFrame.width || target.height != previousFrame.height)
                return;

            temporaryBuffer.Copy(target);

            if (!EdgeDettection)
            {
                Parallel.For(1, target.height - 1, (y) =>
                {
                    for (int x = 1; x < target.width - 1; x++)
                    {
                        int cur = target.width * y + x;
                        int up = target.width * (y - 1) + x;
                        int left = target.width * y + x - 1;
                        int down = target.width * (y + 1) + x;
                        int right = target.width * y + x + 1;

                        int differenceV = dist(
                            target.uint0[up],
                            target.uint0[down]);

                        int differenceH = dist(
                            target.uint0[right],
                            target.uint0[left]);

                        float factorv = differenceV / (255f * 255f * 3f);

                        int edge = differenceH + differenceV;
                        float factor = edge / (255f * 2);
                        factor = factor > 0.05f ? 0.5f : 0.0f;

                        temporaryBuffer.uint0[cur] = avg(
                            previousFrame.uint0[cur],
                            target.uint0[cur],
                            factor);

                        //copy.buffer[cur] = (uint)(factor * 255) * 0x00010101 + 0xff000000;
                    }
                });

                target.Copy(temporaryBuffer);
                previousFrame.Copy(target);
            }
            else
            {
                Parallel.For(1, target.height - 1, (y) =>
                {
                    for (int x = 1; x < target.width - 1; x++)
                    {
                        int cur = target.width * y + x;
                        int up = target.width * (y - 1) + x;
                        int left = target.width * y + x - 1;
                        int down = target.width * (y + 1) + x;
                        int right = target.width * y + x + 1;

                        int differenceV = dist(
                            target.uint0[up],
                            target.uint0[down]);

                        int differenceH = dist(
                            target.uint0[right],
                            target.uint0[left]);

                        float factorv = differenceV / (255f * 255f * 3f);

                        int edge = differenceH + differenceV;
                        float factor = edge / (255f * 2);

                        temporaryBuffer.uint0[cur] = avg(
                            previousFrame.uint0[cur],
                            target.uint0[cur],
                            factor);

                        temporaryBuffer.uint0[cur] = (uint)(factor * 255) * 0x10101 + 0xff000000;
                    }
                });
                previousFrame.Copy(target);
                target.Copy(temporaryBuffer);
            }

            target.Copy(temporaryBuffer);
            previousFrame.Copy(target);
            return;

            int dist(uint pixel1, uint pixel2)
            {
                int sum = 0;
                int tmp;

                tmp = (byte)pixel1 - (byte)pixel2;
                sum += tmp;

                pixel1 >>= 8;
                pixel2 >>= 8;

                tmp = (byte)pixel1 - (byte)pixel2;
                sum += tmp;

                pixel1 >>= 8;
                pixel2 >>= 8;

                tmp = (byte)pixel1 - (byte)pixel2;
                sum += tmp;

                sum = Math.Abs(sum) / 3;

                return sum;
            }

            uint avg(uint pixel1, uint pixel2, float factor1)
            {
                uint res = 0;
                for (int i = 0; i < 3; i++)
                {
                    res += (uint)(factor1 * (byte)pixel1 + (1 - factor1) * (byte)pixel2) << (8 * i);
                    pixel1 >>= 8;
                    pixel2 >>= 8;
                }
                res += 0xff000000;
                return res;
            }
        }

        public void Dispose()
        {
            previousFrame.Dispose();
        }
    }
}
