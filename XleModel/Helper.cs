using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

namespace XleModel
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
            eulerX = rotationaxes.X;
            eulerY = rotationaxes.Y;
            eulerZ = rotationaxes.Z;
        }

        public static void Pointing(GraphicsDevice graphicsDevice, Camera camera, float mouseX, float mouseY, out Vector3 nearPoint, out Vector3 direction)
        {
            Vector3 nearsource = new Vector3(mouseX, mouseY, 0);
            Vector3 farsource = new Vector3(mouseX, mouseY, 1);

            Matrix world = Matrix.CreateTranslation(0, 0, 0);

            nearPoint = graphicsDevice.Viewport.Unproject(nearsource, camera.Projection, camera.World, world);
            Vector3 farPoint = graphicsDevice.Viewport.Unproject(farsource, camera.Projection, camera.World, world);

            direction = farPoint - nearPoint;

            direction.Normalize();
        }

        public static Vector3 Put(GraphicsDevice graphicsDevice, Camera camera, float mouseX, float mouseY, float dist)
        {
            Vector3 nearPoint, direction;
            Helper.Pointing(graphicsDevice, camera, mouseX, mouseY, out nearPoint, out direction);
            return nearPoint + dist * direction;
        }

        public static Ray Pick(GraphicsDevice graphicsDevice, Camera camera, float mouseX, float mouseY)
        {
            Vector3 nearPoint, direction;
            Helper.Pointing(graphicsDevice, camera, mouseX, mouseY, out nearPoint, out direction);
            return new Ray(nearPoint, direction);
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
    }
}
