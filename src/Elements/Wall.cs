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
        public Texture Material
        {
            get => unmanaged->material;
            set
            {
                unmanaged->material = value;
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

        public Wall(Vector start, Vector end, Texture material)
        {
            unmanaged = WallData.Create(start, end, material);
        }
        
        public Wall(Vector start, float angle_deg, float length, Texture material)
        {
            unmanaged = WallData.Create(start, angle_deg, length, material);
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

        public static Wall[] FromBitmap(Bitmap source, IDictionary<int, Texture> materials)
        {
            Wall[] walls = new Wall[4 * source.Width * source.Height];
            int index = 0;
            for (int srcColumn = 0; srcColumn < source.Width; srcColumn++)
            {
                for (int srcLine = 0; srcLine < source.Width; srcLine++)
                {
                    int srcArgb = source.GetPixel(srcLine, srcColumn).ToArgb();

                    if (materials.TryGetValue(srcArgb, out var material))
                    {
                        Vector vert1 = new Vector(srcLine, -srcColumn);
                        Vector vert2 = new Vector(srcLine, -srcColumn - 1);
                        Vector vert3 = new Vector(srcLine + 1, -srcColumn - 1);
                        Vector vert4 = new Vector(srcLine + 1, -srcColumn);
                        walls[index++] = new Wall(vert1, vert2, material);
                        walls[index++] = new Wall(vert2, vert3, material);
                        walls[index++] = new Wall(vert3, vert4, material);
                        walls[index++] = new Wall(vert4, vert1, material);
                    }
                }
            }
            return walls;
        }

        public static Wall[] CreateSequence(Texture material, params Vector[] verts)
        {
            if (verts == null)
                throw new ArgumentNullException("Verts cannot be null.");
            if (verts.Length <= 1)
                return new Wall[0];

            Wall[] result = new Wall[verts.Length - 1];
            Texture material_ = material;
            int walls = verts.Length - 1;

            material_.hrepeat /= walls;

            for (int i = 0; i < walls; i++)
            {
                material_.hoffset = material.hoffset + material.hrepeat * i / walls;
                result[i] = new Wall(verts[i], verts[i + 1], material_);
            }

            return result;
        }

        public static Wall[] CreatePolygon(Texture material, params Vector[] verts) //Beta
        {
            if (verts == null)
                throw new ArgumentNullException("Verts cannot be null.");
            if (verts.Length <= 1)
                return new Wall[0];

            int total_walls = verts.Length;
            Wall[] result = new Wall[total_walls];

            Texture currentMaterial = material;
            currentMaterial.hrepeat /= total_walls;

            for (int i = 0; i < total_walls - 1; i++)
            {
                currentMaterial.hoffset = material.hoffset + material.hrepeat * i / (total_walls);
                result[i] = new Wall(verts[i], verts[i + 1], currentMaterial);
            }

            currentMaterial.hoffset = material.hoffset + material.hrepeat * (total_walls - 1) / total_walls;
            result[total_walls - 1] = new Wall(verts[total_walls - 1], verts[0], currentMaterial);

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
