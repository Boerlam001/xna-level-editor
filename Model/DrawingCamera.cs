using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace EditorModel
{
    public class DrawingCamera : BaseObject, IObserver
    {
        Texture2D texture;
        Vector2 origin;
        float textureScale = 1;
        float drawingScale = 0.5f;
        private Vector2 screenPos;
        float layerDepth = 0;
        private Vector3 screenPos3;
        BasicEffect basicEffect;
        VertexPositionColor[] vertices;
        short[] indices;
        MapModel mapModel;

        public Texture2D Texture
        {
            get { return texture; }
            set
            {
                texture = value;
                textureScale = 50f / texture.Width;
                origin = new Vector2(texture.Width / 2, texture.Height / 2);
            }
        }

        public float LayerDepth
        {
            get { return layerDepth; }
            set { layerDepth = value; }
        }

        public BasicEffect BasicEffect
        {
            get { return basicEffect; }
            set { basicEffect = value; }
        }

        public MapModel MapModel
        {
            get { return mapModel; }
            set { mapModel = value; }
        }

        public override Camera Camera
        {
            get
            {
                return base.Camera;
            }
            set
            {
                if (base.Camera != null)
                    base.Camera.Detach(this);
                base.Camera = value;
                if (value != null)
                    value.Attach(this);
            }
        }

        public float DrawingScale
        {
            get { return drawingScale; }
            set
            {
                drawingScale = value;
                vertices = new VertexPositionColor[5]
                {
                    new VertexPositionColor(Vector3.Zero, Color.LightBlue),
                    new VertexPositionColor(new Vector3(-1, 1, 1)  * drawingScale,  Color.LightBlue),
                    new VertexPositionColor(new Vector3(1, 1, 1)   * drawingScale,   Color.LightBlue),
                    new VertexPositionColor(new Vector3(1, -1, 1)  * drawingScale,  Color.LightBlue),
                    new VertexPositionColor(new Vector3(-1, -1, 1) * drawingScale, Color.LightBlue)
                };
            }
        }

        public DrawingCamera()
            : base()
        {
            vertices = new VertexPositionColor[5]
            {
                new VertexPositionColor(Vector3.Zero, Color.LightBlue),
                new VertexPositionColor(new Vector3(-1, 1, 1)  * drawingScale,  Color.LightBlue),
                new VertexPositionColor(new Vector3(1, 1, 1)   * drawingScale,   Color.LightBlue),
                new VertexPositionColor(new Vector3(1, -1, 1)  * drawingScale,  Color.LightBlue),
                new VertexPositionColor(new Vector3(-1, -1, 1) * drawingScale, Color.LightBlue)
            };
            indices = new short[]
            {
                0, 1, 0, 2, 0, 3, 0, 4, 1, 2, 2, 3, 3, 4, 4, 1
            };
        }

        public void UpdateObserver()
        {
            screenPos3 = GraphicsDevice.Viewport.Project(position, Camera.Projection, Camera.World, Matrix.Identity);
            screenPos = new Vector2(screenPos3.X, screenPos3.Y);
            if (Camera.IsOrthographic)
                DrawingScale = Camera.Zoom / 13f;
            else if (drawingScale != 0.5f)
                DrawingScale = 0.5f;
        }

        public override bool RayIntersects(Ray ray, float mouseX, float mouseY)
        {
            return mouseX >= screenPos.X - origin.X * textureScale &&
                   mouseX <= screenPos.X + (texture.Width - origin.X) * textureScale &&
                   mouseY >= screenPos.Y - origin.Y * textureScale &&
                   mouseY <= screenPos.Y + (texture.Height - origin.Y) * textureScale;
        }

        public override void Draw(SpriteBatch spriteBatch, bool lightDirectionEnabled = false, Vector3 lightDirection = new Vector3(), bool alpha = false)
        {
            if (mapModel == null || mapModel.Selected != this)
                return;

            if (basicEffect == null)
                return;

            if (screenPos3.Z > 1) return;

            Ray ray = Helper.Pick(GraphicsDevice.Viewport, Camera, screenPos.X, screenPos.Y);
            Vector3 screenToLines = ray.Direction * 2;

            basicEffect.World = Matrix.CreateFromQuaternion(rotation) * Matrix.CreateTranslation(ray.Position + screenToLines);
            basicEffect.View = Camera.World;
            basicEffect.Projection = Camera.Projection;
            basicEffect.VertexColorEnabled = true;

            foreach (EffectPass pass in basicEffect.CurrentTechnique.Passes)
            {
                pass.Apply();
                GraphicsDevice.DrawUserIndexedPrimitives(PrimitiveType.LineList, vertices, 0, vertices.Length, indices, 0, indices.Length / 2);
            }

            basicEffect.World = Matrix.Identity;
        }

        public override void DrawSprite(SpriteBatch spriteBatch)
        {
            if (texture == null)
                return;

            if (screenPos3.Z > 1) return;

            //spriteBatch.Begin();
            spriteBatch.Draw(texture, screenPos, null, Color.White, 0, origin, textureScale, SpriteEffects.None, layerDepth);
            //spriteBatch.End();
        }
    }
}
