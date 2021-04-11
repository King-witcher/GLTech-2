using Game.Properties;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using GLTech2;

namespace Game
{
    public partial class MainWindow : Form
    {
        Camera myCamera;
        Map myMap;
        

        public MainWindow() =>
            InitializeComponent();


        public void OnLoad(object sender, EventArgs e)
        {
            //Allocate space in memory to the map and create a camera.
            myMap = new Map(maxWalls: 512, maxSprities: 512);

            //Texture32 is only alocated once for every object that refers to it.
            Texture32 wallTexture = new Texture32(Resources.Wall);
            Texture32 cosmosTexture = new Texture32(Resources.Universe);

            //Material is a flyweight structure that refers to a texture and is alocated one for each object that uses.
            Material myMaterial = new Material(wallTexture);

            //Create your walls and add them to the map.
            Wall[] myWalls = Wall.GetRegularPolygon(
                center: Vector.Origin,
                radius: 0.5f,
                edges: 500,
                texture: wallTexture);

            //myMap.AddWalls(myWalls);

            //Create your camera.
            myCamera = new Camera(map: myMap, width: 640, height: 360);
            myCamera.Skybox = new Material(
                texture: cosmosTexture,
                hoffset: 0f,
                hrepeat: 3f);

            //Take a PictureBox and subscribe to it's Paint event a function that updates it's display once again.
            display.Paint += RefreshPictureBox;

            //Subscribe to camera.OnRender event your custom Update method wich will be called whenever the camera renders a new frame.
            myCamera.OnRender += (a, aa) => Update(a, aa);

            //Start a continuous rendering process.
            myCamera.StartShoting();
        }

        private void RefreshPictureBox(object sender, PaintEventArgs e)
        {
            (sender as PictureBox).Image = myCamera.BitmapCopy;
        }

        //Do whatever you want each time the engine generates a new frame.
        double[] timeRegistry = new double[1000];
        int registryCount = 0;
        public void Update(Camera sender, double deltaTime)
        {
            timeRegistry[registryCount++] = deltaTime * 1000;
            if (registryCount == 1000)
            {
                Console.WriteLine("Average: " + timeRegistry.Average());
                Console.WriteLine("SD: " + CalculateStandardDeviation(timeRegistry));
                registryCount = 0;
            }

            sender.Step(2f * (float) deltaTime);
            sender.CameraAngle += 50f * (float) deltaTime;
        }

        private double CalculateStandardDeviation(IEnumerable<double> values)
        {
            double standardDeviation = 0;

            if (values.Any())
            {
                // Compute the average.     
                double avg = values.Average();

                // Perform the Sum of (value-avg)_2_2.      
                double sum = values.Sum(d => Math.Pow(d - avg, 2));

                // Put it all together.      
                standardDeviation = Math.Sqrt((sum) / (values.Count() - 1));
            }

            return standardDeviation;
        }
    }
}
