using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace XleModel
{
    public class BaseObject : DrawableGameComponent
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
        List<ControllerScript> scripts;
        #endregion

        public delegate void RotationChangedEventHandler(object sender, EventArgs e);
        public RotationChangedEventHandler RotationChanged;

        public delegate void PositionChangedEventHandler(object sender, EventArgs e);
        public RotationChangedEventHandler PositionChanged;

        #region setters and getters
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

        public Vector3 Scale
        {
            get { return scale; }
            set
            {
                scale = value;
                world = Matrix.CreateScale(scale) * Matrix.CreateFromYawPitchRoll(MathHelper.ToRadians(rotationY), MathHelper.ToRadians(rotationX), MathHelper.ToRadians(rotationZ)) * Matrix.CreateTranslation(position);
            }
        }

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

        public Vector3 Direction
        {
            get { return direction; }
            set { direction = value; }
        }

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

        public virtual Quaternion Rotation
        {
            get { return rotation; }
            set
            {
                rotation = value;
                world = Matrix.CreateScale(scale) * Matrix.CreateFromQuaternion(rotation) * Matrix.CreateTranslation(position);
                world.Decompose(out scale, out rotation, out position);
                Helper.QuaternionToEuler(rotation, out rotationX, out rotationY, out rotationZ);
                eulerRotation = new Vector3(rotationX, rotationY, rotationZ);
                OnRotationChanged(this, null);
            }
        }

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

        public virtual Matrix World
        {
            get { return world; }
            set
            {
                world = value;
                world.Decompose(out scale, out rotation, out position);
                OnRotationChanged(this, null);
                OnPositionChanged(this, null);
            }
        }
        #endregion

        public BaseObject(Game game)
            : base(game)
        {
            position = new Vector3(0, 0, 0);
            rotationX = rotationY = rotationZ = 0;
            eulerRotation = new Vector3(rotationX, rotationY, rotationZ);
            scale = Vector3.One;
            world = Matrix.CreateScale(scale) * Matrix.CreateFromYawPitchRoll(MathHelper.ToRadians(rotationY), MathHelper.ToRadians(rotationX), MathHelper.ToRadians(rotationZ)) * Matrix.CreateTranslation(position);
            world.Decompose(out scale, out rotation, out position);
            scripts = new List<ControllerScript>();

            OnRotationChanged(this, null);
        }

        public override void Initialize()
        {
            base.Initialize();

            foreach (ControllerScript script in scripts)
            {
                script.Start();
            }
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            foreach (ControllerScript script in scripts)
            {
                script.Update(gameTime);
            }
        }

        public void AddScript(ControllerScript script)
        {
            script.Game = Game;
            script.Parent = this;
            scripts.Add(script);
        }

        public void RemoveScript(int index)
        {
            if (index < scripts.Count)
                scripts.RemoveAt(index);
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