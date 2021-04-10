using Game.Properties;
using System;
using System.Windows.Forms;
using gLTech2;

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
            myMap = new Map(500, 500);

            //Texture32 is only alocated once for every object that refers to it.
            Texture32 myTexture = new Texture32(Resources.Universe);

            //Material is a flyweight structure that refers to a texture and is alocated one for each object that uses.
            Material myMaterial = new Material(myTexture);

            //Create your walls and add them to the map.
            Wall[] myWalls = Wall.GetRegularPolygon(
                center: Vector.Origin,
                radius: 1.5f,
                edges: 4,
                texture: myTexture);

            myMap.AddWalls(myWalls);

            //Create your camera.
            myCamera = new Camera(myMap, 640, 360);

            //Take a PictureBox and subscribe to it's Paint event a function that updates it's display once again.
            display.Paint += (a, aa) => display.Image = myCamera.BitmapCopy;

            //Subscribe to camera.OnRender event your custom Update method wich will be called whenever the camera renders a new frame.
            myCamera.OnRender += (a, aa) => myCamera.Turn(0.3f);

            //Start a continuous rendering process.
            myCamera.BeginShooting();
        }
    }
}
