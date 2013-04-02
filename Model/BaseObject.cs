using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace EditorModel
{
    public class BaseObject
    {
        protected Vector3 position;
        protected Quaternion rotation;
        protected Vector3 scale;

        protected float rotationX;
        protected float rotationY;
        protected float rotationZ;
        protected Matrix world;

        public delegate void RotationChangedEventHandler(object sender, EventArgs e);
        public RotationChangedEventHandler RotationChanged;

        public static Vector3 rotationReference = new Vector3(0, 0, 10);
        
        public Vector3 Position
        {
            get { return position; }
            set
            {
                position = value;
                world = Matrix.CreateTranslation(position) * Matrix.CreateFromYawPitchRoll(MathHelper.ToRadians(rotationY), MathHelper.ToRadians(rotationX), MathHelper.ToRadians(rotationZ));
            }
        }

        public Quaternion Rotation
        {
            get { return rotation; }
            set
            {
                rotation = value;
                world = Matrix.CreateTranslation(position) * Matrix.CreateFromQuaternion(rotation);
                Helper.QuaternionToEuler(rotation, out rotationX, out rotationY, out rotationZ);
                
                RotationChanged(this, null);
            }
        }

        public float RotationX
        {
            get { return rotationX; }
            set
            {
                rotationX = value % 360;
                //if (rotationX < 0)
                //    rotationX = 360 + rotationX;
                //if (rotationX > 90 && rotationX < 180)
                //    rotationX = 90;
                //if (rotationX >= 180 && rotationX < 270)
                //    rotationX = 270;
                world = Matrix.CreateTranslation(position) * Matrix.CreateFromYawPitchRoll(MathHelper.ToRadians(rotationY), MathHelper.ToRadians(rotationX), MathHelper.ToRadians(rotationZ));
                world.Decompose(out scale, out rotation, out position);

                RotationChanged(this, null);
            }
        }

        public float RotationY
        {
            get { return rotationY; }
            set
            {
                rotationY = value % 360;
                //if (rotationY < 0)
                //    rotationY = 360 + rotationY;
                world = Matrix.CreateTranslation(position) * Matrix.CreateFromYawPitchRoll(MathHelper.ToRadians(rotationY), MathHelper.ToRadians(rotationX), MathHelper.ToRadians(rotationZ));
                world.Decompose(out scale, out rotation, out position);

                RotationChanged(this, null);
            }
        }

        public float RotationZ
        {
            get { return rotationZ; }
            set
            {
                rotationZ = value % 360;
                //if (rotationZ < 0)
                //    rotationZ = 360 + rotationZ;
                world = Matrix.CreateTranslation(position) * Matrix.CreateFromYawPitchRoll(MathHelper.ToRadians(rotationY), MathHelper.ToRadians(rotationX), MathHelper.ToRadians(rotationZ));
                world.Decompose(out scale, out rotation, out position);

                RotationChanged(this, null);
            }
        }

        public Matrix World
        {
            get { return world; }
        }

        public BaseObject()
        {
            position = new Vector3(0, 0, 0);
            rotationX = rotationY = rotationZ = 0;
            world = Matrix.CreateTranslation(position) * Matrix.CreateFromYawPitchRoll(MathHelper.ToRadians(rotationY), MathHelper.ToRadians(rotationX), MathHelper.ToRadians(rotationZ));
            world.Decompose(out scale, out rotation, out position);
        }

        public void Rotate(float x, float y, float z)
        {
            rotationY += y;
            rotationX += x;
            rotationZ += z;
            Matrix rotationMatrix = Matrix.CreateFromYawPitchRoll(MathHelper.ToRadians(rotationY), MathHelper.ToRadians(rotationX), MathHelper.ToRadians(rotationZ));
            Vector3 transformedReference = Vector3.Transform(rotationReference, rotationMatrix);
            Vector3 lookAtVector = position + transformedReference;
            world = Matrix.CreateLookAt(position, lookAtVector, Vector3.Up);
        }
    }
}