# GL Tech 2
GL Tech 2 is a 3D engine designed with it's interface entirely in C# and parts of the rendering engine made in C++ that renders that allows you to render 2D maps as if they are 3D due to the _ray casting_ technique, without need of a GPU.The engine was entirely projected by Giuseppe Lanna, but still in development stage.

**How to build**
If you already have Visual Studio Community 2019 installed, just open `solution.sln` and press F5 or make whatever you want with the IDE.

## Hello World game
In this tutorial, you'll learn the basics of how to use the engine to build maps and render from whatever perspective you want.

After adding a libary reference in your Windows Forms Application project and referencing it with `using GLTech2`, we start double clicking the main Form and allocating space in memory for the map int the Form_Load method:
```
Map map = new Map(maxWalls: 500, maxSprities:5);
```
It allocates a map that fits up to 500 walls and 5 sprities. Sprities are not yet implemented. If you exceed that numbers, an IndexOutOfBoundsException will be thrown.
Now, we need to create a texture element to use in our walls:

    Bitmap bitmap = new Bitmap(1, 1);
    Texture32 texture = new Texture32(bitmap);
We're using an empty 1x1 bitmap, but you can use whatever bitmap you want. Try using `new Bitmap(filename)` constructor.
The `Texture32 : IDisposable` class is nearly the same as the .NET `Bitmap` class, since it stores and manages a buffer in the memory that represents an image, but optimized for the engine purposes.

> Remarks:
 When you create a Texture32 instance from a bitmap, it creates a copy of the image in a place in memory with a 32 bits per pixel setting due to performance optimizations and it won't be released if caught by the runtime Garbage Collector, so make sure to dispose it if not necessary anymore.

Since Texture32 is uses a relativelly fat amout of memory, GLTech2 doesn't use an instance of it fore every single wall or sprite in the game. Instead, we're gonna use a _fliweight_ version of it: the ```Material : IDisposable``` class, that contains information about size, cut and repeating pattern of a texture and the reference to it's buffer in memory.
```
Material material = new Material(texture);
```

> Remarks:
> Just like Texture32, material has unmanaged memory that purposely can't be released by the CLR, so make sure to dispose it as soon as needless.
> Texture32 can be implicitly cast to Material, so you can use texture objects instead of bothering instantiating new material object. It'll automatically create a new Material element, but doing this is not recommended doe to ambiguous semantics and the fact that the Material element created can't be released in memory anymore.

After creating our material object, you could create a ```Wall : IDisposable``` put the material in, and add the wall to the map, just like below.
```
Vector vector1 = new Vector(1, 0);
Wall wall = new Wall(
		start: vector1, 
		angle_deg: 90,
		material: material);
map.AddWall(wall);
```
There are many ways to create sets of walls that build an object at once, but this tutorial won't aproach this owing to briefness.

After designing our raycastable map, we'll finally create our ```Camera : IDisposable``` and set it up to render whatever it sees in the map. Cameras doesn't need to be part of the map structure; they are only objects that navigates through the map information.
Using a camera, for now, isn't as simple as it could. You need, first, to instantiate a camera with a map and the dimentions of the screen:
```
Camera camera = new Camera(
			map: map,
			width: 640,
			height: 360)
```
You'll also need to have a `PictureBox` in the screen, because that's where the image will be displayed. Then, subscribe to it's `Paint` event a method that copies the current buffer in the camera to the PictureBox:
```
myPictureBox.Paint += (anyName, anythingElse) => myPictureBox.Image = camera.BitmapCopy;
```
You'll probably want to set an ```Update()``` method that runs whenever the raycaster renders a new frame, so we also do it by eventing:
```
camera.OnRender += Update;
```
And somewhere else:
```
public void Update(Camera sender, double elapsedTime)
{
	///Do your stuff here
}
```
Lastly, just call a method that continuously renders frames and run the program:
```
camera.BeginShooting();
```

> Remarks:
> Of course, Camera needs to be disposed.
> If the PictureBox doesn't begin rendering anithyng, try forcing an update on it's background image and the Paint event will make the machine run.

## Backlocg

 - [ ] Lots of things
