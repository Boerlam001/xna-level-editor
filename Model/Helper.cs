﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Jitter.LinearMath;

namespace EditorModel
{
    public class Helper
    {
        public static float ArcTanAngle(float X, float Y)
        {
            if (X == 0)
            {
                if (Y == 1)
                    return (float)MathHelper.PiOver2;
                else
                    return (float)-MathHelper.PiOver2;
            }
            else if (X > 0)
                return (float)Math.Atan(Y / X);
            else if (X < 0)
            {
                if (Y > 0)
                    return (float)Math.Atan(Y / X) + MathHelper.Pi;
                else
                    return (float)Math.Atan(Y / X) - MathHelper.Pi;
            }
            else
                return 0;
        }

        public static Vector3 AngleTo(Vector3 from, Vector3 location)
        {
            Vector3 angle = new Vector3();
            Vector3 v3 = Vector3.Normalize(location - from);
            angle.X = (float)Math.Asin(v3.Y);
            angle.Y = ArcTanAngle(-v3.Z, -v3.X);
            return angle;
        }

        public static void QuaternionToEuler(Quaternion rotation, out float eulerX, out float eulerY, out float eulerZ)
        {
            Vector3 rotationaxes = new Vector3();

            Vector3 forward = Vector3.Transform(Vector3.Forward, rotation);
            Vector3 up = Vector3.Transform(Vector3.Up, rotation);
            rotationaxes = AngleTo(new Vector3(), forward);
            if (rotationaxes.X == MathHelper.PiOver2)
            {
                rotationaxes.Y = ArcTanAngle(up.Z, up.X);
                rotationaxes.Z = 0;
            }
            else if (rotationaxes.X == -MathHelper.PiOver2)
            {
                rotationaxes.Y = ArcTanAngle(-up.Z, -up.X);
                rotationaxes.Z = 0;
            }
            else
            {
                up = Vector3.Transform(up, Matrix.CreateRotationY(-rotationaxes.Y));
                up = Vector3.Transform(up, Matrix.CreateRotationX(-rotationaxes.X));
                rotationaxes.Z = ArcTanAngle(up.Y, -up.X);
            }
            eulerX = MathHelper.ToDegrees(rotationaxes.X);
            eulerY = MathHelper.ToDegrees(rotationaxes.Y);
            eulerZ = MathHelper.ToDegrees(rotationaxes.Z);
        }

        public static void Pointing(Viewport viewport, Camera camera, float mouseX, float mouseY, out Vector3 nearPoint, out Vector3 direction)
        {
            Vector3 nearsource = new Vector3(mouseX, mouseY, 0);
            Vector3 farsource = new Vector3(mouseX, mouseY, 1);

            Matrix world = Matrix.CreateTranslation(0, 0, 0);

            nearPoint = viewport.Unproject(nearsource, camera.Projection, camera.World, world);
            Vector3 farPoint = viewport.Unproject(farsource, camera.Projection, camera.World, world);

            direction = farPoint - nearPoint;

            direction.Normalize();
        }

        public static Vector3 Put(Viewport viewport, Camera camera, float mouseX, float mouseY, float dist)
        {
            Vector3 nearPoint, direction;
            Helper.Pointing(viewport, camera, mouseX, mouseY, out nearPoint, out direction);
            return nearPoint + dist * direction;
        }

        public static Ray Pick(Viewport viewport, Camera camera, float mouseX, float mouseY)
        {
            Vector3 nearPoint, direction;
            Helper.Pointing(viewport, camera, mouseX, mouseY, out nearPoint, out direction);
            return new Ray(nearPoint, direction);
        }

        public static Vector2 ScreenCoords(Vector3 v, Camera camera)
        {
            Matrix viewProj = camera.World * camera.Projection;
            
            Vector4 position = new Vector4(v, 1);
            Vector4.Transform(ref position, ref viewProj, out position);
            position /= position.W;

            return new Vector2(position.X, position.Y);

            //float
            //    w = viewProj.M14 * v.X + viewProj.M24 * v.Y + viewProj.M34 * v.Z + viewProj.M44;
            //return new Vector2(
            //    (viewProj.M11 * v.X + viewProj.M21 * v.Y + viewProj.M31 * v.Z + viewProj.M41) / w,
            //    (viewProj.M12 * v.X + viewProj.M22 * v.Y + viewProj.M32 * v.Z + viewProj.M42) / w);
        }

