using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GLTech2.Properties;

namespace GLTech2
{
    static class Program
    {
        static void Main()
        {
            var bg = new Material(new GLBitmap(new System.Drawing.Bitmap(1, 1)));
            Scene scene = new Scene(bg);
            Renderer.CppRendering = true;
            Renderer.Run(scene);
        }
    }
}
