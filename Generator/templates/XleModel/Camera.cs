using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace XleModel
{
    public class Camera : BaseObject
    {
        protected float fieldOfViewAngle;
        protected float nearPlaneDistance;
        protected float farPlaneDistance;
        protected Matrix projection;
        protected float aspectRatio;

        public override Vector3 Position
        {
            get { return position; }
            set
            {
                position = value;
                LookAt();
            }
        }
        
        public override Quaternion Rotation
        {
            get { return rotation; }
            set
            {
                
                rotation = value;
                Helper.QuaternionToEuler(rotation, out rotationX, out rotationY, out rotationZ);
                LookAt();
            }
        }

        public override Vector3 EulerRotation
        {
            get
            {
                return base.EulerRotation;
            }
            set
            {
                RotationX = value.X;
                RotationY = value.Y;
                RotationZ = value.Z;
            }
        }

        public override float RotationX
        {
            get { return rotationX; }
            set
            {
                rotationX = value % 360;
                if (rotationX < 0)
                    rotationX = 360 + rotationX;
                //if (rotationX > 90 && rotationX < 180)
                //    rotationX = 90;
                //if (rotationX >= 180 && rotationX < 270)
                //    rotationX = 270;
                LookAt();
            }
        }

        public override float RotationY
        {
            get { return rotationY; }
            set
            {
                rotationY = value % 360;
                if (rotationY < 0)
                    rotationY = 360 + rotationY;
                LookAt();
            }
        }

        public override float RotationZ
        {
            get { return rotationZ; }
            set
            {
                rotationZ = value % 360;
                if (rotationZ < 0)
                    rotationZ = 360 + rotationZ;
                LookAt();
            }
        }

        public float FieldOfViewAngle
        {
            get { return fieldOfViewAngle; }
            set
            {
                fieldOfViewAngle = value;
                projection = Matrix.CreatePerspectiveFieldOfView(fieldOfViewAngle, aspectRatio, nearPlaneDistance, farPlaneDistance);
            }
        }

        public float NearPlaneDistance
        {
            get { return nearPlaneDistance; }
            set
            {
                nearPlaneDistance = value;
                projection = Matrix.CreatePerspectiveFieldOfView(fieldOfViewAngle, aspectRatio, nearPlaneDistance, farPlaneDistance);
            }
        }

        public float FarPlaneDistance
        {
            get { return farPlaneDistance; }
            set
            {
                farPlaneDistance = value;
                projection = Matrix.CreatePerspectiveFieldOfView(fieldOfViewAngle, aspectRatio, nearPlaneDistance, farPlaneDistance);
            }
        }

        public float AspectRatio
        {
            get { return aspectRatio; }
            set
            {
                aspectRatio = value;
                projection = Matrix.CreatePerspectiveFieldOfView(fieldOfViewAngle, aspectRatio, nearPlaneDistance, farPlaneDistance);
            }
        }

        public Matrix Projection
        {
            get { return projection; }
        }

        public Camera(Game game) : base(game)
        {
            name = "camera";
            fieldOfViewAngle = MathHelper.ToRadians(45);
            nearPlaneDistance = 0.01f;
            farPlaneDistance = 2000f;
            aspectRatio = 16 / 9;
            projection = Matrix.CreatePerspectiveFieldOfView(fieldOfViewAngle, aspectRatio, nearPlaneDistance, farPlaneDistance);
        }

        public override void Initialize()
        {
            base.Initialize();
            AspectRatio = GraphicsDevice.Viewport.AspectRatio;
        }

        public override void Rotate(float x, float y, float z)
        {
            rotationY += y;
            rotationX += x;
            rotationZ += z;
            LookAt();
        }

        public void LookAt()
        {
            Matrix rotationMatrix = Matrix.CreateFromYawPitchRoll(MathHelper.ToRadians(rotationY), MathHelper.ToRadians(rotationX), MathHelper.ToRadians(rotationZ));
            Vector3 transformedReference = Vector3.Transform(rotationReference, rotationMatrix);
            Vector3 lookAtVector = position + transformedReference;
            world = Matrix.CreateLookAt(position, lookAtVector, Vector3.Up);
            Vector3 s, t;
            rotationMatrix.Decompose(out s, out rotation, out t);
            OnRotationChanged(this, null);
        }
    }
}
