using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace EditorModel
{
    public class TerrainBrush : BaseTerrain
    {
        private string text;

        public string Text
        {
            get { return text; }
            set { text = value; }
        }

        private Terrain terrain;
        private int startX;
        private int startY;
        private int endX;
        private int endY;
        private int halfWidth;
        private int halfHeight;

        //public TerrainBrush(GraphicsDevice graphicsDevice, Terrain terrain)
        //    : base(graphicsDevice, 8, 8, false)
        //{
        //    this.terrain = terrain;
        //    InitializeAll();
        //}

        public TerrainBrush(GraphicsDevice graphicsDevice, Terrain terrain, Texture2D heightMap)
            : base(graphicsDevice, heightMap, false)
        {
            this.terrain = terrain;
            vertices = new VertexPositionColorNormal[terrain.Vertices.Length];
            for (int i = 0; i < vertices.Length; i++)
            {
                vertices[i].Position = terrain.Vertices[i].Position;
            }
            //vertices = terrain.Vertices;
            InitializeAll(heightMap);
        }

        protected override void InitializeVertices()
        {
            halfWidth = (int)Math.Round((double)width / 2);
            halfHeight = (int)Math.Round((double)height / 2);
            startX = (int)position.X - halfWidth;
            startY = (int)-position.Z - halfHeight;
            endX = (int)position.X + halfWidth;
            endY = (int)-position.Z + halfHeight;

            for (int x = 0; x < terrain.Width; x++)
            {
                for (int y = 0; y < terrain.Height; y++)
                {
                    int i = x + y * terrain.Width;
                    vertices[i].Position.Y += 0.1f;
                    if (x >= startX && x < endX && y >= startY && y < endY)
                    {
                        vertices[i].Color = Color.Yellow * heightFactor[x - startX, y - startY];
                    }
                    else
                    {
                        vertices[i].Color = Color.Yellow * 0;
                    }
                }
            }

            //int startX = (int)position.X - (int)Math.Round((double)width / 2),
            //    startY = (int)-position.Z - (int)Math.Round((double)height /2);
            //
            //for (int x = 0; x < width; x++)
            //{
            //    for (int y = 0; y < height; y++)
            //    {
            //        float posX = startX + x, posY = startY + y;
            //        int i = x + y * width;
            //        if (posX >= 0 && posX < terrain.HeightData.GetLength(0) && posY >= 0 && posY < terrain.HeightData.GetLength(1))
            //        {
            //            int j = (int)posX + (int)posY * terrain.Width;
            //            vertices[i].Position = new Vector3(posX, terrain.Vertices[j].Position.Y + 0.1f, -posY);
            //        }
            //        else
            //        {
            //            vertices[i].Position = new Vector3(posX, 0, -posY);
            //        }
            //        vertices[i].Color = Color.Yellow * heightFactor[x, y];
            //    }
            //}
        }

        protected override void InitializeIndices()
        {
            indices = new short[(terrain.Width - 1) * (terrain.Height - 1) * 6];
            int counter = 0;
            for (int y = 0; y < terrain.Width - 1; y++)
            {
                for (int x = 0; x < terrain.Height - 1; x++)
                {
                    int lowerLeft = x + y * terrain.Width;
                    int lowerRight = (x + 1) + y * terrain.Width;
                    int topLeft = x + (y + 1) * terrain.Width;
                    int topRight = (x + 1) + (y + 1) * terrain.Width;

                    indices[counter++] = (short)topLeft;
                    indices[counter++] = (short)lowerRight;
                    indices[counter++] = (short)lowerLeft;

                    indices[counter++] = (short)topLeft;
                    indices[counter++] = (short)topRight;
                    indices[counter++] = (short)lowerRight;
                }
            }
        }

        public override void CalculateNormals()
        {
            for (int i = 0; i < vertices.Length; i++)
                vertices[i].Normal = new Vector3(0, 1, 0);
        }

        protected void RecolourizeVertices()
        {
            for (int x = Math.Max(0, startX); x <= Math.Min(terrain.Width - 1, endX); x++)
            {
                for (int y = Math.Max(0, startY); y <= Math.Min(terrain.Height - 1, endY); y++)
                {
                    int i = x + y * terrain.Width;
                    if (x >= startX && x < endX && y >= startY && y < endY)
                    {
                        vertices[i].Color = Color.Yellow * 0;
                    }
                }
            }

            startX = (int)position.X - halfWidth;
            startY = (int)-position.Z - halfHeight;
            endX = (int)position.X + halfWidth;
            endY = (int)-position.Z + halfHeight;

            for (int x = Math.Max(0, startX); x <= Math.Min(terrain.Width - 1, endX); x++)
            {
                for (int y = Math.Max(0, startY); y <= Math.Min(terrain.Height - 1, endY); y++)
                {
                    int i = x + y * terrain.Width;
                    if (x >= startX && x < endX && y >= startY && y < endY)
                    {
                        vertices[i].Color = Color.Yellow * heightFactor[x - startX, y - startY];
                    }
                }
            }
        }

        public void OnMouseMove(int terrainIndex)
        {
            float terrainX = terrainIndex % terrain.Width,
                  terrainY = terrainIndex / terrain.Width;

            text = terrainX + ", " + terrainY + "\r\n" + terrain.TerrainIndexer.Text + " " + terrain.Indices.Length + " " + terrainIndex + "\r\n";

            if (terrainIndex == -1)
            {
                return;
                //position = new Vector3(-width, 0, height);
            }
            else
            {
                position = terrain.Vertices[terrainIndex].Position;
            }
            position.Y += 0.1f;
            Position = position;
            RecolourizeVertices();
            //CopyToBuffers();
            try
            {
                VertexBuffer.SetData(vertices);
            }
            catch { }
        }

        public override void Draw(BasicEffect basicEffect)
        {
            graphicsDevice.BlendState = BlendState.AlphaBlend;
            base.Draw(basicEffect);
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
            for (int x = Math.Max(0, startX); x <= Math.Min(terrain.Width - 1, endX); x++)
            {
                for (int y = Math.Max(0, startY); y <= Math.Min(terrain.Height - 1, endY); y++)
                {
                    int i = x + y * terrain.Width;
                    if (x >= startX && x < endX && y >= startY && y < endY)
                    {
                        terrain.Vertices[i].Position.Y += k * heightFactor[x - startX, y - startY];
                        vertices[i].Position.Y += k * heightFactor[x - startX, y - startY];
                    }
                }
            }
            CopyToBuffers();
            terrain.CalculateNormals();
            terrain.CopyToBuffers();
        }
    }
}
