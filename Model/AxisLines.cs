using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;


// TODO: belum ada icon. mungkin pengecekan klik jangan pake ray, tapi pake 2D saja
namespace EditorModel
{
    public class AxisLines : IObserver
    {
        float scale = 1;
        protected Vector3 xOffset;
        protected Vector3 yOffset;
        protected Vector3 zOffset;
        protected Vector3 xxOffset;
        protected Vector3 yyOffset;
        protected Vector3 zzOffset;
        protected BoundingBoxBuffer[] axisBoundingBoxBuffers;
        protected Vector3 posistionStart;
        protected Vector2 point0Start;
        protected ModelBoundingBox parent;
        Vector3 position;
        protected BoundingBox[] axisBoundingBoxes;
        protected int axisCount;
        protected VertexPositionColor[] axisVertices;
        protected string text;

        #region atribut yang terproyeksi di screen 2D
        protected Vector2 point0, pointX, pointY, pointZ, dragStart;
        protected int min;
        protected float mouseX, mouseY, mouseX2, mouseY2;
        protected bool dragStarted;
        private float zProjection = 0;
        #endregion

        public float Scale
        {
            get { return scale; }
            set 
            {
                scale = value;

                xOffset = new Vector3(0.5f, 0, 0) * scale;
                yOffset = new Vector3(0, 0.5f, 0) * scale;
                zOffset = new Vector3(0, 0, 0.5f) * scale;

                xxOffset = new Vector3(0f, 0.02f, 0.02f) * scale;
                yyOffset = new Vector3(0.02f, 0f, 0.02f) * scale;
                zzOffset = new Vector3(0.02f, 0.02f, 0f) * scale;
            }
        }

        public ModelBoundingBox Parent
        {
            get { return parent; }
            set { parent = value; }
        }

        public VertexPositionColor[] AxisVertices
        {
            get { return axisVertices; }
            set { axisVertices = value; }
        }

        public BoundingBox[] AxisBoundingBoxes
        {
            get { return axisBoundingBoxes; }
            set { axisBoundingBoxes = value; }
        }

        public Vector3 Position
        {
            get { return position; }
            set
            {
                position = value;

                axisVertices[0].Position = axisVertices[2].Position = axisVertices[4].Position = Vector3.Zero + position;

                axisVertices[1].Position = xOffset + position;
                axisVertices[3].Position = yOffset + position;
                axisVertices[5].Position = zOffset + position;

                CreateAxisBoundingBox();
            }
        }

        public string Text
        {
            get { return text; }
        }
        
        public AxisLines()
        {
            axisVertices = new VertexPositionColor[8];
            axisCount = 3;

            axisVertices[0].Color = axisVertices[1].Color = Color.Red;
            axisVertices[2].Color = axisVertices[3].Color = Color.LightGreen;
            axisVertices[4].Color = axisVertices[5].Color = Color.Blue;
            axisVertices[6].Color = axisVertices[7].Color = Color.Yellow;

            axisBoundingBoxBuffers = new BoundingBoxBuffer[3];

            xOffset = new Vector3(0.5f, 0, 0) * scale;
            yOffset = new Vector3(0, 0.5f, 0) * scale;
            zOffset = new Vector3(0, 0, 0.5f) * scale;

            xxOffset = new Vector3(0f, 0.02f, 0.02f) * scale;
            yyOffset = new Vector3(0.02f, 0f, 0.02f) * scale;
            zzOffset = new Vector3(0.02f, 0.02f, 0f) * scale;

            //2d
            point0 = new Vector2();
            pointX = new Vector2();
            pointY = new Vector2();
            pointZ = new Vector2();
            min = -1;
            mouseX = mouseY = -1;
            dragStarted = false;

            text = "";
        }

        public void Draw(ref BasicEffect effect, GraphicsDevice graphicsDevice)
        {
            if (zProjection > 1) return;
            effect.VertexColorEnabled = true;
            effect.World = Matrix.Identity;
            
            DepthStencilState d = new DepthStencilState();
            d.DepthBufferEnable = false;
            graphicsDevice.DepthStencilState = d;

            foreach (EffectPass pass in effect.CurrentTechnique.Passes)
            {
                pass.Apply();

                graphicsDevice.DrawUserPrimitives(PrimitiveType.LineList, axisVertices, 0, axisCount);
            }

            graphicsDevice.DepthStencilState = DepthStencilState.Default;
            graphicsDevice.SetVertexBuffer(null);
            graphicsDevice.Indices = null;

            //for (int i = 0; i < 3; i++)
            //    axisBoundingBoxBuffers[i].Draw(effect);
        }

        private bool OutOfBounds(Vector3 pos)
        {
            return !(pos.X >= 0 && pos.X <= parent.GraphicsDevice.Viewport.Bounds.Width && pos.Y >= 0 && pos.Y <= parent.GraphicsDevice.Viewport.Bounds.Height);
        }

        private void CreateAxisBoundingBox()
        {
            axisBoundingBoxes = new BoundingBox[3]
            {
                new BoundingBox(axisVertices[0].Position - xxOffset,  axisVertices[0].Position + xOffset + xxOffset),
                new BoundingBox(axisVertices[0].Position - yyOffset,  axisVertices[0].Position + yOffset + yyOffset),
                new BoundingBox(axisVertices[0].Position - zzOffset,  axisVertices[0].Position + zOffset + zzOffset)
            };

            for (int i = 0; i < 3; i++)
            {
                axisBoundingBoxBuffers[i] = new BoundingBoxBuffer(parent.GraphicsDevice);
                axisBoundingBoxBuffers[i].BoundingBox = axisBoundingBoxes[i];
            }
        }

