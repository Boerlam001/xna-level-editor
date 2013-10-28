using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace EditorModel
{
    public class Road : GridObject
    {
        DrawingObject drawingObject;

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
            }
        }

        public Road(Grid grid, Model roadModel)
            : base(grid)
        {
            drawingObject = new DrawingObject();
            drawingObject.DrawingModel = roadModel;
            drawingObject.Position = position;
            drawingObject.Scale = new Vector3(grid.Size / 2);
            drawingObject.Camera = grid.Camera;
            drawingObject.GraphicsDevice = grid.GraphicsDevice;
        }

        public override void UpdateObserver()
        {
            drawingObject.Scale = new Vector3(grid.Size);
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            drawingObject.Draw(spriteBatch);
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
