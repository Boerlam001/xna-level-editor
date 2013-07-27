using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace EditorModel
{
    public class BaseObject : Subject
    {
        protected string name;

        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        protected Vector3 position;
        protected Quaternion rotation;
        protected Vector3 scale;

        protected Vector3 rotationVector;

        public Vector3 RotationVector
        {
            get { return rotationVector; }
            set
            {
                rotationX = value.X % 360;
                rotationY = value.Y % 360;
                rotationZ = value.Z % 360;
                rotationVector = new Vector3(rotation.X, rotation.Y, rotation.Z);

                world = Matrix.CreateScale(scale) * Matrix.CreateFromYawPitchRoll(MathHelper.ToRadians(rotationY), MathHelper.ToRadians(rotationX), MathHelper.ToRadians(rotationZ)) * Matrix.CreateTranslation(position);
                world.Decompose(out scale, out rotation, out position);

                OnRotationChanged(this, null);
            }
        }

        protected float rotationX;
        protected float rotationY;
        protected float rotationZ;
        protected Matrix world;

        public delegate void RotationChangedEventHandler(object sender, EventArgs e);
        public RotationChangedEventHandler RotationChanged;

        public static Vector3 rotationReference = new Vector3(0, 0, 10);
        
        public virtual Vector3 Position
        {
            get { return position; }
            set
            {
                position = value;
                world = Matrix.CreateScale(scale) * Matrix.CreateFromYawPitchRoll(MathHelper.ToRadians(rotationY), MathHelper.ToRadians(rotationX), MathHelper.ToRadians(rotationZ)) * Matrix.CreateTranslation(position);
            }
        }

        public virtual Quaternion Rotation
        {
            get { return rotation; }
            set
            {
                rotation = value;
                world = Matrix.CreateScale(scale) * Matrix.CreateFromQuaternion(rotation) * Matrix.CreateTranslation(position);
                Helper.QuaternionToEuler(rotation, out rotationX, out rotationY, out rotationZ);
                rotationVector = new Vector3(rotation.X, rotation.Y, rotation.Z);
                OnRotationChanged(this, null);
            }
        }

        public virtual float RotationX
        {
            get { return rotationX; }
            set
            {
                rotationX = value % 360;
                rotationVector.X = rotationX;
                world = Matrix.CreateScale(scale) * Matrix.CreateFromYawPitchRoll(MathHelper.ToRadians(rotationY), MathHelper.ToRadians(rotationX), MathHelper.ToRadians(rotationZ)) * Matrix.CreateTranslation(position);
                world.Decompose(out scale, out rotation, out position);

                OnRotationChanged(this, null);
            }
        }

        public virtual float RotationY
        {
            get { return rotationY; }
            set
            {
                rotationY = value % 360;
                rotationVector.Y = rotationY;
                world = Matrix.CreateScale(scale) * Matrix.CreateFromYawPitchRoll(MathHelper.ToRadians(rotationY), MathHelper.ToRadians(rotationX), MathHelper.ToRadians(rotationZ)) * Matrix.CreateTranslation(position);
                world.Decompose(out scale, out rotation, out position);

                OnRotationChanged(this, null);
            }
        }

        public virtual float RotationZ
        {
            get { return rotationZ; }
            set
            {
                rotationZ = value % 360;
                rotationVector.Z = rotationZ;
                world = Matrix.CreateScale(scale) * Matrix.CreateFromYawPitchRoll(MathHelper.ToRadians(rotationY), MathHelper.ToRadians(rotationX), MathHelper.ToRadians(rotationZ)) * Matrix.CreateTranslation(position);
                world.Decompose(out scale, out rotation, out position);

                OnRotationChanged(this, null);
            }
        }

        public virtual Matrix World
        {
            get { return world; }
        }

        public BaseObject()
        {
            position = new Vector3(0, 0, 0);
            rotationX = rotationY = rotationZ = 0;
            rotationVector = new Vector3(rotation.X, rotation.Y, rotation.Z);
            scale = Vector3.One;
            world = Matrix.CreateScale(scale) * Matrix.CreateFromYawPitchRoll(MathHelper.ToRadians(rotationY), MathHelper.ToRadians(rotationX), MathHelper.ToRadians(rotationZ)) * Matrix.CreateTranslation(position);
            world.Decompose(out scale, out rotation, out position);
        }

        public virtual void Rotate(float x, float y, float z)
        {
            rotationY += y;
            rotationX += x;
            rotationZ += z;
            rotationVector = new Vector3(rotation.X, rotation.Y, rotation.Z);
            world = Matrix.CreateScale(scale) * Matrix.CreateFromYawPitchRoll(MathHelper.ToRadians(rotationY), MathHelper.ToRadians(rotationX), MathHelper.ToRadians(rotationZ)) * Matrix.CreateTranslation(position);
            world.Decompose(out scale, out rotation, out position);
        }

        public void LookAt(Vector3 target)
        {
            world = Matrix.CreateLookAt(position, target, Vector3.Up);
            world.Decompose(out scale, out rotation, out position);
        }

        protected void OnRotationChanged(object sender, EventArgs e)
        {
            if (RotationChanged != null)
                RotationChanged(this, e);
        }

        public void MoveForward(float speed)
        {
            Vector3 v = new Vector3(0, 0, speed);
            v = Vector3.Transform(v, Matrix.CreateFromQuaternion(rotation));
            v.X += position.X;
            v.Y += position.Y;
            v.Z += position.Z;
            Position = v;
        }

        public void MoveRight(float speed)
        {
            Vector3 v = new Vector3(-speed, 0, 0);
            v = Vector3.Transform(v, Matrix.CreateFromQuaternion(rotation));
            v.X += position.X;
            v.Y += position.Y;
            v.Z += position.Z;
            Position = v;
        }
    }
}