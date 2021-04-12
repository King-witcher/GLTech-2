using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GLTech2
{
    internal sealed class Display : Form
    {
        Map scene;
        Camera camera;

        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Display));
            this.SuspendLayout();
            // 
            // Renderer
            // 
            this.ClientSize = new System.Drawing.Size(300, 300);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "Renderer";
            this.Text = "GL Tech 2 Renderer";
            this.ResumeLayout(false);

        }
    }
}
