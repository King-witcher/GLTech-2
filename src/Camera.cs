#pragma warning disable IDE1006
#define DEVELOPMENT
#define CPP
#undef PARALLEL

using System;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security;
using System.Threading;
using System.Threading.Tasks;
using static gLTech2.Debugging;

namespace gLTech2
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
        internal float camera_angle;
        internal float camera_HFOV;
        internal Vector camera_position;
        internal Map_* map;
        internal Material_* skybox; //Ainda não implmentado.

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
            result->skybox = null;
            result->camera_position = Vector.Origin;
            result->cache_angles = null;
            result->cache_cosines = null; //Atribuição possivelmente desnecessária.

            result->ReloadCaches();
            return result;
        }

        public void Dispose()
        {
            Marshal.FreeHGlobal((IntPtr)cache_angles); //Cossenos estão incluídos porque ângulos e cossenos são alocados contiguamente.
            Marshal.FreeHGlobal((IntPtr)bitmap_buffer);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal void ReloadCaches()
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

    public delegate void FrameCallBack(Camera sender, double secs);

    public sealed unsafe class Camera : IDisposable
    {
        #region Fields
        [SecurityCritical]
        internal Camera_* unmanaged;
        private Map refMap;
        private Bitmap bitmap_acessor;
        private bool beginRendering = false;
        private bool rendering = false;
        private int frame_count = 0;
        private readonly object locker = new object();
        private readonly Random random = new Random();
        #endregion

        #region Constructors
        public Camera(Map map, int width, int height)
        {
            const int pixelsize = 4;

            unmanaged = Camera_.Alloc(width, height, map.unmanaged);
            unmanaged->ReloadCaches();
            bitmap_acessor = new Bitmap(width, height, width * pixelsize, PixelFormat.Format32bppArgb, (IntPtr)unmanaged->bitmap_buffer);
            var temp = bitmap_acessor.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb);
            temp.Scan0 = (IntPtr)unmanaged->bitmap_buffer;
            bitmap_acessor.UnlockBits(temp);
            this.refMap = map;
        }
        #endregion

        #region Properties
        //private Bitmap Skybox { get => data->skybox; set => data->skybox = value; }
        public Bitmap Frame => bitmap_acessor;
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
        public float CameraAngle { get => unmanaged->camera_angle; set => unmanaged->camera_angle = value; }
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
                else if (value <= 0)
                    return;
                unmanaged->camera_HFOV = value;
                unmanaged->ReloadCaches();
            }
        }
        public double AverateFrameTime => unmanaged->averageFrametime;
        public int DisplayWidth { get => unmanaged->bitmap_width; private set => unmanaged->bitmap_width = value; }
        public int DisplayHeight { get => unmanaged->bitmap_height; private set => unmanaged->bitmap_height = value; }
        public int FrameCount { get => frame_count; private set => frame_count = value; }
        public object Locker { get => locker; }
        public Map Map => refMap;
        public Vector Camera_Position { get => unmanaged->camera_position; set => unmanaged->camera_position = value; }
        #endregion

        #region Events
        public event FrameCallBack OnRender;
        #endregion

        #region Methods
        public void BeginShooting()
        {
            if (beginRendering == true)
                return;
            beginRendering = true;
            Task.Run(() =>
            {
                Stopwatch timer = new Stopwatch();
                while (beginRendering)
                {
                    timer.Restart();
                    Render();
                    while (timer.ElapsedMilliseconds < 7)
                        Thread.Yield();

                    OnRender?.Invoke(this, timer.Elapsed.Ticks / (double)Stopwatch.Frequency);
                }
            });
        }

        public void Dispose()
        {
            unmanaged->Dispose();
            Marshal.FreeHGlobal((IntPtr)unmanaged);
        }

        public void Shoot()
        {
            while (rendering)
                Thread.Yield();
            Task.Run(() =>
            {
                Stopwatch timer = Stopwatch.StartNew();
                Render();
                double currentframetime = timer.Elapsed.Ticks / (double) Stopwatch.Frequency;
                double averageframetime = unmanaged->averageFrametime;
                unmanaged->averageFrametime = 0.9 * averageframetime + 0.1 * currentframetime;
                OnRender?.Invoke(this, currentframetime);
            });
        }

        public void SetResolution(int width, int height)
        {
            lock (locker)
            {
                Marshal.ReAllocHGlobal((IntPtr)unmanaged->bitmap_buffer, (IntPtr) (4 * width * height));
                unmanaged->bitmap_height = height;
                unmanaged->bitmap_width = width;
                unmanaged->ReloadCaches();
            }
        }

        public void Step(float amount) => unmanaged->camera_position += new Vector(unmanaged->camera_angle) * amount;

        public void Step(float amount, float angle) => unmanaged->camera_position += new Vector(unmanaged->camera_angle + angle) * amount;

        public void StopShooting() => beginRendering = false;

        //private void TryStep(float amount) { }

        public void Turn(float amount) => unmanaged->camera_angle += amount;

        [DllImport(@"glt2_nat.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern void NativeRender(Camera_* camera);

        private unsafe void Render()
        {
#if CPP
            NativeRender(unmanaged);
            return;
#endif
            lock (locker)
            {
                rendering = true;
#if PARALLEL
                Parallel.For(0, data->bitmap_width, (ray_id) =>
                {
#else
                for (int ray_id = 0; ray_id < unmanaged->bitmap_width; ray_id++)
                {
#endif
                    //Debug("Ray id: " + ray_id);
                    Ray ray = new Ray(unmanaged->camera_position, unmanaged->camera_angle + unmanaged->cache_angles[ray_id]);

                    //Cast the ray towards every wall.
                    Wall_* nearest = ray.NearestWall(unmanaged->map, out float nearest_dist, out float nearest_ratio);
                    if (nearest_dist != float.PositiveInfinity)
                    {
                        float columnHeight = (unmanaged->cache_colHeight1 / (unmanaged->cache_cosines[ray_id] * nearest_dist));
                        float fullColumnRatio = unmanaged->bitmap_height / columnHeight;
                        float topIndex = -(fullColumnRatio - 1f) / 2f;
                        for (int line = 0; line < unmanaged->bitmap_height; line++)
                        {
                            float vratio = topIndex + fullColumnRatio * line / unmanaged->bitmap_height;
                            if (vratio < 0f || vratio >= 1f)
                            {
                                unmanaged->bitmap_buffer[unmanaged->bitmap_width * line + ray_id] = Background(line);
                            }
                            else
                            {
                                int pixel = nearest->material.MapPixel(nearest_ratio, vratio);
                                unmanaged->bitmap_buffer[unmanaged->bitmap_width * line + ray_id] = pixel;
                            }
                        }
                    }
                    else
                    {
                        for (int line = 0; line < unmanaged->bitmap_height; line++)
                            unmanaged->bitmap_buffer[unmanaged->bitmap_width * line + ray_id] = Background(line);
                    }
#if PARALLEL
                });
#else
                }
#endif
                rendering = false;
            }

            int Background(int line)
            {
                if (line < unmanaged->bitmap_height >> 1)
                    return -14_803_426;
                else
                    return -12_171_706;
            }
        }
        #endregion
    }
}