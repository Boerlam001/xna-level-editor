using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using EditorModel;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace View
{
    public partial class Form3 : Form, IObserver
    {
        public Terrain terrain;
        private Pen redPen;
        private Pen bluePen;

        public Form3()
        {
            InitializeComponent();
        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {
            if (terrain == null)
                return;

            try
            {
                int red = -1;
                for (int i = 0; i < terrain.Indices.Length;)
                {
                    Vector3 v1 = terrain.TerrainIndexer.ScreenLocations[terrain.Indices[i++]],
                            v2 = terrain.TerrainIndexer.ScreenLocations[terrain.Indices[i++]],
                            v3 = terrain.TerrainIndexer.ScreenLocations[terrain.Indices[i++]];
                    if (red == 1)
                    {
                        e.Graphics.DrawLine(redPen, v1.X, v1.Y, v2.X, v2.Y);
                        e.Graphics.DrawLine(redPen, v3.X, v3.Y, v2.X, v2.Y);
                    }
                    else
                    {
                        e.Graphics.DrawLine(bluePen, v1.X, v1.Y, v2.X, v2.Y);
                        e.Graphics.DrawLine(bluePen, v3.X, v3.Y, v2.X, v2.Y);
                    }
                    red *= -1;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "\r\n" + ex.StackTrace);
            }
        }

        private void panel2_Paint(object sender, PaintEventArgs e)
        {
            if (terrain == null)
                return;

            try
            {
                for (int x = 0; x < terrain.TerrainIndexer.Indices.GetLength(0); x++)
                {
                    for (int y = 0; y < terrain.TerrainIndexer.Indices.GetLength(1); y++)
                    {
                        if (terrain.TerrainIndexer.Indices[x, y] != -1)
                            e.Graphics.FillRectangle(Brushes.Red, x, y, 1, 1);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "\r\n" + ex.StackTrace);
            }
        }

        private void Form3_Load(object sender, EventArgs e)
        {
            redPen = new Pen(Brushes.Red);
            bluePen = new Pen(Brushes.Blue);
        }

        public void UpdateObserver()
        {
            panel1.Invalidate();
            panel2.Invalidate();
        }

        private void Form3_FormClosing(object sender, FormClosingEventArgs e)
        {
            terrain.TerrainIndexer.Detach(this);
        }
    }
}
