# GL Tech 2

GL Tech 2 is a 3D engine designed with it's interface entirely in C# and parts of the rendering engine made in C++ that renders that allows you to render 2D maps as if they were 3D due to the _ray casting_ technique, without using GPU. The engine was entirely designed by Giuseppe Lanna, but still in development stage.

**How to build**

If you already have Visual Studio Community 2019 installed, just open `CLICKME.sln` and press F5.

## Building and rendering your first scene

We start by importing textures from Bitmaps. Each texture represents a single 32-bits-per-pixel image that can be used by many objects, wich can resize and offset it any different ways you want. There are two different ways that result in the same thing:

	// using System.Drawing;
	// using GLTech2; //OBVIOUSLY
	
	Bitmap bmp = new Bitmap($"filename.png");
	Texture texture = new Texture(bmp); // Via constructor
	Texture texture2 = bmp; 			// Via implicit cast

After that, we need to create our Materials, which represents a flyweight to the texture with a specific repeat pattern and offset of it:

	Material material = new Material(
		texture: texture,
		hoffset: 0f,		// How much the texture should be shift relative to
							// its full size
		hrepeat: 2f);		// How much the texture should be repeated
.
Finally, we are able to create a scene, which requires a background material and allows you to add, by default, up to 512 **walls**, 512 **sprities** how many **observers** and **empties** you want. We'll talk about these later.

	Scene scene = new Scene(material);

Lets create elements to put in the scene.

	// using GLTech2.PrefabElements;
	
	RegularPolygon poly = new RegularPolygon(
		position: Vector.Forward	// <0f, 1f>
		edges: 4, 				
		radius: .707f,
		material: material);
	
	// The observer represents a point of view from where the renderer
	// will render everything. Every scene MUST have at least one point of view.
	Observer observer = new Observer(Vector.Origin);
	
	// You can use as many parameters as you want. Java doesn't allow it =)
	scene.AddElements(poly, observer);

Like in Unity3D, you can add custom behaviours to the scene elements. For didatic purposes, we'll use only the prefab behaviours in this tutorial.

	// using GLTech2.StandardBehaviours

	// Makes the element to rotate infinitelly.
	poly.AddBehaviour<Rotate>();
	
	// Allows the user to rotate the element via mouse.
	Behaviour mouseLook = new MouseLook(3f);
	
	// Allows the user to move the element via Keyboard.
	Behaviour controller = new NoclipController();
	controller.MaxSpeed = 2.5f;

	// Add them to the observer.
	observer.AddBehaviours(controller, mouselook);

Still, you can add post processing effects to the Renderer class. We'll add a FXAA antialiasing to make the image a bit smoother.

	// using GLTech2.PostProcessing;
	
	// Renderer resolution must be setup first.
	Renderer.FullScreen = true;
	
	Effect fxaa= new FXAA(Renderer.CustomWidth, Renderer.CustomHeight);
	Renderer.AddEffect(fxaa);

And just do what we wanted since the beginning:

	Renderer.Run(scene); // And have fun

After ending that, remember to release every unmanaged resource.

		scene.Dispose();	// Will dispose everything inside it.
		texture.Dispose();
		texture2.Dispose();
		fxaa.Dispose();
		
		// Behaviours desn't use unmanaged resources.
> Written with [StackEdit](https://stackedit.io/).