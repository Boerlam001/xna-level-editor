using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Jitter;

namespace XleModel
{
    public class Road : GridObject
    {
        DrawingObject drawingObject;
        Vector3 min1, max1, min2, max2;
        private BasicEffect basicEffect;

        public override Vector2 GridPosition
        {
            get
            {
                return base.GridPosition;
            }
            set
            {
                base.GridPosition = value;
                base.Position = position;
                Vector3 pos = position;
                pos.X += grid.Size / 2;
                pos.Z += grid.Size / 2;
                drawingObject.Position = pos;
            }
        }

        public override Vector3 Position
        {
            get
            {
                return base.Position;
            }
            set
            {
                base.Position = position;
                Vector3 pos = position;
                pos.X += grid.Size / 2;
                pos.Z += grid.Size / 2;
                pos.Y += 0.1f;
                drawingObject.Position = pos;
            }
        }

        public override float RotationY
        {
            get
            {
                return base.RotationY;
            }
            set
            {
                drawingObject.RotationY = base.RotationY = value;
                BoundingBox bbox = drawingObject.CreateBoundingBox();
                Vector3 center = Vector3.Transform(drawingObject.Center * drawingObject.Scale, Matrix.CreateFromQuaternion(drawingObject.Rotation));
                min1 = drawingObject.Position - center + bbox.Min * drawingObject.Scale;
                min1.Y += 0.1f;
                max1 = drawingObject.Position - center + bbox.Max * drawingObject.Scale;
                max1.Y += 0.1f;
                min2 = drawingObject.Position + bbox.Min * drawingObject.Scale;
                min2.Y += 0.1f;
                max2 = drawingObject.Position + bbox.Max * drawingObject.Scale;
                max2.Y += 0.1f;
                drawingObject.Position += (position * (Vector3.UnitX + Vector3.UnitZ) - min1 * (Vector3.UnitX + Vector3.UnitZ));
                //drawingObject.Position += -drawingObject.Center;
            }
        }

        public Road(Game game, World physicsWorld, Grid grid, Model roadModel, BasicEffect basicEffect)
            : base(game, physicsWorld, grid)
        {
            drawingObject = new DrawingObject(game, grid.Camera, null, physicsWorld);
            drawingObject.DrawingModel = roadModel;
            drawingObject.Position = position;
            drawingObject.Scale = new Vector3(grid.Size / 2);
            drawingObject.PhysicsEnabled = false;
            physicsEnabled = false;
            this.basicEffect = basicEffect;
        }

        public override void UpdateObserver()
        {
            drawingObject.Scale = new Vector3(grid.Size);
        }

        public override void Draw(GameTime gameTime)
        {
            if (!IsInitialized)
                Initialize();
            if (!drawingObject.IsInitialized)
                drawingObject.Initialize();

            GraphicsDevice.BlendState = BlendState.AlphaBlend;
            drawingObject.Draw(gameTime);

            GraphicsDevice.BlendState = BlendState.Opaque;
            GraphicsDevice.DepthStencilState = DepthStencilState.Default;
            GraphicsDevice.SamplerStates[0] = SamplerState.LinearWrap;

            base.Draw(gameTime);

            //basicEffect.World = Matrix.Identity;
            //basicEffect.View = Camera.World;
            //basicEffect.Projection = Camera.Projection;
            //basicEffect.VertexColorEnabled = true;
            //VertexPositionColor[] verts = new VertexPositionColor[4]
            //{
            //    new VertexPositionColor(min1, Color.White),
            //    new VertexPositionColor(max1, Color.Red),
            //    new VertexPositionColor(min2, Color.White),
            //    new VertexPositionColor(max2, Color.Red)
            //};
            //foreach (EffectPass pass in basicEffect.CurrentTechnique.Passes)
            //{
            //    pass.Apply();
            //    GraphicsDevice.DrawUserPrimitives(PrimitiveType.LineList, verts, 0, 2);
            //}
        }

        public override void CheckOrientation()
        {
            Vector2 top = gridPosition + new Vector2(0, -1),
                    bottom = gridPosition + new Vector2(0, 1),
                    left = gridPosition + new Vector2(-1, 0),
                    right = gridPosition + new Vector2(1, 0);
            bool topExists = !grid.GridOutOfBounds(top) && grid.GridObjects[(int)top.X, (int)top.Y] != null,
                 bottomExists = !grid.GridOutOfBounds(bottom) && grid.GridObjects[(int)bottom.X, (int)bottom.Y] != null,
                 leftExists = !grid.GridOutOfBounds(left) && grid.GridObjects[(int)left.X, (int)left.Y] != null,
                 rightExists = !grid.GridOutOfBounds(right) && grid.GridObjects[(int)right.X, (int)right.Y] != null;
            RotationY = 0;
            if (topExists || bottomExists)
            {
                if (rotationY != 0)
                    RotationY = 0;
                if (drawingObject.DrawingModel != grid.RoadModel)
                    drawingObject.DrawingModel = grid.RoadModel;
            }
            else if (leftExists || rightExists)
            {
                if (rotationY != 90)
                    RotationY = 90;
                if (drawingObject.DrawingModel != grid.RoadModel)
                    drawingObject.DrawingModel = grid.RoadModel;
            }

            int cek = 0;
            if (leftExists) cek++;
            if (rightExists) cek++;
            if (topExists) cek++;
            if (bottomExists) cek++;
            if (cek > 2)
                return;

            if (topExists && leftExists)
            {
                if (rotationY != 0)
                    RotationY = 0;
                if (drawingObject.DrawingModel != grid.RoadModel_belok)
                    drawingObject.DrawingModel = grid.RoadModel_belok;
            }
            if (bottomExists && leftExists)
            {
                if (rotationY != 90)
                    RotationY = 90;
                if (drawingObject.DrawingModel != grid.RoadModel_belok)
                    drawingObject.DrawingModel = grid.RoadModel_belok;
            }
            if (bottomExists && rightExists)
            {
                if (rotationY != 180)
                    RotationY = 180;
                if (drawingObject.DrawingModel != grid.RoadModel_belok)
                    drawingObject.DrawingModel = grid.RoadModel_belok;
            }
            if (topExists && rightExists)
            {
                if (rotationY != 270)
                    RotationY = 270;
                if (drawingObject.DrawingModel != grid.RoadModel_belok)
                    drawingObject.DrawingModel = grid.RoadModel_belok;
            }
        }
    }
}
