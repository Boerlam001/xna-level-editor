﻿using System;
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
            set
            {
                zoom = value;
                if (zoom < 1) zoom = 1;
                if (isOrthographic)
                    CalculateProjection();
            }
        }

        public Camera(GraphicsDevice graphicsDevice) : base()
        {
            name = "camera";
            fieldOfViewAngle = MathHelper.PiOver4;// MathHelper.ToRadians(45);
            nearPlaneDistance = 1f;
            farPlaneDistance = 2000f;
            aspectRatio = 16 / 9;
            isOrthographic = false;
            this.GraphicsDevice = graphicsDevice;
            CalculateProjection();
            isMoving = false;
            zoom = 100f;
        }

        private void CalculateProjection()
        {
            if (!isOrthographic)
                projection = Matrix.CreatePerspectiveFieldOfView(fieldOfViewAngle, aspectRatio, nearPlaneDistance, farPlaneDistance);
            else
                //projection = Matrix.CreateOrthographicOffCenter(-zoom / 2, zoom, -zoom / 2, zoom / 2, nearPlaneDistance, farPlaneDistance);
                projection = Matrix.CreateOrthographic(zoom, zoom / aspectRatio, -farPlaneDistance, farPlaneDistance);
                //projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(1), aspectRatio, nearPlaneDistance, farPlaneDistance);

            LookAt();
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
            Vector3 up = Vector3.Transform(Vector3.Up, rotationMatrix);
            world = Matrix.CreateScale(scale) * Matrix.CreateLookAt(position, lookAtVector, up);
            eulerRotation = new Vector3(rotationX, rotationY, rotationZ);
            Vector3 s, t;
            rotationMatrix.Decompose(out s, out rotation, out t);
            OnRotationChanged(this, null);
        }
    }
}
