using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace EditorModel
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
                isMoving = true;
                LookAt();
            }
        }
        
        public override Quaternion Rotation
        {
            get { return rotation; }
            set
            {                
                rotation = value;
                isMoving = true;
                Helper.QuaternionToEuler(rotation, out rotationX, out rotationY, out rotationZ);
                LookAt();
                OnRotationChanged(this, null);
            }
        }

        public override float RotationX
        {
            get { return rotationX; }
            set
            {
                rotationX = value % 360;
                isMoving = true;
                if (rotationX < 0)
                    rotationX = 360 + rotationX;
                //if (rotationX > 90 && rotationX < 180)
                //    rotationX = 90;
                //if (rotationX >= 180 && rotationX < 270)
                //    rotationX = 270;
                LookAt();
                OnRotationChanged(this, null);
            }
        }

        public override float RotationY
        {
            get { return rotationY; }
            set
            {
                rotationY = value % 360;
                isMoving = true;
                if (rotationY < 0)
                    rotationY = 360 + rotationY;
                LookAt();
                OnRotationChanged(this, null);
            }
        }

        public override float RotationZ
        {
            get { return rotationZ; }
            set
            {
                rotationZ = value % 360;
                isMoving = true;
                if (rotationZ < 0)
                    rotationZ = 360 + rotationZ;
                LookAt();
                OnRotationChanged(this, null);
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

        public Camera()
        {
            name = "camera";
            fieldOfViewAngle = MathHelper.PiOver4;// MathHelper.ToRadians(45);
            nearPlaneDistance = 1f;
            farPlaneDistance = 2000f;
            aspectRatio = 16 / 9;
            projection = Matrix.CreatePerspectiveFieldOfView(fieldOfViewAngle, aspectRatio, nearPlaneDistance, farPlaneDistance);
            isMoving = false;
        }

        public override void Rotate(float x, float y, float z)
        {
            rotationY += y;
            rotationX += x;
            rotationZ += z;
            isMoving = true;
            LookAt();
        }

        private void LookAt()
        {
            Matrix rotationMatrix = Matrix.CreateFromYawPitchRoll(MathHelper.ToRadians(rotationY), MathHelper.ToRadians(rotationX), MathHelper.ToRadians(rotationZ));
            Vector3 transformedReference = Vector3.Transform(rotationReference, rotationMatrix);
            Vector3 lookAtVector = position + transformedReference;
            world = Matrix.CreateScale(scale) * Matrix.CreateLookAt(position, lookAtVector, Vector3.Up);
            Vector3 s, t;
            rotationMatrix.Decompose(out s, out rotation, out t);
        }
    }
}
