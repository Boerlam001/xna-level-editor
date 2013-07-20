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
    public partial class Form1 : Form, IObserver
    {
        PointF point0, pointX, pointY, pointZ;
        private int min;
        float berpotonganX, berpotonganY, mouseX, mouseY;
        private bool dragStart;
        private int mouseX2;
        private int mouseY2;
        
        // suku persamaan linear

        // suku garis tegak lurus terdekat
        float a, b, c;

        // suku garis X,Y,Z
        float aX, bX, cX,
              aY, bY, cY,
              aZ, bZ, cZ;

        // suku garis tegak lurus min dan max X,Y,Z
        float aMinX, bMinX, cMinX,
              aMaxX, bMaxX, cMaxX;
        float aMinY, bMinY, cMinY,
              aMaxY, bMaxY, cMaxY;
        float aMinZ, bMinZ, cMinZ,
              aMaxZ, bMaxZ, cMaxZ;

        float x1MinX, y1MinX, x2MinX, y2MinX;

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
            point0 = new PointF();
            pointX = new PointF();
            pointY = new PointF();
            pointZ = new PointF();
            min = -1;
            berpotonganX = berpotonganY = -1;
            mouseX = mouseY = -1;
            dragStart = false;
        }

        public void AddAsBoundingBoxObserver()
        {
            mainUserControl1.Editor1.SelectedBoundingBox.BoundingBoxBuffer.Attach(this);
        }

        void IObserver.Update()
        {
            Editor editor = mainUserControl1.Editor1;
            BoundingBoxBuffer selectedBoundingBox = editor.SelectedBoundingBox.BoundingBoxBuffer;

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

            LinearEquationFrom2Points(point0, pointX, out aX, out bX, out cX);
            LinearEquationFrom2Points(point0, pointY, out aY, out bY, out cY);
            LinearEquationFrom2Points(point0, pointZ, out aZ, out bZ, out cZ);

            //gradien tegak lurus
            float m = bX / aX;

            //populate suku min max X
            LinearEquationFromGradientAndPoint(m, point0, out aMinX, out bMinX, out cMinX);
            LinearEquationFromGradientAndPoint(m, pointX, out aMaxX, out bMaxX, out cMaxX);


            //garis tegak lurus di min X
            #region percobaan1
            /* float aCirc = aMinX * aMinX + bMinX * bMinX;
             * float bCirc = 2 * aMinX * cMinX - 2 * bMinX * bMinX * point0.X - 2 * bMinX * point0.Y * aMinX;
             * float cCirc = cMinX * cMinX + point0.X * point0.X * bMinX * bMinX + point0.Y * point0.Y * bMinX * bMinX - 2 * bMinX * point0.Y * cMinX - 25;
             * SquareEquationRoot(aCirc, bCirc, cCirc, out x1MinX, out x2MinX);
             * if (x1MinX == float.MaxValue)
             * {
             *     x1MinX = -1;
             *     y1MinX = -1;
             * }
             * else
             * {
             *     y1MinX = (-aMinX * x1MinX - cMinX) / bMinX;
             * }
             */  
            #endregion

            //populate suku min max Y
            m = bY / aY;
            LinearEquationFromGradientAndPoint(m, point0, out aMinY, out bMinY, out cMinY);
            LinearEquationFromGradientAndPoint(m, pointY, out aMaxY, out bMaxY, out cMaxY);

            //populate suku min max Z
            m = bZ / aZ;
            LinearEquationFromGradientAndPoint(m, point0, out aMinZ, out bMinZ, out cMinZ);
            LinearEquationFromGradientAndPoint(m, pointZ, out aMaxZ, out bMaxZ, out cMaxZ);

            label2.Text =
                "point0: " + point0.X + ", " + point0.Y + "\r\n" +
                "pointX: " + pointX.X + ", " + pointX.Y + "\r\n" +
                "pointY: " + pointY.X + ", " + pointY.Y + "\r\n" +
                "pointZ: " + pointZ.X + ", " + pointZ.Y + "\r\n\r\n" +
                "MinX: " + aMinX + ", " + bMinX + ", " + cMinX + "\r\n" +
                "MaxX: " + aMaxX + ", " + bMaxX + ", " + cMaxX + "\r\n" +
                "MinY: " + aMinY + ", " + bMinY + ", " + cMinY + "\r\n" +
                "MaxY: " + aMaxY + ", " + bMaxY + ", " + cMaxY + "\r\n" +
                "MinZ: " + aMinZ + ", " + bMinZ + ", " + cMinZ + "\r\n" +
                "MaxZ: " + aMaxZ + ", " + bMaxZ + ", " + cMaxZ + "\r\n" +
                "x1MinX: " + x1MinX + ", y1MinX: " + y1MinX;

            panel1.Invalidate();
        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {
            if (mainUserControl1 == null ||
                mainUserControl1.Editor1 == null ||
                mainUserControl1.Editor1.SelectedBoundingBox == null)
                return;

            Pen pen = new Pen(Color.Black);

            e.Graphics.DrawString("0", Font, pen.Brush, point0.X - 5, point0.Y - 10);
            e.Graphics.FillEllipse(pen.Brush, point0.X - 1, point0.Y - 1, 2, 2);

            e.Graphics.DrawString("x", Font, pen.Brush, pointX.X - 5, pointX.Y - 10);
            e.Graphics.FillEllipse(pen.Brush, pointX.X - 1, pointX.Y - 1, 2, 2);

            e.Graphics.DrawString("y", Font, pen.Brush, pointY.X - 5, pointY.Y - 10);
            e.Graphics.FillEllipse(pen.Brush, pointY.X - 1, pointY.Y - 1, 2, 2);

            e.Graphics.DrawString("z", Font, pen.Brush, pointZ.X - 5, pointZ.Y - 10);
            e.Graphics.FillEllipse(pen.Brush, pointZ.X - 1, pointZ.Y - 1, 2, 2);

            //e.Graphics.DrawLine(pen, point0.X, point0.Y, x1MinX, y1MinX);

            switch (min)
            {
                case 1:
                    e.Graphics.DrawLine(pen, point0, pointX);
                    break;
                case 2:
                    e.Graphics.DrawLine(pen, point0, pointY);
                    break;
                case 3:
                    e.Graphics.DrawLine(pen, point0, pointZ);
                    break;
            }
            if (berpotonganX != -1)
                e.Graphics.DrawLine(pen, mouseX, mouseY, berpotonganX, berpotonganY);

            if (dragStart)
            {
                Pen pen2;
                if (a * mouseX2 + b * mouseY2 + c > 0)
                    pen2 = new Pen(Color.Blue);
                else
                    pen2 = new Pen(Color.Red);
                e.Graphics.DrawLine(pen2, mouseX, mouseY, mouseX2, mouseY2);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            AddAsBoundingBoxObserver();
        }

        void SquareEquationRoot(float a, float b, float c, out float x1, out float x2)
        {
            float determinant = b * b - 4 * a * c;
            if (determinant < 0)
            {
                x1 = float.MaxValue;
                x2 = float.MaxValue;
            }
            else
            {
                x1 = (-b - (float)Math.Sqrt(determinant)) / 2 * a;
                x2 = (-b + (float)Math.Sqrt(determinant)) / 2 * a;
            }
        }

        void LinearEquationFrom2Points(PointF p1, PointF p2, out float a, out float b, out float c)
        {
            a = p2.Y - p1.Y;
            b = -(p2.X - p1.X);
            c = -(p2.Y - p1.Y) * p1.X + (p2.X - p1.X) * p1.Y;
            if (a < 0)
            {
                a *= -1;
                b *= -1;
                c *= -1;
            }
        }

        void LinearEquationFromGradientAndPoint(float m, PointF p, out float a, out float b, out float c)
        {
            a = m;
            b = -1;
            c = -m * p.X + p.Y;
            if (a < 0)
            {
                a *= -1;
                b *= -1;
                c *= -1;
            }
        }

        void PointOfTwoLine(float a1, float b1, float c1, float a2, float b2, float c2, out float x, out float y)
        {
            #region desc
            /*
             *  I:
             *  a1*x + b1*y + c1 = 0;
             *  y = (-a1*x - c1) / b1;
             *  
             *  II:
             *  a2*x + b2*y + c2 = 0;
             *  a2*x + b2*((-a1*x - c1) / b1) + c2 = 0;
             *  a2*x - (b2*a1*x/b1) - (b2*c1/b1) + c2 = 0;
             *  (a2 - (b2*a1/b1))*x - (b2*c1/b1) + c2 = 0;
             */
            #endregion

            x = ((b2 * c1 / b1) - c2) / (a2 - (b2 * a1 / b1));
            y = (-a1 * x - c1) / b1;
        }

        public void LookingForClosest(ref MouseEventArgs e)
        {
            StringBuilder sb = new StringBuilder();

            float d1 = float.MaxValue,
                  d2 = float.MaxValue,
                  d3 = float.MaxValue,
                  d0 = (float)Math.Sqrt(Math.Pow(e.X - point0.X, 2) + Math.Pow(e.Y - point0.Y, 2));

            float testMin = aMinX * e.X + bMinX * e.Y + cMinX, testMax = aMaxX * e.X + bMaxX * e.Y + cMaxX;
            if ((testMin < 0 && testMax < 0) || (testMin > 0 && testMax > 0))
            {
                d1 = Math.Min(d0, (float)Math.Sqrt(Math.Pow(e.X - pointX.X, 2) + Math.Pow(e.Y - pointX.Y, 2)));
            }
            else
            {
                d1 = Math.Abs(aX * e.X + bX * e.Y + cX) / (float)Math.Sqrt(aX * aX + bX * bX);
            }

            sb.Append("testMinX: " + testMin + ", testMaxX: " + testMax + "\r\n");

            testMin = aMinY * e.X + bMinY * e.Y + cMinY; testMax = aMaxY * e.X + bMaxY * e.Y + cMaxY;
            if ((testMin < 0 && testMax < 0) || (testMin > 0 && testMax > 0))
            {
                d2 = Math.Min(d0, (float)Math.Sqrt(Math.Pow(e.X - pointY.X, 2) + Math.Pow(e.Y - pointY.Y, 2)));
            }
            else
            {
                d2 = Math.Abs(aY * e.X + bY * e.Y + cY) / (float)Math.Sqrt(aY * aY + bY * bY);
            }

            sb.Append("testMinY: " + testMin + ", testMaxY: " + testMax + "\r\n");

            testMin = aMinZ * e.X + bMinZ * e.Y + cMinZ; testMax = aMaxZ * e.X + bMaxZ * e.Y + cMaxZ;
            if ((testMin < 0 && testMax < 0) || (testMin > 0 && testMax > 0))
            {
                d3 = Math.Min(d0, (float)Math.Sqrt(Math.Pow(e.X - pointZ.X, 2) + Math.Pow(e.Y - pointZ.Y, 2)));
            }
            else
            {
                d3 = Math.Abs(aZ * e.X + bZ * e.Y + cZ) / (float)Math.Sqrt(aZ * aZ + bZ * bZ);
            }

            sb.Append("testMinZ: " + testMin + ", testMaxZ: " + testMax + "\r\n");

            sb.Append("d1: " + d1 + ", d2: " + d2 + ", d3: " + d3);

            label1.Text = sb.ToString();

            int tempMin = min;
            min = (d1 < d2) ? ((d1 < d3) ? 1 : 3) : ((d2 < d3) ? 2 : 3);

            float m = 1;
            switch (min)
            {
                case 1:
                    m = bX / aX;
                    LinearEquationFromGradientAndPoint(m, new PointF(e.X, e.Y), out a, out b, out c);
                    PointOfTwoLine(aX, bX, cX, a, b, c, out berpotonganX, out berpotonganY);
                    break;
                case 2:
                    m = bY / aY;
                    LinearEquationFromGradientAndPoint(m, new PointF(e.X, e.Y), out a, out b, out c);
                    PointOfTwoLine(aY, bY, cY, a, b, c, out berpotonganX, out berpotonganY);
                    break;
                case 3:
                    m = bZ / aZ;
                    LinearEquationFromGradientAndPoint(m, new PointF(e.X, e.Y), out a, out b, out c);
                    PointOfTwoLine(aZ, bZ, cZ, a, b, c, out berpotonganX, out berpotonganY);
                    break;
            }
            mouseX = e.X;
            mouseY = e.Y;
        }

        private void panel1_MouseMove(object sender, MouseEventArgs e)
        {
            //label3.Text = e.X + ", " + e.Y;
            DateTime start = DateTime.Now;
            if (!dragStart)
            {
                LookingForClosest(ref e);
            }
            else
            {
                mouseX2 = e.X;
                mouseY2 = e.Y;
            }
            panel1.Invalidate();
            label3.Text = string.Format("{0:0.00000000}", (DateTime.Now - start).TotalMilliseconds);
        }

        private void panel1_MouseDown(object sender, MouseEventArgs e)
        {
            dragStart = true;
            mouseX = e.X;
            mouseY = e.Y;
        }

        private void panel1_MouseUp(object sender, MouseEventArgs e)
        {
            dragStart = false;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            TryVector2 vectorForm = new TryVector2();
            vectorForm.form1 = this;
            vectorForm.SelectedBoundingBox = mainUserControl1.Editor1.SelectedBoundingBox.BoundingBoxBuffer;
            vectorForm.SelectedBoundingBox.Attach(vectorForm);
            vectorForm.Show();
        }
    }
}
