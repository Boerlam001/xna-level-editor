using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace EditorModel
{
    public class ScaleAxisLines : AxisLines
    {
        private Vector3 scaleStart;

        public override int OnMouseDown(float x, float y)
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
                mouseX2 = x;
                mouseY2 = y;

                scaleStart = parent.Model.Scale;
            }
            return this.min;
        }

        public override void OnMouseMove(float x, float y)
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
                      ratio = (parent.Camera.Position - parent.Model.Position).Length() * 0.002f;

                float diff = sf * ratio;

                if ((sf >= f0 && sf >= s0) || (s0 >= f0 && s0 >= sf))
                {
                    diff *= -1;
                }

                Vector3 scaleTemp = scaleStart;
                switch (min)
                {
                    case 1:
                        scaleTemp.X += diff;
                        break;
                    case 2:
                        scaleTemp.Y += diff;
                        break;
                    case 3:
                        scaleTemp.Z += diff;
                        break;
                }

                parent.Model.Scale = scaleTemp;
                parent.Model.Notify();
            }
        }
    }
}
