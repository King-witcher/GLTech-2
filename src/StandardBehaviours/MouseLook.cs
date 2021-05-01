using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;

namespace GLTech2.StandardBehaviours
{
    public sealed class MouseLook : Behaviour
    {
        public bool Enabled { get; set; } = true;
        public float Sensitivity { get; set; } = 5;

        Point screenCenter;
        int CenterH;

        void Start()
        {
            screenCenter = new Point(Screen.PrimaryScreen.Bounds.Width / 2, Screen.PrimaryScreen.Bounds.Height / 2);
            CenterH = Screen.PrimaryScreen.Bounds.Width / 2;
        }

        void Update()
        {
            if (Enabled)
            {
                int delta = Cursor.Position.X - CenterH;
                Cursor.Position = screenCenter;
                element.Rotate(delta * 0.022f * Sensitivity);
            }
        }
    }
}
