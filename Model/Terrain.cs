using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

namespace EditorModel
{
    public struct VertexPositionColorNormal
    {
        public Vector3 Position;
        public Color Color;
        public Vector3 Normal;

        public readonly static VertexDeclaration VertexDeclaration = new VertexDeclaration
        (
            new VertexElement(0, VertexElementFormat.Vector3, VertexElementUsage.Position, 0),
            new VertexElement(sizeof(float) * 3, VertexElementFormat.Color, VertexElementUsage.Color, 0),
            new VertexElement(sizeof(float) * 3 + 4, VertexElementFormat.Vector3, VertexElementUsage.Normal, 0)
        );
    }

    public class Terrain : BaseObject
    {
        private int width = 128;

        public int Width
        {
            get { return width; }
            set { width = value; }
        }

        private int height = 128;

        public int Height
        {
            get { return height; }
            set { height = value; }
        }

        private float[,] heightData;

        public float[,] HeightData
        {
            get { return heightData; }
            set { heightData = value; }
        }

        private VertexPositionColorNormal[] vertices;

        public VertexPositionColorNormal[] Vertices
        {
            get { return vertices; }
            set { vertices = value; }
        }

        private short[] indices;

        public Terrain()
        {
            LoadHeightData();
            InitializeVertices();
            InitializeIndices();
            CalculateNormals();
            //world = Matrix.CreateTranslation(-width / 2.0f, 0, height / 2.0f);
        }

        public Terrain(Texture2D heightMap)
        {
            LoadHeightData(heightMap);
            InitializeVertices();
            InitializeIndices();
            CalculateNormals();
            //world = Matrix.CreateTranslation(-width / 2.0f, 0, height / 2.0f);
        }
                
        private void InitializeVertices()
        {
            float minHeight = float.MaxValue;
            float maxHeight = float.MinValue;
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    if (heightData[x, y] < minHeight)
                        minHeight = heightData[x, y];
                    if (heightData[x, y] > maxHeight)
                        maxHeight = heightData[x, y];
                }
            }

            vertices = new VertexPositionColorNormal[width * height];
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    vertices[x + y * width].Position = new Vector3(x, heightData[x, y], -y);
                    //vertices[x + y * width].Position = new Vector3(x, 0, -y);
                    //vertices[x + y * width].Color = Color.White;
                    
                    if (heightData[x, y] < minHeight + (maxHeight - minHeight) / 4)
                        vertices[x + y * width].Color = Color.Blue;
                    else if (heightData[x, y] < minHeight + (maxHeight - minHeight) * 2 / 4)
                        vertices[x + y * width].Color = Color.Green;
                    else if (heightData[x, y] < minHeight + (maxHeight - minHeight) * 3 / 4)
                        vertices[x + y * width].Color = Color.Brown;
                    else
                        vertices[x + y * width].Color = Color.White;
                }
            }
        }

        private void InitializeIndices()
        {
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

        private void LoadHeightData()
        {
            heightData = new float[width, height];
            Random random = new Random();
            for (int x = 0; x < width; x++)
                for (int y = 0; y < height; y++)
                    heightData[x, y] = 0;// (float)random.NextDouble() * 100;
        }

        private void LoadHeightData(Texture2D heightMap)
        {
            width = heightMap.Width;
            height = heightMap.Height;

            Color[] heightMapColors = new Color[width * height];
            heightMap.GetData(heightMapColors);

            heightData = new float[width, height];
            for (int x = 0; x < width; x++)
                for (int y = 0; y < height; y++)
                    heightData[x, y] = heightMapColors[x + y * width].R / 5.0f;
        }

        private void CalculateNormals()
        {
            for (int i = 0; i < vertices.Length; i++)
                vertices[i].Normal = new Vector3(0, 0, 0);

            for (int i = 0; i < indices.Length / 3; i++)
            {
                int index1 = indices[i * 3];
                int index2 = indices[i * 3 + 1];
                int index3 = indices[i * 3 + 2];

                Vector3 side1 = vertices[index1].Position - vertices[index3].Position;
                Vector3 side2 = vertices[index1].Position - vertices[index2].Position;
                Vector3 normal = Vector3.Cross(side1, side2);

                vertices[index1].Normal += normal;
                vertices[index2].Normal += normal;
                vertices[index3].Normal += normal;
            }

            for (int i = 0; i < vertices.Length; i++)
                vertices[i].Normal.Normalize();
        }

        public void Draw(BasicEffect basicEffect, Effect effect, GraphicsDevice graphicsDevice, Camera camera)
        {
            try
            {
                RasterizerState rs = new RasterizerState();
                rs.CullMode = CullMode.None;
                //rs.FillMode = FillMode.WireFrame;
                graphicsDevice.RasterizerState = rs;
                //effect = null;
                if (effect != null)
                {
                    Vector3 lightDirection = new Vector3(1.0f, -1.0f, -1.0f);
                    lightDirection.Normalize();

                    effect.CurrentTechnique = effect.Techniques["Colored"];
                    effect.Parameters["xView"].SetValue(camera.World);
                    effect.Parameters["xProjection"].SetValue(camera.Projection);
                    effect.Parameters["xWorld"].SetValue(world);
                    effect.Parameters["xLightDirection"].SetValue(lightDirection);
                    effect.Parameters["xAmbient"].SetValue(0.1f);
                    effect.Parameters["xEnableLighting"].SetValue(true);  
                    foreach (EffectPass pass in effect.CurrentTechnique.Passes)
                    {
                        pass.Apply();
                        graphicsDevice.DrawUserIndexedPrimitives(PrimitiveType.TriangleList, vertices, 0, vertices.Length, indices, 0, indices.Length / 3, VertexPositionColorNormal.VertexDeclaration);
                    }
                }
                else
                {
                    basicEffect.World = world;

                    foreach (EffectPass pass in basicEffect.CurrentTechnique.Passes)
                    {
                        pass.Apply();
                        //graphicsDevice.DrawUserIndexedPrimitives(PrimitiveType.LineList, 0, 0, vertexCount, 0, primitiveCount);
                        graphicsDevice.DrawUserIndexedPrimitives(PrimitiveType.TriangleList, vertices, 0, vertices.Length, indices, 0, indices.Length / 3, VertexPositionColor.VertexDeclaration);
                    }

                    basicEffect.World = Matrix.Identity;
                    graphicsDevice.RasterizerState = RasterizerState.CullCounterClockwise;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
