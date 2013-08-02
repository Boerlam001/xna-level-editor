using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace EditorModel
{
    public class RotationAxisLines : AxisLines
    {
        protected Vector3 rotationStart;

        public override int OnMouseDown(float x, float y)
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
                mouseX2 = x;
                mouseY2 = y;

                rotationStart = parent.Model.EulerRotation;
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
                Vector2 mouseVectorFromProj = mouseVector - proj;
                float diff;
                if ((axis.Y >= 0 && mouseVectorFromProj.X >= 0) || (axis.Y < 0 && mouseVectorFromProj.X < 0))
                    diff = mouseVectorFromProj.Length() / 10;
                else
                    diff = -mouseVectorFromProj.Length() / 10;

                Vector3 rotatationTemp = rotationStart;
                switch (min)
                {
                    case 1:
                        rotatationTemp.X += diff;
                        break;
                    case 2:
                        rotatationTemp.Y += diff;
                        break;
                    case 3:
                        rotatationTemp.Z += diff;
                        break;
                }

                parent.Model.EulerRotation = rotatationTemp;
                parent.Model.Notify();
            }
        }
    }
}
