#pragma warning disable IDE1006
#define DEVELOPMENT
#undef CPP
#undef PARALLEL

using GLTech2.Properties;
using System;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using static GLTech2.Debugging;

namespace GLTech2
{
    [StructLayout(LayoutKind.Sequential)]
    internal unsafe struct Camera_ : IDisposable
    {
        internal double averageFrametime;
        internal volatile Int32* bitmap_buffer;
        internal int bitmap_height;
        internal int bitmap_width;
        internal float* cache_angles;
        internal float cache_colHeight1;
        internal float* cache_cosines;
        internal float camera_angle; //MUST be 0 <= x < 360
        internal float camera_HFOV;
        internal Vector camera_position;
        internal Map_* map;
        internal Material_* background;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static Camera_* Alloc(int width, int height, Map_* map)
        {
            Camera_* result = (Camera_*)Marshal.AllocHGlobal(sizeof(Camera_));
            result->bitmap_height = height;
            result->bitmap_width = width;
            result->bitmap_buffer = (Int32*)Marshal.AllocHGlobal(sizeof(Int32) * width * height);
            result->averageFrametime = 0f;
            result->camera_angle = 0f;
            result->camera_HFOV = 90f;
            result->map = map;
            result->background = null;
            result->camera_position = Vector.Origin;
            result->cache_angles = null;
            result->cache_cosines = null; //Atribuição possivelmente desnecessária.

            result->RefreshCaches();
            return result;
        }

        public void Dispose()
        {
            Marshal.FreeHGlobal((IntPtr)cache_angles); //Cossenos estão incluídos porque ângulos e cossenos são alocados contiguamente.
            Marshal.FreeHGlobal((IntPtr)bitmap_buffer);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal void RefreshCaches()
        {
            const double TORAD = Math.PI / 180.0f;
            double tan = Math.Tan(TORAD * camera_HFOV / 2);

            cache_colHeight1 = bitmap_width / (float)(2 * tan);

            //Ambos os caches são alocados lado a lado para facilitar o chaching em nível de processador.
            if (cache_angles != null)
                Marshal.ReAllocHGlobal((IntPtr)cache_angles, (IntPtr) (2 * sizeof(float) * bitmap_width));
            else
                cache_angles = (float*)Marshal.AllocHGlobal(2 * sizeof(float) * bitmap_width);
            cache_cosines = cache_angles + bitmap_width;

            double step = 2 * tan / (bitmap_width - 1);
            for (int i = 0; i < bitmap_width; i++)
            {
                float angle = (float)(Math.Atan(i * step - tan) / TORAD);
                cache_angles[i] = angle;
                cache_cosines[i] = (float)(Math.Cos(TORAD * angle));
            }
        }
    }

    [Obsolete]
    public delegate void FrameCallBack(Camera sender, double secs);

    public delegate void FrameUpdateCallback(double renderTime, double elapsedTime);

    public unsafe class Camera : IDisposable
    {
        public Camera(Map map, Material background, PictureBox output, FrameUpdateCallback updateCallback, int width = 640, int height = 360)
        {
            const int pixelsize = 4;

            refMap = map;
            unmanaged = Camera_.Alloc(width, height, map.unmanaged);
            Background = background;
            SetOutput(output);

            RenderCallback = updateCallback;

            //Possibly obsolete
            bufferBitmap = new Bitmap(width, height, width * pixelsize, PixelFormat.Format32bppArgb, (IntPtr)unmanaged->bitmap_buffer);
            var temp = bufferBitmap.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb);
            temp.Scan0 = (IntPtr)unmanaged->bitmap_buffer;
            bufferBitmap.UnlockBits(temp);
        }

        [SecurityCritical]
        internal Camera_* unmanaged;
        private readonly Random random = new Random();


        private Map refMap;
        public Map Map => refMap;

        private Material refBackground;
        public Material Background
        {
            get
            {
                return refBackground;
            }
            set
            {
                refBackground = value ?? throw new ArgumentNullException();
                unmanaged->background = value.unmanaged;
            }
        }

        [Obsolete]
        public Bitmap BufferBitmap => bufferBitmap;
        private Bitmap bufferBitmap;
        public Bitmap BitmapCopy
        {
            get
            {
                lock (locker)
                {
                    return new Bitmap(
                        unmanaged->bitmap_width,
                        unmanaged->bitmap_height,
                        unmanaged->bitmap_width * 4,
                        PixelFormat.Format32bppArgb,
                        (IntPtr)unmanaged->bitmap_buffer);
                }
            }
        }
        public float CameraAngle
        {
            get => unmanaged->camera_angle;
            set
            {
                //Ensures the angle restriction
                if (value >= 0)
                    unmanaged->camera_angle = value % 360;
                else
                    unmanaged->camera_angle = value % 360 + 1;
            }
        }

        public float FOV
        {
            get
            {
                return unmanaged->camera_HFOV;
            }
            set
            {
                if (value > 179)
                    value = 179;
                else if (value < 1)
                    value = 1;
                unmanaged->camera_HFOV = value;
                unmanaged->RefreshCaches();
            }
        }

        public double AverageFrameTime => unmanaged->averageFrametime;
        public int DisplayWidth { get => unmanaged->bitmap_width; private set => unmanaged->bitmap_width = value; }
        public int DisplayHeight { get => unmanaged->bitmap_height; private set => unmanaged->bitmap_height = value; }
        private int frame_count = 0;
        public int FrameCount { get => frame_count; private set => frame_count = value; }

