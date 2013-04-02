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
            fieldOfViewAngle = MathHelper.ToRadians(45);
            nearPlaneDistance = 0.01f;
            farPlaneDistance = 100f;
            aspectRatio = 16 / 9;
            projection = Matrix.CreatePerspectiveFieldOfView(fieldOfViewAngle, aspectRatio, nearPlaneDistance, farPlaneDistance);
        }

        public void LookAt(Vector3 target)
        {
            world = Matrix.CreateLookAt(position, target, Vector3.Up);
            world.Decompose(out scale, out rotation, out position);
        }
    }
}
