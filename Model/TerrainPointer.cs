using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace EditorModel
{
    public class TerrainPointer
    {
        private string text;

        public string Text
        {
            get { return text; }
            set { text = value; }
        }

        private Terrain terrain;
        private VertexPositionColor[] vertices;
        private int sphereResolution;
        private int lineCount;

        public TerrainPointer(Terrain terrain)
        {
            this.terrain = terrain;
            sphereResolution = 45;
            
            // calculate the number of lines to draw for all circles
            lineCount = (sphereResolution + 1) * 3;

            // we need two vertices per line, so we can allocate our vertices
            vertices = new VertexPositionColor[lineCount * 2];

            text = "";
        }

        public void GenerateCircle(int terrainIndex)
        {
            float terrainX = terrainIndex % terrain.Width,
                  terrainY = terrainIndex / terrain.Width;
            if (terrainX < 0 || terrainX >= terrain.HeightData.GetLength(0) || terrainY < 0 || terrainY >= terrain.HeightData.GetLength(1))
            {
                return;
            }

            text = terrainX + ", " + terrainY + "\r\n";

            Vector3 position = terrain.Vertices[terrainIndex].Position;
            position.Y += 0.1f;

            float step = MathHelper.TwoPi / sphereResolution;
            int index = 0;
            
            Vector3 pos1 = new Vector3((float)Math.Cos(0) * 5, 0f, (float)Math.Sin(0) * 5) + position;

            int x = (int)Math.Round(pos1.X), y = -(int)Math.Round(pos1.Z);
            if (x >= 0 && x < terrain.HeightData.GetLength(0) && y >= 0 && y < terrain.HeightData.GetLength(1))
            {
                pos1.Y = terrain.HeightData[x, y] + 0.1f;
            }
            else
            {
                pos1.Y = position.Y;
            }

            vertices[index++] = new VertexPositionColor(pos1, Color.Yellow);

            for (float a = step; a < MathHelper.TwoPi; a += step)
            {
                pos1 = new Vector3((float)Math.Cos(a) * 5, 0f, (float)Math.Sin(a) * 5) + position;

                x = (int)Math.Round(pos1.X); y = -(int)Math.Round(pos1.Z);
                if (x >= 0 && x < terrain.HeightData.GetLength(0) && y >= 0 && y < terrain.HeightData.GetLength(1))
                {
                    pos1.Y = terrain.HeightData[x, y] + 0.1f;
                }
                else
                {
                    pos1.Y = position.Y;
                }

                vertices[index++] = new VertexPositionColor(pos1, Color.Yellow);
                vertices[index] = vertices[index - 1];
                index++;
            }

            vertices[index] = vertices[0];
        }

        public void OnMouseMove(int terrainIndex)
        {
            GenerateCircle(terrainIndex);
        }

        public void Draw(BasicEffect basicEffect, GraphicsDevice graphicsDevice)
        {
            basicEffect.VertexColorEnabled = true;
            basicEffect.World = Matrix.Identity;

            DepthStencilState d = new DepthStencilState();
            d.DepthBufferEnable = false;
            graphicsDevice.DepthStencilState = d;

            foreach (EffectPass pass in basicEffect.CurrentTechnique.Passes)
            {
                pass.Apply();
                graphicsDevice.DrawUserPrimitives(PrimitiveType.LineList, vertices, 0, lineCount);
            }
        }
    }
}
