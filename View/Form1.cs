using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using EditorModel;

namespace View
{
    public partial class Form1 : Form
    {
        public MainUserControl MainUserControl1
        {
            get
            {
                return mainUserControl1;
            }
        }

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Form3 form = new Form3();

            form.terrain = mainUserControl1.Editor.Terrain;
            form.terrain.TerrainIndexer.Attach(form);
            form.Show();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (mainUserControl1.Editor.openFileDialog1.ShowDialog() != DialogResult.OK)
            {
                return;
            }
            mainUserControl1.Editor.ImportHeightmap(mainUserControl1.Editor.openFileDialog1.FileName);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            mainUserControl1.Editor.Camera.Zoom += 50;
            mainUserControl1.Editor.Camera.Notify();
        }
    }
}
