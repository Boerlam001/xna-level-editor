using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Jitter;
using Jitter.Collision;
using Jitter.Collision.Shapes;
using Jitter.Dynamics;
using Jitter.LinearMath;

namespace XleModel
{
    public class BaseObject : DrawableGameComponent
    {
        public delegate void RotationChangedEventHandler(object sender, EventArgs e);
        public RotationChangedEventHandler RotationChanged;

        public delegate void PositionChangedEventHandler(object sender, EventArgs e);
        public RotationChangedEventHandler PositionChanged;

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
        protected World physicsWorld;
        protected bool physicsEnabled;
        protected PhysicsAdapter physicsAdapter;
        protected Type physicsAdapterType;
        private bool physicsAdapterChangeRequested;
        private object[] physicsAdapterParameters;
        protected BaseObject parent;
        protected Vector3 relativePosition;
        bool isInitialized = false;
        #endregion

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
                OnPositionChanged(this, null);
            }
        }

        public virtual Vector3 EulerRotation
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

        public bool IsInitialized
        {
            get { return isInitialized; }
            set { isInitialized = value; }
        }

        public virtual Vector3 Position
        {
            get { return position; }
            set
            {
                position = value;
                if (parent != null)
                {
                    Vector3 v = position - parent.Position;
                    relativePosition = Vector3.Transform(v, Matrix.Invert(Matrix.CreateFromQuaternion(rotation)));
                }
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

        public bool PhysicsEnabled
        {
            get { return physicsEnabled; }
            set
            {
                physicsEnabled = value;
            }
        }

        public PhysicsAdapter PhysicsAdapter
        {
            get
            {
                return physicsAdapter;
            }
        }

        public Vector3 RelativePosition
        {
            get { return relativePosition; }
            set
            {
                relativePosition = value;
                if (parent != null)
                {
                    Vector3 v = Vector3.Transform(relativePosition, Matrix.CreateFromQuaternion(rotation));
                    position = parent.Position + v;
                    OnPositionChanged(this, null);
                }
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
            physicsEnabled = false;
        }

        public BaseObject(Game game, World physicsWorld)
            : this(game)
        {
            this.physicsWorld = physicsWorld;
            if (!(this is PhysicsAdapter))
            {
                physicsAdapter = new BoxAdapter(game, this, physicsWorld, Vector3.Zero, Vector3.One);
                physicsAdapterType = typeof(BoxAdapter);
                physicsAdapterChangeRequested = false;
            }
            OnRotationChanged(this, null);
        }

        public override void Initialize()
        {
            base.Initialize();

            foreach (ControllerScript script in scripts)
            {
                script.Start();
            }

            isInitialized = true;
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            if (physicsAdapterChangeRequested)
            {
                bool reenableCharacterController = false;
                if (physicsAdapter != null && physicsAdapter.CharacterController != null)
                    reenableCharacterController = true;
                physicsAdapter = (PhysicsAdapter)Activator.CreateInstance(physicsAdapterType, physicsAdapterParameters);
                if (reenableCharacterController)
                    physicsAdapter.EnableCharacterController();
                physicsAdapterChangeRequested = false;
            }

            foreach (ControllerScript script in scripts)
            {
                script.Update(gameTime);
            }

            if (physicsAdapter != null)
            {
                if (physicsEnabled)
                {
                    if (!physicsWorld.RigidBodies.Contains(physicsAdapter.Body))
                        physicsWorld.AddBody(physicsAdapter.Body);

                    physicsAdapter.Update(gameTime);
                    Vector3 v = Vector3.Transform(physicsAdapter.RelativePosition, Matrix.CreateFromQuaternion(physicsAdapter.Rotation));
                    position = physicsAdapter.Position - v;
                    rotation = physicsAdapter.Rotation;

                    OnRotationChanged(this, new BodyGeoEventArgs(false));
                    OnPositionChanged(this, new BodyGeoEventArgs(false));
                }
                else
                {
                    if (physicsWorld.RigidBodies.Contains(physicsAdapter.Body))
                        physicsWorld.RemoveBody(physicsAdapter.Body);
                }
            }

            foreach (ControllerScript script in scripts)
            {
                script.LateUpdate();
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
            Helper.QuaternionToEuler(rotation, out rotationX, out rotationY, out rotationZ);
            eulerRotation = new Vector3(rotationX, rotationY, rotationZ);
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

        public void ChangePhysicsAdapter(object[] parameters)
        {
            physicsWorld.RemoveBody(physicsAdapter.Body);
            physicsAdapterChangeRequested = true;
            this.physicsAdapterParameters = parameters;
        }

        public void ChangePhysicsAdapter(Type physicsAdapterType, object[] parameters)
        {
            this.physicsAdapterType = physicsAdapterType;
            ChangePhysicsAdapter(parameters);
        }

        protected virtual void OnRotationChanged(object sender, EventArgs e)
        {
            direction = new Vector3(0, 0, 1);
            direction = Vector3.Transform(direction, Matrix.CreateFromQuaternion(rotation));
            direction.Normalize();

            if (RotationChanged != null)
                RotationChanged(sender, e);

            if (physicsAdapter != null && e == null)
                physicsAdapter.Rotation = rotation;

            if (this is PhysicsAdapter &&
                (e == null || (e != null && e is BodyGeoEventArgs && (e as BodyGeoEventArgs).SetBodyGeo)) &&
                ((PhysicsAdapter)this).Body != null)
                ((PhysicsAdapter)this).Body.Orientation = Helper.ToJitterMatrix(Matrix.CreateFromQuaternion(rotation));
        }

        protected virtual void OnPositionChanged(object sender, EventArgs e)
        {
            if (PositionChanged != null)
                PositionChanged(sender, e);

            if (physicsAdapter != null && e == null)
                physicsAdapter.RelativePosition = physicsAdapter.RelativePosition;

            if (this is PhysicsAdapter &&
                (e == null || (e != null && e is BodyGeoEventArgs && (e as BodyGeoEventArgs).SetBodyGeo)) &&
                ((PhysicsAdapter)this).Body != null)
                ((PhysicsAdapter)this).Body.Position = Helper.ToJitterVector(position);
        }

        public virtual void CollisionDetected(BaseObject other)
        {
            foreach (ControllerScript script in scripts)
            {
                script.CollisionDetected(other);
            }
        }
    }
}