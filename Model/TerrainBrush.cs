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

        public TerrainBrush(GraphicsDevice graphicsDevice, Terrain terrain)
            : base(graphicsDevice, 8, 8, false)
        {
            this.terrain = terrain;
            InitializeAll();
        }

        public TerrainBrush(GraphicsDevice graphicsDevice, Terrain terrain, Texture2D heightMap)
            : base(graphicsDevice, heightMap, false)
        {
            this.terrain = terrain;
            InitializeAll(heightMap);
        }

        protected override void InitializeVertices()
        {
            vertices = new VertexPositionColorNormal[width * height];
            int startX = (int)position.X - (int)Math.Round((double)width / 2),
                startY = (int)-position.Z - (int)Math.Round((double)height /2);

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    float posX = startX + x, posY = startY + y;
                    int i = x + y * width;
                    if (posX >= 0 && posX < terrain.HeightData.GetLength(0) && posY >= 0 && posY < terrain.HeightData.GetLength(1))
                    {
                        int j = (int)posX + (int)posY * terrain.Width;
                        vertices[i].Position = new Vector3(posX, terrain.Vertices[j].Position.Y + 0.1f, -posY);
                    }
                    else
                    {
                        vertices[i].Position = new Vector3(posX, 0, -posY);
                    }
                    vertices[i].Color = Color.Yellow * heightFactor[x, y];
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
            InitializeVertices();
            //CopyToBuffers();
            try
            {
                vertexBuffer.SetData(vertices);
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
            int startX = (int)position.X - (int)Math.Round((double)width / 2),
                startY = (int)-position.Z - (int)Math.Round((double)height / 2);

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    float posX = startX + x, posY = startY + y;
                    if (posX >= 0 && posX < terrain.HeightData.GetLength(0) && posY >= 0 && posY < terrain.HeightData.GetLength(1))
                    {
                        int j = (int)posX + (int)posY * terrain.Width;
                        terrain.Vertices[j].Position.Y += heightFactor[x, y];
                    }
                }
            }

            terrain.CopyToBuffers();
        }
    }
}
