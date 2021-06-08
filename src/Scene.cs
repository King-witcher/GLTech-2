using System;
using System.Drawing;
using System.Collections.Generic;

namespace GLTech2
{
    /// <summary>
    /// Represents a scene, which stores a set of elements that can be or not rendered and, at least, one observer.
    /// </summary>
    public unsafe sealed partial class Scene : IDisposable
    {
        internal SceneData* unmanaged;
        private Observer activeObserver;    //Provisional
        private List<Element> elements = new List<Element>();

        /// <summary>
        /// Gets a new instance of Scene.
        /// </summary>
        /// <param name="maxWalls">Max walls that the scene can fit</param>
        /// <param name="maxSprities">Max sprities that the scene can fit</param>
        public Scene(int maxWalls = 512, int maxSprities = 512)
        {
            if (maxWalls <= 0)
                throw new ArgumentException("maxWalls cannot be < 1.");
            if (maxSprities <= 0)
                throw new ArgumentException("maxSprities cannot be < 1.");

            Texture background = new Texture((PixelBuffer)new Bitmap(1, 1));
            unmanaged = SceneData.Create(maxWalls, maxSprities, background);
        }

        /// <summary>
        /// Creates a new scene and builds a map based on a PixelBuffer. Each pixel will be converted to a block, except for black pixels.
        /// </summary>
        /// <param name="map">A PixelBuffer that represents the map</param>
        /// <param name="textures">A IDictionary that maps pixel colors from the PixelBuffer to textures</param>
        /// <param name="maxWalls">Max walls that the scene can fit</param>
        /// <param name="maxSprities">Max sprities that the scene can fit</param>
        public Scene(PixelBuffer map, IDictionary<RGB, Texture> textures, int maxWalls = 512, int maxSprities = 512) : this(maxWalls, maxSprities)
        {
            if (textures is null)
                throw new ArgumentNullException("textureMap");

            BuildFromPixelBuffer(map, textures);
        }

        /// <summary>
        /// Creates a new scene and builds a map based on a PixelBuffer. Each pixel will be converted to a block, except for black pixels.
        /// </summary>
        /// <param name="map">A PixelBuffer that represents the map</param>
        /// <param name="maxWalls">Max walls that the scene can fit</param>
        /// <param name="maxSprities">Max sprities that the scene can fit</param>
        public Scene(PixelBuffer map, int maxWalls = 5000, int maxSprities = 512) : this(maxWalls, maxSprities)
        {
            BuildFromPixelBuffer(map);
        }

        // Must be optimized.
        private void BuildFromPixelBuffer(PixelBuffer map)
        {
            Dictionary<RGB, Texture> textures = new Dictionary<RGB, Texture>();

            // Gets a new texture if exists, otherwise creates it.
            Texture getTexture(RGB rgb)
            {
                if (textures.TryGetValue(rgb, out Texture texture))
                    return texture;
                else
                {
                    PixelBuffer buffer = new PixelBuffer(1, 1);     // Must be added to disposables
                    buffer[0, 0] = rgb;
                    Texture tex = new Texture(buffer);
                    textures[rgb] = tex;
                    return tex;
                }
            }

            for (int column = 0; column < map.Width; column++)
            {
                for (int line = 0; line < map.Width; line++)
                {
                    // Checks for transparency.
                    if (map[column, line] == (RGB)(0, 0, 0))
                        continue;

                    Texture texture = getTexture(map[column, line]);

                    Vector vert1 = (line, -column);
                    Vector vert2 = (line, -column - 1);
                    Vector vert3 = (line + 1, -column - 1);
                    Vector vert4 = (line + 1, -column);

                    AddElement(new Wall(vert1, vert2, texture));
                    AddElement(new Wall(vert2, vert3, texture));
                    AddElement(new Wall(vert3, vert4, texture));
                    AddElement(new Wall(vert4, vert1, texture));
                }
            }
        }

        // Must be optimized.
        private void BuildFromPixelBuffer(PixelBuffer map, IDictionary<RGB, Texture> textures)
        {
            for (int column = 0; column < map.Width; column++)
            {
                for (int line = 0; line < map.Width; line++)
                {
                    if (textures.TryGetValue(map[column, line], out Texture texture))
                    {
                        Vector vert1 = (line, -column);
                        Vector vert2 = (line, -column - 1);
                        Vector vert3 = (line + 1, -column - 1);
                        Vector vert4 = (line + 1, -column);

                        AddElement(new Wall(vert1, vert2, texture));
                        AddElement(new Wall(vert2, vert3, texture));
                        AddElement(new Wall(vert3, vert4, texture));
                        AddElement(new Wall(vert4, vert1, texture));
                    }
                }
            }
        }

