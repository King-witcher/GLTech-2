using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GLTech2.PrefabElements
{
    public sealed class RegularPolygon : Element
    {
        public RegularPolygon(Vector position, int edges, float radius, Material material)
        {
            if (edges <= 2)
                throw new ArgumentException("\"edges\" must be greater than 2.");
            if (radius == 0)
                throw new ArgumentException("\"radius\" cannot be zero.");

            AbsolutePosition = position;
            AbsoluteNormal = Vector.Unit;

            Vector[] verts = Vector.GetPolygon(position, radius, edges);
            Wall[] walls = Wall.CreatePolygon(material, verts);
            foreach (Wall wall in walls)
                wall.Parent = this;

        }

        private protected override Vector AbsolutePosition { get; set; }
        private protected override Vector AbsoluteNormal { get; set; }
    }
}
