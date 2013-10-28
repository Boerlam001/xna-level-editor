using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace EditorModel
{
    public class TerrainBrush : BaseObject
    {
        private string text;
        protected VertexPositionColorNormal[] vertices;
        private Terrain terrain;
        private int startX;
        private int startY;
        private int endX;
        private int endY;
        private int halfWidth;
        private int halfHeight;
        protected float[,] heightFactor;
        private Texture2D heightMap;
        private int width;
        private int height;
        private Color[] heightMapColors;
        private float[,] heightData;
        private short[] indices;
        private BasicEffect basicEffect;
        private float heightColorFactor;

        public string Text
        {
            get { return text; }
            set { text = value; }
        }

        public Terrain Terrain
        {
            get { return terrain; }
            set { terrain = value; }
        }

        public VertexPositionColorNormal[] Vertices
        {
            get { return vertices; }
            set { vertices = value; }
        }

        public BasicEffect BasicEffect
        {
            get { return basicEffect; }
            set { basicEffect = value; }
        }

        public float HeightColorFactor
        {
            get { return heightColorFactor; }
            set { heightColorFactor = value; }
        }

        public TerrainBrush(GraphicsDevice graphicsDevice, Terrain terrain, Texture2D heightMap, bool initialize = false)
        {
            this.GraphicsDevice = graphicsDevice;
            this.heightMap = heightMap;
            this.terrain = terrain;
            heightColorFactor = 0.2f;
            InitializeAll(heightMap);
        }

        protected void InitializeAll(Texture2D heightMap)
        {
            LoadHeightData(heightMap);
            InitializeVertices();
            InitializeIndices();
        }

        protected void LoadHeightData(Texture2D heightMap)
        {
            width = heightMap.Width;
            height = heightMap.Height;

            heightMapColors = new Color[width * height];
            heightMap.GetData(heightMapColors);

            heightData = new float[width, height];
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    heightData[x, y] = heightMapColors[x + y * width].R * heightColorFactor;
                }
            }
            
            heightFactor = new float[width, height];
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    heightFactor[x, y] = heightData[x, y] / terrain.MaxHeight;
                }
            }
        }



        protected void InitializeVertices()
        {
            vertices = new VertexPositionColorNormal[width * height];

            halfWidth = (int)Math.Round((double)width / 2);
            halfHeight = (int)Math.Round((double)height / 2);
            startX = (int)position.X - halfWidth;
            startY = (int)position.Z - halfHeight;
            endX = (int)position.X + halfWidth;
            endY = (int)position.Z + halfHeight;

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    float posX = startX + x, posY = startY + y;
                    int i = x + y * width;
                    if (posX >= 0 && posX < terrain.HeightData.GetLength(0) && posY >= 0 && posY < terrain.HeightData.GetLength(1))
                    {
                        int j = (int)posX + (int)posY * terrain.Width;
                        vertices[i].Position = new Vector3(posX, terrain.Vertices[j].Position.Y + 0.1f, posY);
                        vertices[i].Color = Color.Yellow * heightFactor[x, y];
                    }
                    else
                    {
                        vertices[i].Position = new Vector3(posX, 0, posY);
                        vertices[i].Color = new Color(0, 0, 0, 0);
                    }
                    
                }
            }
        }

        protected virtual void InitializeIndices()
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

        protected void RecolourizeVertices()
        {
            startX = (int)position.X - halfWidth;
            startY = (int)position.Z - halfHeight;
            endX = (int)position.X + halfWidth;
            endY = (int)position.Z + halfHeight;

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    float posX = startX + x, posY = startY + y;
                    int i = x + y * width;
                    if (posX >= 0 && posX < terrain.Width && posY >= 0 && posY < terrain.Height)
                    {
                        int j = (int)posX + (int)posY * terrain.Width;
                        vertices[i].Position = new Vector3(posX, terrain.Vertices[j].Position.Y + 0.1f, posY);
                        vertices[i].Color = Color.Yellow * heightFactor[x, y];
                    }
                    else
                    {
                        vertices[i].Position = new Vector3(posX, 0, posY);
                        vertices[i].Color = new Color(0, 0, 0, 0);
                    }

                }
            }
        }

        public void OnMouseMove(int terrainIndex)
        {
            float terrainX = terrainIndex % terrain.Width,
                  terrainY = terrainIndex / terrain.Width;

            if (terrainIndex == -1)
            {
                return;
            }
            else
            {
                position = terrain.Vertices[terrainIndex].Position;
            }
            position.Y += 0.1f;
            Position = position;
            RecolourizeVertices();
        }

        public void Draw()
        {
            GraphicsDevice.BlendState = BlendState.AlphaBlend;
            try
            {
                RasterizerState rs = new RasterizerState();
                rs.CullMode = CullMode.None;
                GraphicsDevice.RasterizerState = rs;
                basicEffect.World = Matrix.Identity;
                basicEffect.VertexColorEnabled = true;

                foreach (EffectPass pass in basicEffect.CurrentTechnique.Passes)
                {
                    pass.Apply();
                    GraphicsDevice.DrawUserIndexedPrimitives(PrimitiveType.TriangleList, vertices, 0, vertices.Length, indices, 0, indices.Length / 3);
                }

                GraphicsDevice.RasterizerState = RasterizerState.CullCounterClockwise;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void Increase()
        {
            ModifyTerrain(1);
        }

        public void Decrease()
        {
            ModifyTerrain(-1);
        }

        public void ModifyTerrain(int k)
        {
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    float posX = startX + x, posY = startY + y;
                    int i = x + y * width;
                    if (posX >= 0 && posX < terrain.HeightData.GetLength(0) && posY >= 0 && posY < terrain.HeightData.GetLength(1))
                    {
                        int j = (int)posX + (int)posY * terrain.Width;
                        float newHeight = terrain.Vertices[j].Position.Y + k * heightFactor[x, y];
                        float newHeightColor = (newHeight - terrain.MinHeight) / terrain.HeightColorFactor;
                        if (newHeightColor >= 0 && newHeightColor <= 255)
                        {
                            terrain.Vertices[j].Position.Y = terrain.HeightData[(int)posX, (int)posY] = newHeight;
                            terrain.HeightMapColors[j].R = (byte)newHeightColor;
                            terrain.HeightMapColors[j].G = (byte)newHeightColor;
                            terrain.HeightMapColors[j].B = (byte)newHeightColor;
                            terrain.HeightMapColors[j].A = 255;
                            vertices[i].Position.Y += k * heightFactor[x, y];
                        }
                        else if (newHeightColor < 0)
                        {
                            terrain.Vertices[j].Position.Y = terrain.HeightData[(int)posX, (int)posY] = terrain.MinHeight;
                            terrain.HeightMapColors[j].R = (byte)0;
                            terrain.HeightMapColors[j].G = (byte)0;
                            terrain.HeightMapColors[j].B = (byte)0;
                            terrain.HeightMapColors[j].A = 255;
                            vertices[i].Position.Y = terrain.MinHeight + 0.1f;
                        }
                        else if (newHeightColor > 255)
                        {
                            terrain.Vertices[j].Position.Y = terrain.HeightData[(int)posX, (int)posY] = terrain.MaxHeight;
                            terrain.HeightMapColors[j].R = (byte)255;
                            terrain.HeightMapColors[j].G = (byte)255;
                            terrain.HeightMapColors[j].B = (byte)255;
                            terrain.HeightMapColors[j].A = 255;
                            vertices[i].Position.Y = terrain.MaxHeight + 0.1f;
                        }
                    }
                }
            }

            terrain.CalculateNormals();
            terrain.CopyToBuffers();
        }
    }
}
