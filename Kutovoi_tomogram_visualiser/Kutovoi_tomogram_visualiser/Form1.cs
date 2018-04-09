using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Kutovoi_tomogram_visualiser
{
    public partial class Form1 : Form
    {
        Bin binaries = new Bin();
        View view = new View();
        bool loaded = false;
        bool needReload = false;
        int currentLayer = 0;
        int FrameCount;
        DateTime NextFpsUpdate = DateTime.Now.AddSeconds(1);

        void displayFPS()
        {
            if(DateTime.Now >= NextFpsUpdate)
            {
                this.Text = String.Format("CT Visualiser (fps={0})", FrameCount);
                NextFpsUpdate = DateTime.Now.AddSeconds(1);
                FrameCount = 0;
            }
            FrameCount++;
        }
        public Form1()
        {
            InitializeComponent();
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            
            if(dialog.ShowDialog() == DialogResult.OK)
            {
                string str = dialog.FileName;
                binaries.readBin(str);
                view.SetupViewZ(glControl1.Width, glControl1.Height);
                loaded = true;
                glControl1.Invalidate();
                trackBar1.Maximum = Bin.Y - 1;
            }
        }

        private void glControl1_Paint(object sender, EventArgs e)
        {
            if(loaded)
            {
                if (radioButton1.Checked)
                    view.DrawQuadsZ(currentLayer);
                if (radioButton2.Checked)
                {
                    if (needReload)
                    {
                        view.generateTextureImage(currentLayer);
                        view.Load2DTexture();
                        needReload = false;
                    }
                    view.DrawTexture();
                }
                glControl1.SwapBuffers();
            }
        }

        private void trackBar1_Scroll(object sender, EventArgs e)
        {
            currentLayer = trackBar1.Value;
            if(radioButton2.Checked)needReload = true;
            glControl1_Paint(sender, e);
        }

        void Application_Idle(object sender, EventArgs e)
        {
            while (glControl1.IsIdle)
            {
                displayFPS();
                glControl1.Invalidate();
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            Application.Idle += Application_Idle;
        }

        private void trackBar2_Scroll(object sender, EventArgs e)
        {
            view.min = trackBar2.Value;
        }

        private void trackBar3_Scroll(object sender, EventArgs e)
        {
            view.width = trackBar3.Value;
        }
    }
}
