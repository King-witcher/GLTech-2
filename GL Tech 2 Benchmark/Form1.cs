using System;
using System.Diagnostics;
using GLTech2;
using GL_Tech_2_Benchmark.Properties;

namespace GL_Tech_2_Benchmark
{
    public partial class Form1 : System.Windows.Forms.Form
    {
        public Form1()
        {
            InitializeComponent();
        }


        Stopwatch global = new Stopwatch();
        Scene scene;
        Wall[] closewall;
        Wall[] farwall;

        private void Form1_Load(object sender, EventArgs e)
        {

            Material bricks = new GLBitmap(Resources.Wall);
            bricks.HorizontalRepeat = 12.6f;
            Material universe = new GLBitmap(Resources.Universe);

            Vector[] close = Vector.GetPolygon(Vector.Origin, 1f, 200);
            Vector[] far = Vector.GetPolygon(Vector.Origin, 2f, 200);

            closewall = Wall.CreatePolygon(bricks, close);
            farwall = Wall.CreatePolygon(bricks, far);
            scene = new Scene(universe);

            global.Start();
            session_stopwatch.Start();
            Renderer.Run(scene, null, Update);
        }

        int current_session = 0;
        int session_frames = 0;
        double session_frametime = 0.0;
        Stopwatch session_stopwatch = new Stopwatch();
        private void Update(double rendertime, double frametime)
        {
            const double session_maxtime = 4.0;

            session_frames++;
            session_frametime += rendertime;

            double session_time = (double) session_stopwatch.ElapsedTicks / Stopwatch.Frequency;
            double total_time = (double)global.ElapsedTicks / Stopwatch.Frequency;

            if (session_time >= session_maxtime)
            {
                Console.WriteLine(session_frames / session_frametime);

                session_frames = 0;
                session_frametime = 0.0;
                session_stopwatch.Restart();

                current_session++;

                if (current_session == 2)
                    scene.AddWalls(farwall);
                if (current_session == 4)
                    scene.AddWalls(closewall);
            }

            //camera.Turn(2 * (float)(frametime * Math.Sin(total_time)));
            //camera.Step(0.01f * (float)(frametime * Math.Sin(total_time)));
        }
    }
}
