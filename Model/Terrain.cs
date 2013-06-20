using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

namespace EditorModel
{
    public class Terrain : BaseObject
    {
        private int width = 40;

        public int Width
        {
            get { return width; }
            set { width = value; }
        }

        private int height = 40;

        public int Height
        {
            get { return height; }
            set { height = value; }
        }

        private float[,] heightData;
        private short[] indices;
        private VertexPositionColor[] vertices;

        public Terrain()
        {
            //heightData = new float[4, 3];
            //heightData[0, 0] = 0;
            //heightData[1, 0] = 0;
            //heightData[2, 0] = 0;
            //heightData[3, 0] = 0;
            //
            //heightData[0, 1] = 0.5f  ;
            //heightData[1, 1] = 0     ;
            //heightData[2, 1] = -1.0f ;
            //heightData[3, 1] = 0.2f  ;
            //
            //heightData[0, 2] = 1.0f  ;
            //heightData[1, 2] = 1.2f  ;
            //heightData[2, 2] = 0.8f  ;
            //heightData[3, 2] = 0     ;

            vertices = new VertexPositionColor[width * height];
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    //vertices[x + y * width].Position = new Vector3(x, heightData[x, y], -y);
                    vertices[x + y * width].Position = new Vector3(x, 0, -y);
                    vertices[x + y * width].Color = Color.White;
                }
            }

            indices = new short[(width - 1) * (height - 1) * 6];
            int counter = 0;
            for (int y = 0; y < height - 1; y++)
            {
                for (int x = 0; x < width - 1; x++)
                {
                    int lowerLeft = x + y * width;
                    int lowerRight = (x + 1) + y * width;
                    int topLeft = x + (y + 1) * width;
                    int topRight = (x + 1) + (y + 1) * width;

                    indices[counter++] = (short)topLeft;
                    indices[counter++] = (short)lowerRight;
                    indices[counter++] = (short)lowerLeft;

                    indices[counter++] = (short)topLeft;
                    indices[counter++] = (short)topRight;
                    indices[counter++] = (short)lowerRight;
                }
            }
        }

        public void Draw(BasicEffect effect, GraphicsDevice graphicsDevice)
        {
            effect.World = world;
            RasterizerState rs = new RasterizerState();
            rs.CullMode = CullMode.None;
            graphicsDevice.RasterizerState = rs;

            foreach (EffectPass pass in effect.CurrentTechnique.Passes)
            {
                pass.Apply();
                //graphicsDevice.DrawUserIndexedPrimitives(PrimitiveType.LineList, 0, 0, vertexCount, 0, primitiveCount);
                graphicsDevice.DrawUserIndexedPrimitives(PrimitiveType.TriangleList, vertices, 0, vertices.Length, indices, 0, indices.Length / 3, VertexPositionColor.VertexDeclaration);
            }

            effect.World = Matrix.Identity;
            graphicsDevice.RasterizerState = RasterizerState.CullCounterClockwise;
        }
    }
}
