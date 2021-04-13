using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GLTech2
{
    public class CameraAdapter : Element
    {
        public override Vector Position { get; set; }
        public override Vector Rotation { get; set; }
        public float FieldOfView { get; set; }
        public void SetDefault() =>
            Renderer.UseCamera(this);
    }
}