        public void UpdateObserver()
        {
            if (parent.Camera.IsOrthographic)
                Scale = parent.Camera.Zoom / 6.5f;
            else if (scale != 1)
                Scale = 1;
            try
            {
                Position = position;
                Vector3 x1 = parent.GraphicsDevice.Viewport.Project(axisVertices[0].Position, parent.Camera.Projection, parent.Camera.World, Matrix.Identity);
                Vector3 x2 = parent.GraphicsDevice.Viewport.Project(axisVertices[1].Position, parent.Camera.Projection, parent.Camera.World, Matrix.Identity); //Helper.ProjectAndClipToViewport(axisVertices[1].Position, 0, 0, parent.GraphicsDevice.Viewport.Width, parent.GraphicsDevice.Viewport.Height, 0, 1, parent.Camera.World, out pointXWasInsideScreen);
                Vector3 y2 = parent.GraphicsDevice.Viewport.Project(axisVertices[3].Position, parent.Camera.Projection, parent.Camera.World, Matrix.Identity);
                Vector3 z2 = parent.GraphicsDevice.Viewport.Project(axisVertices[5].Position, parent.Camera.Projection, parent.Camera.World, Matrix.Identity);
                zProjection = Math.Max(x1.Z, Math.Max(x2.Z, Math.Max(y2.Z, z2.Z)));

                Ray ray = Helper.Pick(parent.GraphicsDevice.Viewport, parent.Camera, x1.X, x1.Y);
                Vector3 screenToAxis = ray.Direction * 2;
                //if (screenToAxis.Length() < (axisVertices[0].Position - ray.Position).Length())
                //{
                    Vector3 temp = axisVertices[0].Position;
                    axisVertices[0].Position = ray.Position + screenToAxis;
                    temp = axisVertices[0].Position - temp;
                    for (int i = 1; i < 6; i++)
                    {
                        axisVertices[i].Position += temp;
                    }

                    CreateAxisBoundingBox();
                //}

                point0.X = x1.X;
                point0.Y = x1.Y;
                pointX.X = x2.X;
                pointX.Y = x2.Y;
                pointY.X = y2.X;
                pointY.Y = y2.Y;
                pointZ.X = z2.X;
                pointZ.Y = z2.Y;

                text = (parent.Camera.Position - parent.Model.Position).Length() + "\r\n";
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.WriteLine(ex.StackTrace);
            }
        }

        public void LookingForClosest(float x, float y)
        {
            mouseX = x;
            mouseY = y;

            Vector2 axis = pointX - point0;
            Vector2 mouseVector = new Vector2(x, y);
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

            switch (min)
            {
                case 1:
                    dragStart = projXdirection;
                    break;
                case 2:
                    dragStart = projYdirection;
                    break;
                case 3:
                    dragStart = projZdirection;
                    break;
            }
        }

        public virtual void OnMouseMove(float x, float y)
        {
            if (dragStarted)
            {
                mouseX2 = x;
                mouseY2 = y;

                Vector2 axisEnd = Vector2.Zero;

                axisEnd = (min == 1) ? pointX : ((min == 2) ? pointY : pointZ);

                Vector2 axis = axisEnd - point0;
                Vector2 mouseVector = new Vector2(x, y);
                Vector2 mouseVector0 = mouseVector - point0;
                Vector2 proj = point0 + (Vector2.Dot(axis, mouseVector0) / axis.LengthSquared()) * axis;

                float sf = (proj - dragStart).Length(),
                      f0 = (proj - point0Start).Length(),
                      s0 = (dragStart - point0Start).Length(),
                      ratio = (!parent.Camera.IsOrthographic) ? (parent.Camera.Position - parent.Model.Position).Length() * 0.002f : parent.Camera.Zoom * 0.001f;

                float diff = sf * ratio;

                if ((sf >= f0 && sf >= s0) || (s0 >= f0 && s0 >= sf))
                {
                    diff *= -1;
                }

                Vector3 posistionTemp = posistionStart;
                switch (min)
                {
                    case 1:
                        posistionTemp.X += diff;
                        break;
                    case 2:
                        posistionTemp.Y += diff;
                        break;
                    case 3:
                        posistionTemp.Z += diff;
                        break;
                }

                parent.Model.Position = posistionTemp;
                parent.Model.Notify();
            }
        }

        public virtual int OnMouseDown(float x, float y)
        {
            this.min = -1;
            Ray ray = Helper.Pick(parent.GraphicsDevice.Viewport, parent.Camera, x, y);
            float min = float.MaxValue;
            Vector2 axisEnd = Vector2.Zero;
            for (short i = 0; i < 3; i++)
            {
                float? dist = ray.Intersects(axisBoundingBoxes[i]);
                if (dist != null && dist < min)
                {
                    min = (float)dist;
                    this.min = i + 1;
                    axisEnd = (this.min == 1) ? pointX : ((this.min == 2) ? pointY : pointZ);
                }
            }

            if (this.min != -1)
            {
                point0Start = point0;
                Vector2 axis = axisEnd - point0;
                Vector2 mouseVector = new Vector2(x, y);
                Vector2 mouseVector0 = mouseVector - point0;
                dragStart = point0 + (Vector2.Dot(axis, mouseVector0) / axis.LengthSquared()) * axis;

                dragStarted = true;
                mouseX = x;
                mouseY = y;

                posistionStart = parent.Model.Position;
            }

            return this.min;
        }

        public void OnMouseUp(float x, float y)
        {
            dragStarted = false;
            this.min = -1;
        }
    }
}
