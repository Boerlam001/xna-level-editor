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
            base.OnMouseDown(x, y);
            rotationStart = parent.Model.RotationVector;
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

                parent.Model.RotationVector = rotatationTemp;
                parent.Model.Notify();
            }
        }
    }
}
