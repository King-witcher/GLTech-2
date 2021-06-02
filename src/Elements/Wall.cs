using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace GLTech2
{
    /// <summary>
    /// Represents a wall that can be rendered on the screen.
    /// </summary>
    public unsafe class Wall : Element
    {
        internal WallData* unmanaged;

        /// <summary>
        /// Gets and sets the starting point of the wall.
        /// </summary>
        public Vector StartPoint
        {
            get => unmanaged->geom_start;
            set => unmanaged->geom_start = value;
        }

        /// <summary>
        /// Gets and sets the ending point of the wall.
        /// </summary>
        public Vector EndPoint
        {
            get => unmanaged->geom_start + unmanaged->geom_direction;
            set => unmanaged->geom_direction = value - unmanaged->geom_start;
        }

        /// <summary>
        /// Gets and sets the length of the wall.
        /// </summary>
        public float Length
        {
            get => unmanaged->geom_direction.Module;
            set => unmanaged->geom_direction *= value / unmanaged->geom_direction.Module;
        }

        /// <summary>
        /// Gets and sets the material of the wall.
        /// </summary>
        public Texture Texture
        {
            get => unmanaged->texture;
            set
            {
                unmanaged->texture = value;
            }
        }

        private protected override Vector AbsolutePosition
        {
            get => StartPoint;
            set => StartPoint = value;
        }

        private protected override Vector AbsoluteNormal
        {
            get => unmanaged->geom_direction;
            set
            {
                unmanaged->geom_direction = value;
            }
        }

        public Wall(Vector start, Vector end, Texture texture)
        {
            unmanaged = WallData.Create(start, end, texture);
        }
        
        public Wall(Vector start, float angle_deg, float length, Texture texture)
        {
            unmanaged = WallData.Create(start, angle_deg, length, texture);
        }

        public static Wall[] FromBitmap(Bitmap source, params Color[] ignoreList)
        {
            if (ignoreList.Length == 0)
                throw new ArgumentException("It doesn't make sense to create a set of walls from a bitmap without having an ignore list. You're probably missing it.");

            Wall[] walls = new Wall[4 * source.Width * source.Height];
            int[] ignoreArgb = new int[ignoreList.Length];
            int index = 0;

            //Caches the argb of every color.
            foreach (Color color in ignoreList)
                ignoreArgb[index++] = color.ToArgb();

            index = 0;
            for (int column = 0; column < source.Width; column++)
            {
                for (int line = 0; line < source.Width; line++)
                {
                    int srcArgb = source.GetPixel(line, column).ToArgb();

                    if (ignoreArgb.Contains(srcArgb)) continue;
                    else
                    {
                        PixelBuffer blockTexture;
                        using (Bitmap blockBitmap = new Bitmap(1, 1))
                        {
                            blockBitmap.SetPixel(0, 0, source.GetPixel(line, column));
                            blockTexture = new PixelBuffer(blockBitmap);
                        }
                        Vector vert1 = new Vector(line, -column);
                        Vector vert2 = new Vector(line, -column - 1);
                        Vector vert3 = new Vector(line + 1, -column - 1);
                        Vector vert4 = new Vector(line + 1, -column);
                        walls[index++] = new Wall(vert1, vert2, blockTexture);
                        walls[index++] = new Wall(vert2, vert3, blockTexture);
                        walls[index++] = new Wall(vert3, vert4, blockTexture);
                        walls[index++] = new Wall(vert4, vert1, blockTexture);
                    }
                }
            }
            return walls;
        }

        public static Wall[] FromBitmap(Bitmap source, IDictionary<int, Texture> textures)
        {
            Wall[] walls = new Wall[4 * source.Width * source.Height];
            int index = 0;
            for (int srcColumn = 0; srcColumn < source.Width; srcColumn++)
            {
                for (int srcLine = 0; srcLine < source.Width; srcLine++)
                {
                    int srcArgb = source.GetPixel(srcLine, srcColumn).ToArgb();

                    if (textures.TryGetValue(srcArgb, out var texure))
                    {
                        Vector vert1 = (srcLine, -srcColumn);
                        Vector vert2 = (srcLine, -srcColumn - 1);
                        Vector vert3 = (srcLine + 1, -srcColumn - 1);
                        Vector vert4 = (srcLine + 1, -srcColumn);
                        walls[index++] = new Wall(vert1, vert2, texure);
                        walls[index++] = new Wall(vert2, vert3, texure);
                        walls[index++] = new Wall(vert3, vert4, texure);
                        walls[index++] = new Wall(vert4, vert1, texure);
                    }
                }
            }
            return walls;
        }

        /// <summary>
        /// Gets a new set of walls given a pixelbuffer. I'll explain how it works later.
        /// </summary>
        /// <param name="source">The pixelbuffer that contains information about the map</param>
        /// <param name="textures">The dicionary that maps colors to textures</param>
        /// <returns>The resulting set of walls</returns>
        public static Wall[] FromPixelBuffer(PixelBuffer source, IDictionary<RGB, Texture> textures)
        {
            Wall[] walls = new Wall[4 * source.Width * source.Height];
            int index = 0;

            for (int column = 0; column < source.Width; column++)
                for (int line = 0; line < source.Width; line++)
                    if (textures.TryGetValue(source[column, line], out var texure))
                    {
                        Vector vert1 = (line, -column);
                        Vector vert2 = (line, -column - 1);
                        Vector vert3 = (line + 1, -column - 1);
                        Vector vert4 = (line + 1, -column);
                        walls[index++] = new Wall(vert1, vert2, texure);
                        walls[index++] = new Wall(vert2, vert3, texure);
                        walls[index++] = new Wall(vert3, vert4, texure);
                        walls[index++] = new Wall(vert4, vert1, texure);
                    }

            return walls;
        }

        public static Wall[] CreateSequence(Texture textures, params Vector[] verts)
        {
            if (verts == null)
                throw new ArgumentNullException("Verts cannot be null.");
            if (verts.Length <= 1)
                return new Wall[0];

            Wall[] result = new Wall[verts.Length - 1];
            Texture texture_ = textures;
            int walls = verts.Length - 1;

            texture_.hrepeat /= walls;

            for (int i = 0; i < walls; i++)
            {
                texture_.hoffset = textures.hoffset + textures.hrepeat * i / walls;
                result[i] = new Wall(verts[i], verts[i + 1], texture_);
            }

            return result;
        }

        public static Wall[] CreatePolygon(Texture texture, params Vector[] verts) //Beta
        {
            if (verts == null)
                throw new ArgumentNullException("Verts cannot be null.");
            if (verts.Length <= 1)
                return new Wall[0];

            int total_walls = verts.Length;
            Wall[] result = new Wall[total_walls];

            Texture currentTexture = texture;
            currentTexture.hrepeat /= total_walls;

            for (int i = 0; i < total_walls - 1; i++)
            {
                currentTexture.hoffset = texture.hoffset + texture.hrepeat * i / (total_walls);
                result[i] = new Wall(verts[i], verts[i + 1], currentTexture);
            }

            currentTexture.hoffset = texture.hoffset + texture.hrepeat * (total_walls - 1) / total_walls;
            result[total_walls - 1] = new Wall(verts[total_walls - 1], verts[0], currentTexture);

            return result;
        }

        public override void Dispose()
        {
            WallData.Delete(unmanaged);
            unmanaged = null;
        }

        public override string ToString()
        {
            return $"|{ StartPoint } -- { EndPoint }| ";
        }
    }
}
