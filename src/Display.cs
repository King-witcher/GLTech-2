using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GLTech2
{
    internal class Display : PictureBox
    {
        public Display(int width, int height)
        {
            this.Width = width;
            this.Height = height;
        }
    }
}
