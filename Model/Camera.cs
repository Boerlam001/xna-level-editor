using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace EditorModel
{
    public class Camera : BaseObject
    {
        protected float fieldOfViewAngle;
        protected float nearPlaneDistance;
        protected float farPlaneDistance;
        protected Matrix projection;
        protected float aspectRatio;
        private bool isOrthographic;
        private GraphicsDevice graphicsDevice;
        private float zoom;

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
                CalculateProjection();
            }
        }

        public float NearPlaneDistance
        {
            get { return nearPlaneDistance; }
            set
            {
                nearPlaneDistance = value;
                CalculateProjection();
            }
        }

        public float FarPlaneDistance
        {
            get { return farPlaneDistance; }
            set
            {
                farPlaneDistance = value;
                CalculateProjection();
            }
        }

        public float AspectRatio
        {
            get { return aspectRatio; }
            set
            {
                aspectRatio = value;
                CalculateProjection();
            }
        }

        public Matrix Projection
        {
            get { return projection; }
        }

        public bool IsOrthographic
        {
            get { return isOrthographic; }
            set
            {
                isOrthographic = value;
                CalculateProjection();
            }
        }

        public float Zoom
        {
            get { return zoom; }
            set { zoom = value; }
        }

        public Camera(GraphicsDevice graphicsDevice) : base()
        {
            name = "camera";
            fieldOfViewAngle = MathHelper.PiOver4;// MathHelper.ToRadians(45);
            nearPlaneDistance = 1f;
            farPlaneDistance = 2000f;
            aspectRatio = 16 / 9;
            isOrthographic = false;
            this.graphicsDevice = graphicsDevice;
            CalculateProjection();
            isMoving = false;
            zoom = 100f;
        }

        private void CalculateProjection()
        {
            if (!isOrthographic)
                projection = Matrix.CreatePerspectiveFieldOfView(fieldOfViewAngle, aspectRatio, nearPlaneDistance, farPlaneDistance);
            else
                projection = Matrix.CreateOrthographicOffCenter(-zoom / 2, zoom, -zoom / 2, zoom / 2, nearPlaneDistance, farPlaneDistance);
        }

        public override void Rotate(float x, float y, float z)
        {
            rotationY += y;
            rotationX += x;
            rotationZ += z;
            eulerRotation = new Vector3(rotationX, rotationY, rotationZ);
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
            OnRotationChanged(this, null);
        }
    }
}