        /// <summary>
        /// Gets a new instance of Scene.
        /// </summary>
        /// <param name="background">Background material rendered behind everything</param>
        /// <param name="maxWalls">Max walls that the scene can fit</param>
        /// <param name="maxSprities">Max sprities that the scene can fit</param>
        public Scene(Texture background, int maxWalls = 512, int maxSprities = 512) =>
            unmanaged = SceneData.Create(maxWalls, maxSprities, background);


        /// <summary>
        /// Gets and sets the current observer from where the scene will be rendered.
        /// </summary>
        public Observer ActiveObserver
        {
            get => activeObserver;
            set
            {
                if (value is null || value.scene == null)   // null pointer
                {
                    activeObserver = value;
                    unmanaged->activeObserver = value.unmanaged;
                }
                else
                    Debug.InternalLog("Scene", "Can\'t set a camera that is not in this scene.", Debug.Options.Error);
            }
        }

        /// <summary>
        /// Gets how many walls the scene can fit.
        /// </summary>
        public int MaxWalls => unmanaged->wall_max;

        /// <summary>
        /// Gets how many walls the scene fits.
        /// </summary>
        public int WallCount => unmanaged->wall_count;

        /// <summary>
        /// Gets and sets the background texture of the Scene.
        /// </summary>
        public Texture Background
		{
            get => unmanaged->background;
            set => unmanaged->background = value;
        }

        /// <summary>
        ///     Add a new element and every child it has to the scene.
        /// </summary>
        /// <remarks>
        ///     <para>
        ///         Every element can only be added once to a scene. Trying to add an element twice or an element that is already bound to another scene will generate command line warning.
        ///     </para>
        ///     <para>
        ///         This method was not yet fully tested!
        ///     </para>
        /// </remarks>
        /// <param name="element">An element to be added</param>
        public void AddElement(Element element)
        {
            if (element is null)
                throw new ArgumentNullException("Cannot add null elements.");

            if (element.scene != null && Debug.DebugWarnings)
            {
                Console.WriteLine($"\"{element}\" is already bound to scene {element.scene}. Adding operation will be aborted.");
                return;
            }

            if (element.Parent != null && element.Parent.scene != this)       // Must be tested
            {
                element.Parent = null;
            }

            if (element is Wall)
                UnmanagedAddWall(element as Wall);
            else if (element is Sprite)
                UnmanagedAddSprite(element as Sprite);
            else if (element is Observer)
                UnmanagedAddObserver(element as Observer);

            elements.Add(element);
            element.scene = this;

            foreach (var item in element.childs)
                AddElement(item);
        }

        /// <summary>
        ///     Adds a whole set of elements.
        /// </summary>
        /// <param name="elements">Set of elements</param>
        public void AddElements(IEnumerable<Element> elements)
        {
            foreach (Element item in elements)
            {
                if (item is null)
                    break;

                AddElement(item);
            }
        }

        /// <summary>
        ///     Add an array of elements via params.
        /// </summary>
        /// <param name="elements">Array of elements</param>
        public void AddElements(params Element[] elements)
        {
            AddElements((IEnumerable<Element>) elements);
        }

        private void UnmanagedAddWall(Wall w)
        {
            if (unmanaged->wall_count >= unmanaged->wall_max)
                throw new IndexOutOfRangeException("Wall limit reached.");
            unmanaged->Add(w.unmanaged);
        }
        private void UnmanagedAddSprite(Sprite s) => throw new NotImplementedException();

        private void UnmanagedAddObserver(Observer p)
        {
            ActiveObserver = p;
        }

        /// <summary>
        ///     Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            foreach(Element item in elements)
                item.Dispose();

            SceneData.Delete(unmanaged);
            unmanaged = null;
            activeObserver = null;

            elements.Clear();
        }

        internal void InvokeStart()
        {
            foreach (var element in elements)
                element.InvokeStart();
        }

        internal void InvokeUpdate()
        {
            foreach (var element in elements)
                element.InvokeUpdate();
        }
    }
}
