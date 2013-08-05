using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;


// TODO: axis line masih berantakan (length-nya agak kacau). belum ada icon. mungkin pengecekan klik jangan pake ray, tapi pake 2D saja
namespace EditorModel
{
    public class AxisLines : IObserver
    {
        protected ModelBoundingBox parent;

        public ModelBoundingBox Parent
        {
            get { return parent; }
            set { parent = value; }
        }

        protected VertexPositionColor[] axisVertices;

        public VertexPositionColor[] AxisVertices
        {
            get { return axisVertices; }
            set { axisVertices = value; }
        }

        protected int axisCount;

        protected BoundingBox[] axisBoundingBoxes;

        public BoundingBox[] AxisBoundingBoxes
        {
            get { return axisBoundingBoxes; }
            set { axisBoundingBoxes = value; }
        }

        Vector3 position;

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

        protected Vector3 xOffset;
        protected Vector3 yOffset;
        protected Vector3 zOffset;
        protected Vector3 xxOffset;
        protected Vector3 yyOffset;
        protected Vector3 zzOffset;

        protected BoundingBoxBuffer[] axisBoundingBoxBuffers;

        protected string text;
        protected Vector3 posistionStart;
        protected Vector2 point0Start;


        #region atribut yang terproyeksi di screen 2D

        protected Vector2 point0, pointX, pointY, pointZ, dragStart;
        protected int min;
        protected float mouseX, mouseY, mouseX2, mouseY2;
        protected bool dragStarted;

