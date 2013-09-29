using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using Microsoft.Xna.Framework;

namespace EditorModel
{
    public class BaseObject : Subject
    {
        #region attributes
        private Vector3 direction;
        protected float rotationX;
        protected float rotationY;
        protected float rotationZ;
        protected Matrix world;
        protected string name;
        protected Vector3 position;
        protected Quaternion rotation;
        protected bool isMoving;
        protected Vector3 scale;
        protected Vector3 eulerRotation;
        public static Vector3 rotationReference = new Vector3(0, 0, 10);
        protected bool isActive;
        protected bool isStatic;
        private List<string> scripts;
        #endregion

        public delegate void RotationChangedEventHandler(object sender, EventArgs e);
        public RotationChangedEventHandler RotationChanged;

        public delegate void PositionChangedEventHandler(object sender, EventArgs e);
        public RotationChangedEventHandler PositionChanged;

        #region setters and getters
        [Browsable(false)]
        public bool IsMoving
        {
            get { return isMoving; }
            set { isMoving = value; }
        }

        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        [Category("Transform")]
        public Vector3 Scale
        {
            get { return scale; }
            set
            {
                scale = value;
                world = Matrix.CreateScale(scale) * Matrix.CreateFromYawPitchRoll(MathHelper.ToRadians(rotationY), MathHelper.ToRadians(rotationX), MathHelper.ToRadians(rotationZ)) * Matrix.CreateTranslation(position);
            }
        }

        [Category("Transform")]
        public Vector3 EulerRotation
        {
            get { return eulerRotation; }
            set
            {
                rotationX = value.X % 360;
                rotationY = value.Y % 360;
                rotationZ = value.Z % 360;
                eulerRotation = new Vector3(rotationX, rotationY, rotationZ);

                world = Matrix.CreateScale(scale) * Matrix.CreateFromYawPitchRoll(MathHelper.ToRadians(rotationY), MathHelper.ToRadians(rotationX), MathHelper.ToRadians(rotationZ)) * Matrix.CreateTranslation(position);
                world.Decompose(out scale, out rotation, out position);

                OnRotationChanged(this, null);
            }
        }

        [Category("Transform")]
        public virtual Vector3 Position
        {
            get { return position; }
            set
            {
                position = value;
                world = Matrix.CreateScale(scale) * Matrix.CreateFromYawPitchRoll(MathHelper.ToRadians(rotationY), MathHelper.ToRadians(rotationX), MathHelper.ToRadians(rotationZ)) * Matrix.CreateTranslation(position);

                OnPositionChanged(this, null);
            }
        }

        [Category("Transform")]
        public virtual Quaternion Rotation
        {
            get { return rotation; }
            set
            {
                rotation = value;
                world = Matrix.CreateScale(scale) * Matrix.CreateFromQuaternion(rotation) * Matrix.CreateTranslation(position);
                Helper.QuaternionToEuler(rotation, out rotationX, out rotationY, out rotationZ);
                eulerRotation = new Vector3(rotationX, rotationY, rotationZ);
                OnRotationChanged(this, null);
            }
        }

        [Category("Transform")]
        public Vector3 Direction
        {
            get { return direction; }
            set
            {
                direction = value;
                Matrix w = Matrix.CreateLookAt(position, position + direction, Vector3.Up);
                Vector3 s, t;
                Quaternion r;
                w.Decompose(out s, out r, out t);
                Rotation = r;
            }
        }

        [Browsable(false)]
        public virtual float RotationX
        {
            get { return rotationX; }
            set
            {
                rotationX = value % 360;
                eulerRotation.X = rotationX;
                world = Matrix.CreateScale(scale) * Matrix.CreateFromYawPitchRoll(MathHelper.ToRadians(rotationY), MathHelper.ToRadians(rotationX), MathHelper.ToRadians(rotationZ)) * Matrix.CreateTranslation(position);
                world.Decompose(out scale, out rotation, out position);

                OnRotationChanged(this, null);
            }
        }

        [Browsable(false)]
        public virtual float RotationY
        {
            get { return rotationY; }
            set
            {
                rotationY = value % 360;
                eulerRotation.Y = rotationY;
                world = Matrix.CreateScale(scale) * Matrix.CreateFromYawPitchRoll(MathHelper.ToRadians(rotationY), MathHelper.ToRadians(rotationX), MathHelper.ToRadians(rotationZ)) * Matrix.CreateTranslation(position);
                world.Decompose(out scale, out rotation, out position);

                OnRotationChanged(this, null);
            }
        }

        [Browsable(false)]
        public virtual float RotationZ
        {
            get { return rotationZ; }
            set
            {
                rotationZ = value % 360;
                eulerRotation.Z = rotationZ;
                world = Matrix.CreateScale(scale) * Matrix.CreateFromYawPitchRoll(MathHelper.ToRadians(rotationY), MathHelper.ToRadians(rotationX), MathHelper.ToRadians(rotationZ)) * Matrix.CreateTranslation(position);
                world.Decompose(out scale, out rotation, out position);

                OnRotationChanged(this, null);
            }
        }


        [Category("Transform")]
        public virtual Matrix World
        {
            get { return world; }
        }

        [Category("Body")]
        public bool IsActive
        {
            get { return isActive; }
            set { isActive = value; }
        }

        [Category("Body")]
        public bool IsStatic
        {
            get { return isStatic; }
            set { isStatic = value; }
        }

        [Description("The controller scripts")]
        [EditorAttribute("System.Windows.Forms.Design.StringCollectionEditor, System.Design, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", typeof(System.Drawing.Design.UITypeEditor))]
        public List<string> Scripts
        {
            get { return scripts; }
            set { scripts = value; }
        }
        #endregion

        public BaseObject()
        {
            position = new Vector3(0, 0, 0);
            rotationX = rotationY = rotationZ = 0;
            eulerRotation = new Vector3(rotationX, rotationY, rotationZ);
            scale = Vector3.One;
            world = Matrix.CreateScale(scale) * Matrix.CreateFromYawPitchRoll(MathHelper.ToRadians(rotationY), MathHelper.ToRadians(rotationX), MathHelper.ToRadians(rotationZ)) * Matrix.CreateTranslation(position);
            world.Decompose(out scale, out rotation, out position);

            OnRotationChanged(this, null);
        }

        public virtual void Rotate(float x, float y, float z)
        {
            rotationY += y;
            rotationX += x;
            rotationZ += z;
            eulerRotation = new Vector3(rotationX, rotationY, rotationZ);
            world = Matrix.CreateScale(scale) * Matrix.CreateFromYawPitchRoll(MathHelper.ToRadians(rotationY), MathHelper.ToRadians(rotationX), MathHelper.ToRadians(rotationZ)) * Matrix.CreateTranslation(position);
            world.Decompose(out scale, out rotation, out position);

            OnRotationChanged(this, null);
        }

        public void LookAt(Vector3 target)
        {
            world = Matrix.CreateLookAt(position, target, Vector3.Up);
            world.Decompose(out scale, out rotation, out position);

            OnRotationChanged(this, null);
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

        protected virtual void OnRotationChanged(object sender, EventArgs e)
        {
            direction = new Vector3(0, 0, 1);
            direction = Vector3.Transform(direction, Matrix.CreateFromQuaternion(rotation));
            direction.Normalize();

            if (RotationChanged != null)
                RotationChanged(sender, e);
        }

        protected virtual void OnPositionChanged(object sender, EventArgs e)
        {
            if (PositionChanged != null)
                PositionChanged(sender, e);
        }
    }
}