        //reference: http://graphicdna.blogspot.com/2013/01/projecting-3d-vector-into-2d-screen.html
        public static Vector2 ProjectAndClipToViewport(Vector3 pVector, float pX, float pY,
                                float pWidth, float pHeight, float pMinZ, float pMaxZ,
                                Matrix pWorldViewProjection, out bool pWasInsideScreen)
        {
            // First, multiply by worldViewProj, to get the coordinates in projection space
            Vector4 vProjected = Vector4.Zero;
            Vector4.Transform(ref pVector, ref pWorldViewProjection, out vProjected);

            // Secondly (OPTIONAL STEP), multiply by the clipMatrix, if you want to scale
            // or shift the clip volume. If not (most of the times you won´t), just leave 
            // this part commented,
            // or set an Identity Matrix as the clip matrix. The default clip volume parameters
            // (see below), will produce an identity clip matrix.
            //float clipWidth = 2;
            //float clipHeight = 2;
            //float clipX = -1;
            //float clipY = 1;
            //float clipMinZ = 0;
            //float clipMaxZ = 1;
            //Matrix mclip = new Matrix();
            //mclip.M11 = 2f / clipWidth;
            //mclip.M12 = 0f;
            //mclip.M13 = 0f;
            //mclip.M14 = 0f;
            //mclip.M21 = 0f;
            //mclip.M22 = 2f / clipHeight;
            //mclip.M23 = 0f;
            //mclip.M24 = 0f;
            //mclip.M31 = 0f;
            //mclip.M32 = 0;
            //mclip.M33 = 1f / (clipMaxZ - clipMinZ);
            //mclip.M34 = 0f;
            //mclip.M41 = -1 -2 * (clipX / clipWidth);
            //mclip.M42 = 1 - 2 * (clipY / clipHeight);
            //mclip.M43 = -clipMinZ / (clipMaxZ - clipMinZ);
            //mclip.M44 = 1f;
            //vProjected = Vector4.Transform(vProjected, mclip);

            // Third: Once we have coordinates in clip space, perform the clipping,
            // to leave the coordinates inside the screen. The clip volume is defined by:
            //
            //  -Wp < Xp <= Wp
            //  -Wp < Yp <= Wp
            //  0 < Zp <= Wp
            //
            // If any clipping is needed, then the point was out of the screen.
            pWasInsideScreen = true;
            if (vProjected.X < -vProjected.W)
            {
                vProjected.X = -vProjected.W;
                pWasInsideScreen = false;
            }
            if (vProjected.X > vProjected.W)
            {
                vProjected.X = vProjected.W;
                pWasInsideScreen = false;
            }
            if (vProjected.Y < -vProjected.W)
            {
                vProjected.Y = -vProjected.W;
                pWasInsideScreen = false;
            }
            if (vProjected.Y > vProjected.W)
            {
                vProjected.Y = vProjected.W;
                pWasInsideScreen = false;
            }
            if (vProjected.Z < 0)
            {
                vProjected.Z = 0;
                pWasInsideScreen = false;
            }
            if (vProjected.Z > vProjected.W)
            {
                vProjected.Z = vProjected.W;
                pWasInsideScreen = false;
            }

            // Fourth step: Divide by w, to move from homogeneous coordinates to 3D
            // coordinates again
            vProjected.X = vProjected.X / vProjected.W;
            vProjected.Y = vProjected.Y / vProjected.W;
            vProjected.Z = vProjected.Z / vProjected.W;

            // Last step: Perform the viewport scaling, to get the appropiate coordinates
            // inside the viewport
            vProjected.X = ((float)(((vProjected.X + 1.0) * 0.5) * pWidth)) + pX;
            vProjected.Y = ((float)(((1.0 - vProjected.Y) * 0.5) * pHeight)) + pY;
            vProjected.Z = (vProjected.Z * (pMaxZ - pMinZ)) + pMinZ;

            // Return pixel coordinates as 2D (change this to 3D if you need Z)
            return new Vector2(vProjected.X, vProjected.Y);
        }

        // reference http://ghoshehsoft.wordpress.com/2010/12/09/xna-picking-tutorial-part-ii/
        public static BoundingFrustum UnprojectRectangle(Rectangle source, Viewport viewport, Matrix projection, Matrix view)
        {
            //http://forums.create.msdn.com/forums/p/6690/35401.aspx , by "The Friggm"
            // Many many thanks to him...

            // Point in screen space of the center of the region selected
            Vector2 regionCenterScreen = new Vector2(source.Center.X, source.Center.Y);

            // Generate the projection matrix for the screen region
            Matrix regionProjMatrix = projection;

            // Calculate the region dimensions in the projection matrix. M11 is inverse of width, M22 is inverse of height.
            regionProjMatrix.M11 /= ((float)source.Width / (float)viewport.Width);
            regionProjMatrix.M22 /= ((float)source.Height / (float)viewport.Height);

            // Calculate the region center in the projection matrix. M31 is horizonatal center.
            regionProjMatrix.M31 = (regionCenterScreen.X - (viewport.Width / 2f)) / ((float)source.Width / 2f);

            // M32 is vertical center. Notice that the screen has low Y on top, projection has low Y on bottom.
            regionProjMatrix.M32 = -(regionCenterScreen.Y - (viewport.Height / 2f)) / ((float)source.Height / 2f);

            return new BoundingFrustum(view * regionProjMatrix);
        }

        public static JVector ToJitterVector(Vector3 vector)
        {
            return new JVector(vector.X, vector.Y, vector.Z);
        }

        public static Matrix ToXNAMatrix(JMatrix matrix)
        {
            return new Matrix(matrix.M11,
                            matrix.M12,
                            matrix.M13,
                            0.0f,
                            matrix.M21,
                            matrix.M22,
                            matrix.M23,
                            0.0f,
                            matrix.M31,
                            matrix.M32,
                            matrix.M33,
                            0.0f, 0.0f, 0.0f, 0.0f, 1.0f);
        }

        public static JMatrix ToJitterMatrix(Matrix matrix)
        {
            JMatrix result;
            result.M11 = matrix.M11;
            result.M12 = matrix.M12;
            result.M13 = matrix.M13;
            result.M21 = matrix.M21;
            result.M22 = matrix.M22;
            result.M23 = matrix.M23;
            result.M31 = matrix.M31;
            result.M32 = matrix.M32;
            result.M33 = matrix.M33;
            return result;

        }


        public static Vector3 ToXNAVector(JVector vector)
        {
            return new Vector3(vector.X, vector.Y, vector.Z);
        }
    }
}
