using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace EditorModel
{
    public class ModelBoundingBox : IObserver
    {
        private BoundingBoxBuffer boundingBoxBuffer;

        public BoundingBoxBuffer BoundingBoxBuffer
        {
            get { return boundingBoxBuffer; }
            set { boundingBoxBuffer = value; }
        }

        private AxisLines axisLines;

        public AxisLines AxisLines
        {
            get { return axisLines; }
            set { axisLines = value; }
        }

        private DrawingObject model;

        public DrawingObject Model
        {
            get { return model; }
            set
            {
                model = value;
                Update();
            }
        }

        private Camera camera;

        public Camera Camera
        {
            get { return camera; }
            set { camera = value; }
        }

        private GraphicsDevice graphicsDevice;

        public GraphicsDevice GraphicsDevice
        {
            get { return graphicsDevice; }
            set { graphicsDevice = value; }
        }

        private SpriteBatch spriteBatch;

        public SpriteBatch SpriteBatch
        {
            get { return spriteBatch; }
            set { spriteBatch = value; }
        }

        private SpriteFont spriteFont;

        public SpriteFont SpriteFont
        {
            get { return spriteFont; }
            set { spriteFont = value; }
        }

        public ModelBoundingBox(GraphicsDevice graphicsDevice)
        {
            this.graphicsDevice = graphicsDevice;
            boundingBoxBuffer = new BoundingBoxBuffer(graphicsDevice);
            boundingBoxBuffer.Parent = this;
            axisLines = new AxisLines();
            axisLines.Parent = this;
            boundingBoxBuffer.Attach(axisLines);
        }

        public void Draw(BasicEffect basicEffect)
        {
            boundingBoxBuffer.Draw(basicEffect);
            boundingBoxBuffer.Notify();
            axisLines.Draw(basicEffect, graphicsDevice);
        }

        public void Update()
        {
            boundingBoxBuffer.BoundingBox = model.CreateBoundingBox();
            boundingBoxBuffer.Position = model.Position;
            boundingBoxBuffer.Rotation = model.Rotation;
            axisLines.Position = model.Position;
            boundingBoxBuffer.Notify();
        }
    }
}
