//See Vector.cs before
//See Material.cs before
    //See Texture32.cs before

#define DEVELOPMENT

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security;

namespace GLTech2
{
    public unsafe class Wall : Element
    {
        internal WallData* walldata;


        public Vector StartPoint
        {
            get => walldata->geom_start;
            set => walldata->geom_start = value;
        }

        public Vector EndPoint
        {
            get => walldata->geom_start + walldata->geom_direction;
            set => walldata->geom_direction = value - walldata->geom_start;
        }

        public float Length
        {
            get => walldata->geom_direction.Module;
            set => walldata->geom_direction *= value / walldata->geom_direction.Module;
        }

        public Material Material
        {
            set
            {
                walldata->material = value;
            }
        }

        public override Vector Position
        {
            get => StartPoint;
            set => StartPoint = value;
        }

        public override float Rotation
        {
            get => walldata->geom_direction.Angle;
            set
            {
                walldata->geom_direction = walldata->geom_direction.Module * new Vector(value);
            }
        }



        public Wall(Vector start, Vector end, Material material)
        {
            walldata = WallData.Alloc(start, end, material);
        }
        
        public Wall(Vector start, float angle_deg, float length, Material material)
        {
            walldata = WallData.Alloc(start, angle_deg, length, material);
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
                        Texture32 blockTexture;
                        using (Bitmap blockBitmap = new Bitmap(1, 1))
                        {
                            blockBitmap.SetPixel(0, 0, source.GetPixel(line, column));
                            blockTexture = new Texture32(blockBitmap);
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

        public static Wall[] FromBitmap(Bitmap source, IDictionary<int, Material> materials)
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

        public static Wall[] CreateSequence(Material material, params Vector[] verts)
        {
            if (verts == null)
                throw new ArgumentNullException("Verts cannot be null.");
            if (verts.Length <= 1)
                return new Wall[0];

            Wall[] result = new Wall[verts.Length - 1];
            Material material_ = material;
            int walls = verts.Length - 1;

            material_.hrepeat /= walls;

            for (int i = 0; i < walls; i++)
            {
                material_.hoffset = material.hoffset + material.hrepeat * i / walls;
                result[i] = new Wall(verts[i], verts[i + 1], material_);
            }

            return result;
        }

        public static Wall[] CreatePolygon(Material material, params Vector[] verts) //Beta
        {
            if (verts == null)
                throw new ArgumentNullException("Verts cannot be null.");
            if (verts.Length <= 1)
                return new Wall[0];

            int total_walls = verts.Length;
            Wall[] result = new Wall[total_walls];

            Material currentMaterial = material;
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
            Marshal.FreeHGlobal((IntPtr) walldata);
        }
    }
}
