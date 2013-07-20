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

namespace View
{
    public partial class TryVector2 : Form, IObserver
    {
        public Form1 form1;

        BoundingBoxBuffer selectedBoundingBox;
        Vector2 point0, pointX, pointY, pointZ;
        private int min;
        private float berpotonganX, berpotonganY, mouseX, mouseY;
        private bool dragStart;

        public BoundingBoxBuffer SelectedBoundingBox
        {
            get { return selectedBoundingBox; }
            set { selectedBoundingBox = value; }
        }

        public TryVector2()
        {
            InitializeComponent();
        }

        void IObserver.Update()
        {
            Editor editor = form1.MainUserControl1.Editor1;

            Microsoft.Xna.Framework.Vector3 x1 = editor.GraphicsDevice.Viewport.Project(selectedBoundingBox.Parent.AxisLines.AxisVertices[0].Position, editor.Camera.Projection, editor.Camera.World, Microsoft.Xna.Framework.Matrix.Identity);
            Microsoft.Xna.Framework.Vector3 x2 = editor.GraphicsDevice.Viewport.Project(selectedBoundingBox.Parent.AxisLines.AxisVertices[1].Position, editor.Camera.Projection, editor.Camera.World, Microsoft.Xna.Framework.Matrix.Identity);
            Microsoft.Xna.Framework.Vector3 y2 = editor.GraphicsDevice.Viewport.Project(selectedBoundingBox.Parent.AxisLines.AxisVertices[3].Position, editor.Camera.Projection, editor.Camera.World, Microsoft.Xna.Framework.Matrix.Identity);
            Microsoft.Xna.Framework.Vector3 z2 = editor.GraphicsDevice.Viewport.Project(selectedBoundingBox.Parent.AxisLines.AxisVertices[5].Position, editor.Camera.Projection, editor.Camera.World, Microsoft.Xna.Framework.Matrix.Identity);

            point0.X = x1.X;
            point0.Y = x1.Y;
            pointX.X = x2.X;
            pointX.Y = x2.Y;
            pointY.X = y2.X;
            pointY.Y = y2.Y;
            pointZ.X = z2.X;
            pointZ.Y = z2.Y;

            panel1.Invalidate();
        }

        private void TryVector2_Load(object sender, EventArgs e)
        {
            point0 = new Vector2();
            pointX = new Vector2();
            pointY = new Vector2();
            pointZ = new Vector2();
            min = -1;
            berpotonganX = berpotonganY = -1;
            mouseX = mouseY = -1;
            dragStart = false;

            selectedBoundingBox.Notify();
        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {
            Pen pen = new Pen(System.Drawing.Color.Black);

            e.Graphics.DrawString("0", Font, pen.Brush, point0.X - 5, point0.Y - 10);
            e.Graphics.FillEllipse(pen.Brush, point0.X - 1, point0.Y - 1, 2, 2);

            e.Graphics.DrawString("x", Font, pen.Brush, pointX.X - 5, pointX.Y - 10);
            e.Graphics.FillEllipse(pen.Brush, pointX.X - 1, pointX.Y - 1, 2, 2);

            e.Graphics.DrawString("y", Font, pen.Brush, pointY.X - 5, pointY.Y - 10);
            e.Graphics.FillEllipse(pen.Brush, pointY.X - 1, pointY.Y - 1, 2, 2);

            e.Graphics.DrawString("z", Font, pen.Brush, pointZ.X - 5, pointZ.Y - 10);
            e.Graphics.FillEllipse(pen.Brush, pointZ.X - 1, pointZ.Y - 1, 2, 2);

            if (berpotonganX != -1)
                e.Graphics.DrawLine(pen, mouseX, mouseY, berpotonganX, berpotonganY);

            switch (min)
            {
                case 1:
                    e.Graphics.DrawLine(pen, point0.X, point0.Y, pointX.X, pointX.Y);
                    break;
                case 2:
                    e.Graphics.DrawLine(pen, point0.X, point0.Y, pointY.X, pointY.Y);
                    break;
                case 3:
                    e.Graphics.DrawLine(pen, point0.X, point0.Y, pointZ.X, pointZ.Y);
                    break;
            }
        }

        private void panel1_MouseMove(object sender, MouseEventArgs e)
        {
            DateTime start = DateTime.Now;
            mouseX = e.X;
            mouseY = e.Y;
            
            Vector2 axis = pointX - point0;
            Vector2 mouseVector = new Vector2(e.X, e.Y);
            Vector2 mouseVector0 = mouseVector - point0;
            Vector2 projXdirection = (Vector2.Dot(axis, mouseVector0) / axis.LengthSquared()) * axis;
            float distX = (Vector2.Distance(point0 + projXdirection, pointX) > axis.Length()) ? float.MaxValue : Vector2.Distance(projXdirection, mouseVector0);

            axis = pointY - point0;
            Vector2 projYdirection = (Vector2.Dot(axis, mouseVector0) / axis.LengthSquared()) * axis;
            float distY = (Vector2.Distance(point0 + projYdirection, pointY) > axis.Length()) ? float.MaxValue : Vector2.Distance(projYdirection, mouseVector0);

            axis = pointZ - point0;
            Vector2 projZdirection = (Vector2.Dot(axis, mouseVector0) / axis.LengthSquared()) * axis;
            float distZ = (Vector2.Distance(point0 + projZdirection, pointZ) > axis.Length()) ? float.MaxValue : Vector2.Distance(projZdirection, mouseVector0);

            min = (distX < distY) ? ((distX < distZ) ? 1 : 3) : ((distY < distZ) ? 2 : 3);
            Vector2 proj;

            switch (min)
            {
                case 1:
                    proj = point0 + projXdirection;
                    berpotonganX = proj.X;
                    berpotonganY = proj.Y;
                    break;
                case 2:
                    proj = point0 + projYdirection;
                    berpotonganX = proj.X;
                    berpotonganY = proj.Y;
                    break;
                case 3:
                    proj = point0 + projZdirection;
                    berpotonganX = proj.X;
                    berpotonganY = proj.Y;
                    break;
            }
            
            panel1.Invalidate();
            label1.Text = string.Format("{0:0.00000000}", (DateTime.Now - start).TotalMilliseconds);
        }

        private void panel1_MouseDown(object sender, MouseEventArgs e)
        {

        }

        private void panel1_MouseUp(object sender, MouseEventArgs e)
        {

        }
    }
}
