using Game.Properties;
using System;
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

            myMap.AddWalls(myWalls);

            //Create your camera.
            myCamera = new Camera(map: myMap, width: 640, height: 360);
            myCamera.Skybox = cosmosTexture;

            //Take a PictureBox and subscribe to it's Paint event a function that updates it's display once again.
            display.Paint += (a, aa) => display.Image = myCamera.BitmapCopy;

            //Subscribe to camera.OnRender event your custom Update method wich will be called whenever the camera renders a new frame.
            myCamera.OnRender += (a, aa) => Update(a, aa);

            //Start a continuous rendering process.
            myCamera.StartShoting();
        }

        //Do whatever you want each time the engine generates a new frame.
        public void Update(Camera sender, double deltaTime)
        {
            sender.Step(2f * (float) deltaTime);
            sender.CameraAngle += 50f * (float) deltaTime;
        }
    }
}
