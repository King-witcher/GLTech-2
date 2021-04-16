using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GLTech2.StandardObjects
{
    internal sealed class RegularPolygon : Element
    {
        public RegularPolygon(Vector position, int edges, float radius, Material material)
        {
            Vector[] verts = Vector.GetPolygon(position, radius, 64);
            Wall[] walls = Wall.CreatePolygon(material, verts);
            foreach (Wall wall in walls)
                wall.Parent = this;

            UpdateRelative();
        }

        private protected override Vector AbsolutePosition { get; set; }
        private protected override Vector AbsoluteNormal { get; set; }
    }
}
