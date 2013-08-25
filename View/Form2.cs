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
    public partial class Form2 : Form
    {
        private Terrain terrain;
        Pen pen;
        private Pen pen2;

        public Terrain Terrain
        {
            get { return terrain; }
            set
            {
                terrain = value;
                UpdateModel();
            }
        }

        public Form2()
        {
            InitializeComponent();
        }

        public void UpdateModel()
        {
            panel1.Invalidate();
        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {
            
            Vector2 a = new Vector2(10, 20);
            Vector2 b = new Vector2(150, 10);
            Vector2 c = new Vector2(150, 150);

            SortTriangleByY(ref a, ref b, ref c);
            
            Vector2 ac = c - a, ab = b - a, bc = b - c;
            for (int i = 1; i <= ac.Y; i++)
            {
                float y = i + a.Y;
                float x1 = ((ac.X * i) / ac.Y) + a.X;
                float x2 = (y < b.Y) ? ((ab.X * i) / ab.Y) + a.X : ((bc.X * (i + a.Y - b.Y)) / bc.Y) + b.X;
                e.Graphics.DrawLine(pen, x1, i + a.Y, x2, i + a.Y);
                //if (y < b.Y)
                //    e.Graphics.DrawLine(pen, x1, i + a.Y, x2, i + a.Y);
                //else
                //{
                //    float x3 = ((bc.X * (i + a.Y - b.Y)) / bc.Y) + b.X;
                //    e.Graphics.DrawLine(pen, x1, i + a.Y, x3, i + a.Y);
                //}
            }

            e.Graphics.FillEllipse(pen2.Brush, a.X - 2, a.Y - 2, 4, 4);
            e.Graphics.DrawString("A", DefaultFont, pen2.Brush, a.X, a.Y - 10);
            e.Graphics.FillEllipse(pen2.Brush, b.X - 2, b.Y - 2, 4, 4);
            e.Graphics.DrawString("B", DefaultFont, pen2.Brush, b.X, b.Y - 10);
            e.Graphics.FillEllipse(pen2.Brush, c.X - 2, c.Y - 2, 4, 4);
            e.Graphics.DrawString("C", DefaultFont, pen2.Brush, c.X, c.Y - 10);
        }

        public void SortTriangleByY(ref Vector2 p, ref Vector2 q, ref Vector2 r)
        {
            Vector2 temp;

            if (q.Y < p.Y)
            {
                temp = p;
                if (r.Y < q.Y) { p = r; r = temp; }
                else
                {
                    if (r.Y < temp.Y) { p = q; q = r; r = temp; }
                    else              { p = q; q = temp; }
                }
            }
            else
            {
                if (r.Y < q.Y)
                {
                    temp = r; r = q;
                    if (temp.Y < p.Y) { q = p; p = temp; }
                    else              { q = temp; }
                }
            }
        }

        private void Form2_Load(object sender, EventArgs e)
        {
            pen = new Pen(System.Drawing.Color.Blue);
            pen2 = new Pen(System.Drawing.Color.Red);
        }
    }
}