        #endregion

        
        public AxisLines()
        {
            axisVertices = new VertexPositionColor[8];
            axisCount = 3;

            axisVertices[0].Color = axisVertices[1].Color = Color.Red;
            axisVertices[2].Color = axisVertices[3].Color = Color.Green;
            axisVertices[4].Color = axisVertices[5].Color = Color.Blue;
            axisVertices[6].Color = axisVertices[7].Color = Color.Yellow;

            axisBoundingBoxBuffers = new BoundingBoxBuffer[3];

            xOffset = new Vector3(0.5f, 0, 0);
            yOffset = new Vector3(0, 0.5f, 0);
            zOffset = new Vector3(0, 0, 0.5f);

            xxOffset = new Vector3(0f, 0.02f, 0.02f);
            yyOffset = new Vector3(0.02f, 0f, 0.02f);
            zzOffset = new Vector3(0.02f, 0.02f, 0f);

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
            effect.VertexColorEnabled = true;
            
            DepthStencilState d = new DepthStencilState();
            d.DepthBufferEnable = false;

            foreach (EffectPass pass in effect.CurrentTechnique.Passes)
            {
                pass.Apply();

                graphicsDevice.DepthStencilState = d;
                graphicsDevice.DrawUserPrimitives(PrimitiveType.LineList, axisVertices, 0, axisCount);
                graphicsDevice.DepthStencilState = DepthStencilState.Default;
            }
            
            graphicsDevice.SetVertexBuffer(null);
            graphicsDevice.Indices = null;

            //for (int i = 0; i < 3; i++)
            //    axisBoundingBoxBuffers[i].Draw(effect);

            //try
            //{
            //    parent.SpriteBatch.Begin();
            //    parent.SpriteBatch.DrawString(parent.SpriteFont, text, new Vector2(50, 50), Color.White);
            //    parent.SpriteBatch.End();
            //}
            //catch (Exception ex)
            //{
            //    parent.SpriteBatch.End();
            //}
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

        public void Update()
        {
            try
            {
                Position = position;
                Vector3 x1 = parent.GraphicsDevice.Viewport.Project(axisVertices[0].Position, parent.Camera.Projection, parent.Camera.World, Matrix.Identity);
                Vector3 x2 = parent.GraphicsDevice.Viewport.Project(axisVertices[1].Position, parent.Camera.Projection, parent.Camera.World, Matrix.Identity); //Helper.ProjectAndClipToViewport(axisVertices[1].Position, 0, 0, parent.GraphicsDevice.Viewport.Width, parent.GraphicsDevice.Viewport.Height, 0, 1, parent.Camera.World, out pointXWasInsideScreen);
                Vector3 y2 = parent.GraphicsDevice.Viewport.Project(axisVertices[3].Position, parent.Camera.Projection, parent.Camera.World, Matrix.Identity);
                Vector3 z2 = parent.GraphicsDevice.Viewport.Project(axisVertices[5].Position, parent.Camera.Projection, parent.Camera.World, Matrix.Identity);

                Ray ray = Helper.Pick(parent.GraphicsDevice, parent.Camera, x1.X, x1.Y);
                Vector3 screenToAxis = ray.Direction * 2;
                if (screenToAxis.Length() < (axisVertices[0].Position - ray.Position).Length())
                {
                    Vector3 temp = axisVertices[0].Position;
                    axisVertices[0].Position = ray.Position + screenToAxis;
                    temp = axisVertices[0].Position - temp;
                    for (int i = 1; i < 6; i++)
                    {
                        axisVertices[i].Position += temp;
                    }

                    CreateAxisBoundingBox();
                }

                point0.X = x1.X;
                point0.Y = x1.Y;
                pointX.X = x2.X;
                pointX.Y = x2.Y;
                pointY.X = y2.X;
                pointY.Y = y2.Y;
                pointZ.X = z2.X;
                pointZ.Y = z2.Y;
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

                text = dragStart + " " + proj + " " + min;

                float sf = (proj - dragStart).Length(),
                      f0 = (proj - point0Start).Length(),
                      s0 = (dragStart - point0Start).Length();

                float diff = sf / 100;

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
            Ray ray = Helper.Pick(parent.GraphicsDevice, parent.Camera, x, y);
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



        /*
                //if ((axisVertices[0].Position - parent.Camera.Position).Z > 0)
                //{
                    //float lengthX = (axisVertices[1].Position - axisVertices[0].Position).Length(), subtractor = 1;
                    //while (OutOfBounds(x2) && lengthX > 0)
                    //{
                    //    lengthX -= subtractor;
                    //    if (lengthX == 0)
                    //    {
                    //        lengthX += subtractor;
                    //        subtractor /= 10;
                    //        lengthX -= subtractor;
                    //    }
                    //    x2 = parent.GraphicsDevice.Viewport.Project(axisVertices[0].Position + new Vector3(lengthX, 0, 0), parent.Camera.Projection, parent.Camera.World, Matrix.Identity);
                    //}
                    //
                    //float lengthY = (axisVertices[3].Position - axisVertices[0].Position).Length(); subtractor = 1;
                    //while (OutOfBounds(y2) && lengthY > 0)
                    //{
                    //    lengthY -= subtractor;
                    //    if (lengthY == 0)
                    //    {
                    //        lengthY += subtractor;
                    //        subtractor /= 10;
                    //        lengthY -= subtractor;
                    //    }
                    //    y2 = parent.GraphicsDevice.Viewport.Project(axisVertices[0].Position + new Vector3(0, lengthY, 0), parent.Camera.Projection, parent.Camera.World, Matrix.Identity);
                    //}
                    //
                    //float lengthZ = (axisVertices[5].Position - axisVertices[0].Position).Length(); subtractor = 1;
                    //while (OutOfBounds(z2) && lengthZ > 0)
                    //{
                    //    lengthZ -= subtractor;
                    //    if (lengthX == 0)
                    //    {
                    //        lengthZ += subtractor;
                    //        subtractor /= 10;
                    //        lengthZ -= subtractor;
                    //    }
                    //    z2 = parent.GraphicsDevice.Viewport.Project(axisVertices[0].Position + new Vector3(0, 0, lengthZ), parent.Camera.Projection, parent.Camera.World, Matrix.Identity);
                    //}
                    //
                    //text =
                    //x1.ToString() + "\r\n" +
                    //x2.ToString() + " " + lengthX + "\r\n" +
                    //y2.ToString() + " " + lengthY + "\r\n" +
                    //z2.ToString() + " " + lengthZ;
                //}
                //else
                //{
                //    text =
                //    x1.ToString() + "\r\n" +
                //    x2.ToString() + " " + (axisVertices[0].Position - parent.Camera.Position).Z + "\r\n" +
                //    y2.ToString() + "\r\n" +
                //    z2.ToString();
                //}

                //point0.X = x1.X;
                //point0.Y = x1.Y;
                //pointX.X = x2.X;
                //pointX.Y = x2.Y;
                //pointY.X = y2.X;
                //pointY.Y = y2.Y;
                //pointZ.X = z2.X;
                //pointZ.Y = z2.Y;
                //
                //axisBoundingBoxBuffers = new BoundingBoxBuffer[6];
                //for (int i = 0; i < 3; i++)
                //{
                //    axisBoundingBoxBuffers[i] = new BoundingBoxBuffer(parent.GraphicsDevice);
                //    axisBoundingBoxBuffers[i].BoundingBox = axisBoundingBoxes[i];
                //}
                //
                //axisCount = 3;
                //
                //Vector2 direction = pointX - point0;
                //float dist = direction.Length(), length = 100;
                //if (dist > length)
                //{
                //    direction.Normalize();
                //    pointX = point0 + direction * length;
                //    Ray ray = Helper.Pick(parent.GraphicsDevice, parent.Camera, pointX.X, pointX.Y);
                //    float? intersectsAt = ray.Intersects(axisBoundingBoxes[0]);
                //    if (intersectsAt != null)
                //    {
                //        xOffset.X = (ray.Position + ray.Direction * (float)intersectsAt - position).X;
                //    }
                //    axisVertices[1].Position = position + xOffset;
                //}
                //
                //direction = pointY - point0;
                //dist = direction.Length();
                //if (dist > length)
                //{
                //    direction.Normalize();
                //    pointY = point0 + direction * length;
                //    Ray ray = Helper.Pick(parent.GraphicsDevice, parent.Camera, pointY.X, pointY.Y);
                //    float? intersectsAt = ray.Intersects(axisBoundingBoxes[1]);
                //    if (intersectsAt != null)
                //    {
                //        yOffset.Y = (ray.Position + ray.Direction * (float)intersectsAt - position).Y;
                //    }
                //    axisVertices[3].Position = position + yOffset;
                //}
                //
                //direction = pointZ - point0;
                //dist = direction.Length();
                //if (dist > length)
                //{
                //    direction.Normalize();
                //    pointZ = point0 + direction * length;
                //    Ray ray = Helper.Pick(parent.GraphicsDevice, parent.Camera, pointZ.X, pointZ.Y);
                //    float? intersectsAt = ray.Intersects(axisBoundingBoxes[2]);
                //    if (intersectsAt != null)
                //    {
                //        zOffset.Z = (ray.Position + ray.Direction * (float)intersectsAt - position).Z;
                //    }
                //    axisVertices[5].Position = position + zOffset;
                //}
                //
                //axisBoundingBoxes = new BoundingBox[3]
                //{
                //    new BoundingBox(Vector3.Zero - xxOffset + position, xOffset + xxOffset + position),
                //    new BoundingBox(Vector3.Zero - yyOffset + position, yOffset + yyOffset + position),
                //    new BoundingBox(Vector3.Zero - zzOffset + position, zOffset + zzOffset + position)
                //};
                //
                //for (int i = 3; i < 6; i++)
                //{
                //    axisBoundingBoxBuffers[i] = new BoundingBoxBuffer(parent.GraphicsDevice);
                //    axisBoundingBoxBuffers[i].BoundingBox = axisBoundingBoxes[i - 3];
                //}
                 */
    }
}
