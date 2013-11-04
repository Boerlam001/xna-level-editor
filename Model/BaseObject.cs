using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.ComponentModel.Design;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using EditorModel.PropertyModel;
using Jitter;
using Jitter.Collision;
using Jitter.Dynamics;
using Jitter.Collision.Shapes;
using Jitter.LinearMath;

namespace EditorModel
{
    public enum PhysicsShapeKind
    {
        BoxShape,
        ConvexHullShape,
        CapsuleShape
    }

    public class BaseObject : Subject
    {
        #region attributes
        public delegate void RotationChangedEventHandler(object sender, EventArgs e);
        public RotationChangedEventHandler RotationChanged;

        public delegate void PositionChangedEventHandler(object sender, EventArgs e);
        public RotationChangedEventHandler PositionChanged;

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
        private ScriptCollection scripts;
        PhysicsShapeKind physicsShapeKind;
        protected Shape physicsShape;
        RigidBody body;
        Vector3 bodyPosition;
        bool characterControllerEnabled;
        bool physicsEnabled;
        private GraphicsDevice graphicsDevice;
        private Camera camera;
        #endregion

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
                if (physicsShapeKind == EditorModel.PhysicsShapeKind.ConvexHullShape)
                {
                    PhysicsShapeKind = PhysicsShapeKind.ConvexHullShape;
                }
                OnPositionChanged(this, null);
            }
        }

        [Category("Transform")]
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
        [TypeConverter(typeof(ExpandableObjectConverter))]
        [EditorAttribute(typeof(ItemCollectionEditor), typeof(System.Drawing.Design.UITypeEditor))]
        public ScriptCollection Scripts
        {
            get { return scripts; }
            set { scripts = value; }
        }

        [Category("Body")]
        [TypeConverter(typeof(ExpandableObjectConverter))]
        public Shape PhysicsShape
        {
            get { return physicsShape; }
            set
            {
                physicsShape = value;

                if (physicsShape is ConvexHullShape)
                {
                    physicsShapeKind = EditorModel.PhysicsShapeKind.ConvexHullShape;
                }
                else if (physicsShape is BoxShape)
                {
                    physicsShapeKind = EditorModel.PhysicsShapeKind.BoxShape;
                }
                else if (physicsShape is CapsuleShape)
                {
                    physicsShapeKind = EditorModel.PhysicsShapeKind.CapsuleShape;
                }

                body = new RigidBody(physicsShape);
                body.Orientation = Helper.ToJitterMatrix(Matrix.CreateFromQuaternion(rotation));
                Vector3 v = Vector3.Transform(bodyPosition, Matrix.CreateFromQuaternion(rotation));
                body.Position = Helper.ToJitterVector(position + v);
            }
        }

        public Vector3 BoxShapeSize
        {
            get
            {
                if (physicsShape is BoxShape)
                    return Helper.ToXNAVector((physicsShape as BoxShape).Size);
                return Vector3.Zero;
            }
            set
            {
                if (physicsShape is BoxShape)
                    (physicsShape as BoxShape).Size = Helper.ToJitterVector(value);
            }
        }

        [Category("Body")]
        public PhysicsShapeKind PhysicsShapeKind
        {
            get { return physicsShapeKind; }
            set
            {
                if (!(this is DrawingObject) && value == PhysicsShapeKind.ConvexHullShape)
                    return;

                physicsShapeKind = value;
                if (value == EditorModel.PhysicsShapeKind.BoxShape)
                    physicsShape = new BoxShape(JVector.One);
                else if (value == EditorModel.PhysicsShapeKind.CapsuleShape)
                    physicsShape = new CapsuleShape(1, 1);
                else if (value == EditorModel.PhysicsShapeKind.ConvexHullShape)
                {
                    DrawingObject obj = this as DrawingObject;
                    if (obj.DrawingModel == null)
                        return;
                    BoundingBox parentBoundingBox = obj.CreateBoundingBox();
                    Vector3 parentShift = scale * obj.Center - new Vector3(0, parentBoundingBox.Min.Y, 0);
                    physicsShape = ConvexHullHelper.BuildConvexHullShape(obj.DrawingModel, scale);
                    //bodyPosition = -Helper.ToXNAVector(((ConvexHullShape)physicsShape).Shift) - parentShift;
                }

                body = new RigidBody(physicsShape);
                body.Orientation = Helper.ToJitterMatrix(Matrix.CreateFromQuaternion(rotation));
                Vector3 v = Vector3.Transform(bodyPosition, Matrix.CreateFromQuaternion(rotation));
                body.Position = Helper.ToJitterVector(position + v);
            }
        }

        [Browsable(false)]
        public RigidBody Body
        {
            get { return body; }
        }

        [Category("Body")]
        public Vector3 BodyPosition
        {
            get { return bodyPosition; }
            set
            {
                bodyPosition = value;
                Vector3 v = Vector3.Transform(bodyPosition, Matrix.CreateFromQuaternion(rotation));
                body.Position = Helper.ToJitterVector(position + v);
            }
        }

        [Category("Body")]
        public bool CharacterControllerEnabled
        {
            get { return characterControllerEnabled; }
            set
            {
                characterControllerEnabled = value;
                if (characterControllerEnabled)
                    isStatic = false;
            }
        }

        [Category("Body")]
        public bool PhysicsEnabled
        {
            get { return physicsEnabled; }
            set
            {
                physicsEnabled = value;
                if (physicsEnabled == false)
                    characterControllerEnabled = false;
            }
        }

        [Browsable(false)]
        public GraphicsDevice GraphicsDevice
        {
            get { return graphicsDevice; }
            set { graphicsDevice = value; }
        }

        [Browsable(false)]
        public virtual Camera Camera
        {
            get { return camera; }
            set { camera = value; }
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

            scripts = new ScriptCollection();

            PhysicsShapeKind = EditorModel.PhysicsShapeKind.BoxShape;

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

        public virtual void Rotate(Vector3 eulerRotation)
        {
            Rotate(eulerRotation.X, eulerRotation.Y, eulerRotation.Z);
        }

        public void LookAt(Vector3 target)
        {
            world = Matrix.CreateLookAt(position, target, Vector3.Up);
            world.Decompose(out scale, out rotation, out position);

            OnRotationChanged(this, null);
        }

        public void MoveForward(float speed)
        {
            MoveRelative(Vector3.UnitZ, speed);
        }

        public void MoveRight(float speed)
        {
            MoveRelative(Vector3.UnitX, -speed);
        }

        public void MoveTop(float speed)
        {
            MoveRelative(Vector3.UnitY, speed);
        }

        public void MoveTopRight(Vector2 speed)
        {
            Vector3 rightSpeed = Vector3.UnitX * speed.X;
            Vector3 topSpeed = Vector3.UnitY * speed.Y;
            Vector3 v = Vector3.Transform(rightSpeed + topSpeed, Matrix.CreateFromQuaternion(rotation));
            Move(v);
        }

        public void MoveRelative(Vector3 direction, float speed)
        {
            Vector3 v = direction * speed;
            v = Vector3.Transform(v, Matrix.CreateFromQuaternion(rotation));
            Move(v);
        }

        public void Move(Vector3 speed)
        {
            Position += speed;
        }

        public void Move(Vector3 speed, Vector3 angularSpeed)
        {
            Move(speed);
            EulerRotation += angularSpeed;
        }

        public virtual bool RayIntersects(Ray ray, float mouseX, float mouseY)
        {
            return false;
        }

        public virtual void Draw(SpriteBatch spriteBatch, bool lightDirectionEnabled = false, Vector3 lightDirection = new Vector3(), bool alpha = false)
        {
        }

        public virtual void DrawSprite(SpriteBatch spriteBatch)
        {
        }

        protected virtual void OnRotationChanged(object sender, EventArgs e)
        {
            direction = new Vector3(0, 0, 1);
            direction = Vector3.Transform(direction, Matrix.CreateFromQuaternion(rotation));
            direction.Normalize();

            if (body != null)
                body.Orientation = Helper.ToJitterMatrix(Matrix.CreateFromQuaternion(rotation));

            if (RotationChanged != null)
                RotationChanged(sender, e);
        }

        protected virtual void OnPositionChanged(object sender, EventArgs e)
        {
            if (body != null)
                body.Position = Helper.ToJitterVector(position);

            if (PositionChanged != null)
                PositionChanged(sender, e);
        }
    }
}