        private readonly object locker = new object();
        [Obsolete]
        public object Locker { get => locker; }
        public Vector Camera_Position { get => unmanaged->camera_position; set => unmanaged->camera_position = value; }

        [Obsolete]
        public event FrameCallBack OnRender;

        public void Dispose()
        {
            unmanaged->Dispose();
            Marshal.FreeHGlobal((IntPtr)unmanaged);
        }

        public void SetResolution(int width, int height)
        {
            lock (locker)
            {
                Marshal.ReAllocHGlobal((IntPtr)unmanaged->bitmap_buffer, (IntPtr) (4 * width * height));
                unmanaged->bitmap_height = height;
                unmanaged->bitmap_width = width;
                unmanaged->RefreshCaches();
            }
        }

        [Obsolete]
        public void Step(float amount) => unmanaged->camera_position += new Vector(unmanaged->camera_angle) * amount;

        [Obsolete]
        public void Step(float amount, float angle) => unmanaged->camera_position += new Vector(unmanaged->camera_angle + angle) * amount;


        [Obsolete]
        public void Turn(float amount) => CameraAngle += amount;


        public PictureBox Output { get; private set; }
        private void RePaint(object sender, PaintEventArgs e) =>
            ((PictureBox)sender).Image = BitmapCopy;
        public void SetOutput(PictureBox pictureBox)
        {
            if (Output != null)
                Output.Paint -= RePaint;
            if (pictureBox != null)
                Output = pictureBox;
            Output.Paint += RePaint;
        }

        private FrameUpdateCallback RenderCallback;

        private bool rendering = false;
        [Obsolete]
        public void Render()
        {
            while (rendering)
                Thread.Yield();
            Task.Run(() =>
            {
                Stopwatch timer = Stopwatch.StartNew();
                CLRRender();
                double currentframetime = timer.Elapsed.Ticks / (double)Stopwatch.Frequency;
                double averageframetime = unmanaged->averageFrametime;
                unmanaged->averageFrametime = 0.9 * averageframetime + 0.1 * currentframetime;
                RenderCallback(0, timer.Elapsed.Ticks / (double)Stopwatch.Frequency);
            });
        }

        private bool keepRendering = false;
        public void StartRendering()
        {
            if (keepRendering == true)
                return;
            keepRendering = true;
            Task.Run(() =>
            {
                Stopwatch timer = new Stopwatch();
                while (keepRendering)
                {
                    timer.Restart();
                    CLRRender();
                    while (timer.ElapsedMilliseconds < 7) //Don't let the framerate go higher than 143 fps.
                        Thread.Yield();
                    //Mexer no frametime
                    RenderCallback(0, timer.Elapsed.Ticks / (double)Stopwatch.Frequency);
                    //OnRender?.Invoke(this, timer.Elapsed.Ticks / (double)Stopwatch.Frequency);
                }
            });
        }

        public void StopRendering() =>
            keepRendering = false;

        //Don't know how to pinvoke fromm current directory =/
        [Obsolete("Don't change the directory of the files yet!")]
        [DllImport(@"D:\GitHub\GLTech-2\bin\Release\glt2_nat.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern void NativeRender(Camera_* camera);

        private unsafe void CLRRender()
        {
#if CPP
            NativeRender(unmanaged);
            return;
#endif
            lock (locker)
            {
                rendering = true;

                //Caching frequently used values.
                int display_width = unmanaged->bitmap_width;
                int display_height = unmanaged->bitmap_height;
                Material_ background = *unmanaged->background;
#if PARALLEL
                ParallelLoopResult plr = Parallel.For(0, unmanaged->bitmap_width, (ray_id) =>
                {
#else
                for (int ray_id = 0; ray_id < unmanaged->bitmap_width; ray_id++)
                {
#endif
                    //Caching
                    float ray_cos = unmanaged->cache_cosines[ray_id];
                    float ray_angle = unmanaged->cache_angles[ray_id] + unmanaged->camera_angle;
                    Ray ray = new Ray(unmanaged->camera_position, ray_angle);

                    int GetBackground(int line)
                    {
                        float hratio = ray_angle / 360 + 1; //Temporary bugfix to avoid hratio being < 0

                        float screenVratio = (float) line / display_height;
                        float vratio = (1 - ray_cos) / 2 + ray_cos * screenVratio;
                        return background.MapPixel(hratio, vratio);
                    }

                    //Cast the ray towards every wall.
                    Wall_* nearest = ray.NearestWall(unmanaged->map, out float nearest_dist, out float nearest_ratio);
                    if (nearest_dist != float.PositiveInfinity)
                    {
                        float columnHeight = (unmanaged->cache_colHeight1 / (ray_cos * nearest_dist));
                        float fullColumnRatio = display_height / columnHeight;
                        float topIndex = -(fullColumnRatio - 1f) / 2f;
                        for (int line = 0; line < display_height; line++)
                        {
                            float vratio = topIndex + fullColumnRatio * line / display_height;
                            if (vratio < 0f || vratio >= 1f)
                            {
                                unmanaged->bitmap_buffer[display_width * line + ray_id] = GetBackground(line);
                            }
                            else
                            {
                                int pixel = nearest->material.MapPixel(nearest_ratio, vratio);
                                unmanaged->bitmap_buffer[display_width * line + ray_id] = pixel;
                            }
                        }
                    }
                    else
                    {
                        for (int line = 0; line < display_height; line++)
                            unmanaged->bitmap_buffer[display_width * line + ray_id] = GetBackground(line);
                    }
#if PARALLEL
                });
#else
                }
#endif
                rendering = false;
            }
        }
    }
}