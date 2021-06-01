using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;

namespace GLTech2
{
    internal sealed class Display : Form, IDisposable
    {
        private PictureBox pictureBox;  // Will only be released by GC
		private Label label1;
		private Label label2;
		private Bitmap source;

        internal Display(bool fullscreen, int width, int height, Bitmap videoSource)
        {
            InitializeComponent();
            pictureBox.Click += OnFocus;
            LostFocus += OnLoseFocus;

            source = videoSource;

            if (fullscreen)
            {
                WindowState = FormWindowState.Maximized;
                FormBorderStyle = FormBorderStyle.None;
            }
            else
                SetSize(width, height);

            // This create an infinite loop that keeps updating the image on the screen.
            pictureBox.Paint += RePaint;
            OnFocus(null, null);
        }

        internal void RePaint(object _ = null, EventArgs __ = null)
        {
            pictureBox.Image = source;
        }

        private void InitializeComponent()
        {
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Display));
			this.pictureBox = new System.Windows.Forms.PictureBox();
			this.label1 = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			((System.ComponentModel.ISupportInitialize)(this.pictureBox)).BeginInit();
			this.SuspendLayout();
			// 
			// pictureBox
			// 
			this.pictureBox.Dock = System.Windows.Forms.DockStyle.Fill;
			this.pictureBox.Location = new System.Drawing.Point(0, 0);
			this.pictureBox.Name = "pictureBox";
			this.pictureBox.Size = new System.Drawing.Size(640, 360);
			this.pictureBox.TabIndex = 0;
			this.pictureBox.TabStop = false;
			// 
			// label1
			// 
			this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.label1.AutoSize = true;
			this.label1.Font = new System.Drawing.Font("Bahnschrift Condensed", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label1.ForeColor = System.Drawing.SystemColors.ButtonFace;
			this.label1.Location = new System.Drawing.Point(357, 28);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(271, 19);
			this.label1.TabIndex = 1;
			this.label1.Text = "Algumas texturas podem conter direitos autorais.";
			this.label1.TextAlign = System.Drawing.ContentAlignment.BottomRight;
			// 
			// label2
			// 
			this.label2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.label2.AutoSize = true;
			this.label2.Font = new System.Drawing.Font("Bahnschrift Condensed", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label2.ForeColor = System.Drawing.SystemColors.ButtonFace;
			this.label2.Location = new System.Drawing.Point(511, 9);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(117, 19);
			this.label2.TabIndex = 2;
			this.label2.Text = "GL Tech 2, build 0106";
			this.label2.TextAlign = System.Drawing.ContentAlignment.BottomRight;
			// 
			// Display
			// 
			this.AccessibleRole = System.Windows.Forms.AccessibleRole.WhiteSpace;
			this.BackColor = System.Drawing.Color.Black;
			this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
			this.ClientSize = new System.Drawing.Size(640, 360);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.pictureBox);
			this.Cursor = System.Windows.Forms.Cursors.Arrow;
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.MaximizeBox = false;
			this.Name = "Display";
			this.Text = "GL Tech 2 Screen";
			this.Load += new System.EventHandler(this.Display_Load);
			this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.Display_KeyDown);
			this.KeyUp += new System.Windows.Forms.KeyEventHandler(this.Display_KeyUp);
			((System.ComponentModel.ISupportInitialize)(this.pictureBox)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();

        }

        public void SetSize(int width, int height)
        {
            pictureBox.Size = new System.Drawing.Size(width, height);
            this.ClientSize = new System.Drawing.Size(width, height);
        }

        private void Display_KeyDown(object sender, KeyEventArgs e)
        {
            Keyboard.KeyDown((Key)e.KeyCode);
        }

        private void Display_KeyUp(object sender, KeyEventArgs e)
        {
            Keyboard.KeyUp((Key)e.KeyCode);
        }

        private void Display_Load(object sender, EventArgs e)
        {
        }

        private void OnFocus(object sender, EventArgs e)
        {
            if (Renderer.CaptureMouse)
            {
                Mouse.Enable();
            }
        }

        private void OnLoseFocus(object _, EventArgs __)
        {
            if (Renderer.CaptureMouse)
            {
                Mouse.Disable();
            }
        }
    }
}
