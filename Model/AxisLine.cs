using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

namespace EditorModel
{
    public class AxisLine
    {
        DrawingObject model;

        public DrawingObject Model
        {
            get { return model; }
            set { model = value; }
        }

        VertexPositionColor[] axisVertices;

        public VertexPositionColor[] AxisVertices
        {
            get { return axisVertices; }
            set { axisVertices = value; }
        }

        public AxisLine()
        {
            axisVertices = new VertexPositionColor[6];

            axisVertices[0].Position = axisVertices[2].Position = axisVertices[4].Position = Vector3.Zero;

            Vector3 xOffset = new Vector3(10, 0, 0),
                    yOffset = new Vector3(0, 10, 0),
                    zOffset = new Vector3(0, 0, 10);

            axisVertices[1].Position = xOffset;
            axisVertices[3].Position = yOffset;
            axisVertices[5].Position = zOffset;

            axisVertices[0].Color = axisVertices[1].Color = Color.Red;
            axisVertices[2].Color = axisVertices[3].Color = Color.Green;
            axisVertices[4].Color = axisVertices[5].Color = Color.Blue;
        }

        public void Draw(BasicEffect effect, GraphicsDevice graphicsDevice)
        {
            effect.World = model.World;

            effect.VertexColorEnabled = true;

            DepthStencilState d = new DepthStencilState();
            d.DepthBufferEnable = false;

            foreach (EffectPass pass in effect.CurrentTechnique.Passes)
            {
                pass.Apply();

                graphicsDevice.DepthStencilState = d;
                graphicsDevice.DrawUserPrimitives(PrimitiveType.LineList, axisVertices, 0, 3);
                graphicsDevice.DepthStencilState = DepthStencilState.Default;
            }

            effect.World = Matrix.Identity;
            graphicsDevice.SetVertexBuffer(null);
            graphicsDevice.Indices = null;
        }
    }
}
