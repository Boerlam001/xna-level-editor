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

        private BaseObject model;

        public BaseObject Model
        {
            get { return model; }
            set
            {
                if (model != null)
                {
                    model.Detach(this);
                    model.Detach(axisLines);
                }
                model = value;
                model.Attach(this);
                model.Attach(axisLines);
                UpdateObserver();
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

        DebugDrawer debugDrawer;

        bool viewBody;

        public bool ViewBody
        {
            get { return viewBody; }
            set { viewBody = value; }
        }

        public ModelBoundingBox(GraphicsDevice graphicsDevice, Camera camera, bool viewBody)
        {
            this.graphicsDevice = graphicsDevice;
            boundingBoxBuffer = new BoundingBoxBuffer(graphicsDevice);
            boundingBoxBuffer.Parent = this;
            axisLines = new AxisLines();
            axisLines.Parent = this;
            this.camera = camera;
            camera.Attach(axisLines);
            debugDrawer = new DebugDrawer(graphicsDevice, camera);
            debugDrawer.Color = Color.LightGreen;
            this.viewBody = viewBody;
        }

        public void Draw(ref BasicEffect basicEffect)
        {
            if (model is DrawingObject)
                boundingBoxBuffer.Draw(basicEffect, model.World);
            axisLines.Draw(ref basicEffect, graphicsDevice);
            model.Body.EnableDebugDraw = viewBody && model.PhysicsEnabled;
            if (model.Body.EnableDebugDraw)
            {
                model.Body.DebugDraw(debugDrawer);
                debugDrawer.Draw();
            }
        }

        public void UpdateObserver()
        {
            axisLines.Position = model.Position;

            if (model is DrawingObject)
            {
                DrawingObject obj = model as DrawingObject;
                boundingBoxBuffer.BoundingBox = obj.CreateBoundingBox();
                axisLines.Position = obj.Position;
            }
        }

        public void ChangeAxisLines(int mode)
        {
            switch (mode)
            {
                case 1:
                    if (axisLines.GetType() != typeof(AxisLines))
                    {
                        camera.Detach(axisLines);
                        axisLines = new AxisLines();
                        axisLines.Parent = this;
                        if (model != null)
                            model.Notify();
                        camera.Attach(axisLines);
                        camera.Notify();
                    }
                    break;
                case 2:
                    if (axisLines.GetType() != typeof(RotationAxisLines))
                    {
                        camera.Detach(axisLines);
                        axisLines = new RotationAxisLines();
                        axisLines.Parent = this;
                        if (model != null)
                            model.Notify();
                        camera.Attach(axisLines);
                        camera.Notify();
                    }
                    break;
                case 3:
                    if (axisLines.GetType() != typeof(ScaleAxisLines))
                    {
                        camera.Detach(axisLines);
                        axisLines = new ScaleAxisLines();
                        axisLines.Parent = this;
                        if (model != null)
                            model.Notify();
                        camera.Attach(axisLines);
                        camera.Notify();
                    }
                    break;
            }
        }
    }
}
