using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace EditorModel
{
    public class GridPointer : GridObject
    {
        VertexPositionColor[] vertices;
        short[] indices;
        VertexBuffer vertexBuffer;
        IndexBuffer indexBuffer;
        string text;

        public override Vector3 Position
        {
            get
            {
                return base.Position;
            }
            set
            {
                base.Position = value;
                InitializeVertices();
                text = position + "\r\n";
            }
        }

        public override Vector2 GridPosition
        {
            get
            {
                return base.GridPosition;
            }
            set
            {
                base.GridPosition = value;
                InitializeVertices();
            }
        }

        public string Text
        {
            get { return text; }
        }

        public GridPointer(Grid grid)
            : base(grid)
        {
            InitializeVertices();
            GridPosition = prevGridPosition = new Vector2();
            text = "";
            InitializeVertices();
            InitializeIndices();
        }

        public override void UpdateObserver()
        {
            InitializeVertices();
            InitializeIndices();
        }

        public void InitializeVertices()
        {
            vertices = new VertexPositionColor[(grid.Size + 1) * (grid.Size + 1)];
            float startX = gridPosition.X * grid.Size, startY = gridPosition.Y * grid.Size;
            for (int x = 0; x < grid.Size + 1; x++)
            {
                for (int y = 0; y < grid.Size + 1; y++)
                {
                    float tempX = startX + x, tempY = startY + y;
                    int i = x + y * (grid.Size + 1), j = (int)(tempX + tempY * grid.Terrain.Width);
                    if (j < grid.Terrain.Vertices.Length)
                        vertices[i] = new VertexPositionColor(new Vector3(tempX, grid.Terrain.Vertices[j].Position.Y + 0.1f, tempY), Color.Yellow * 0.7f);
                    else
                        vertices[i] = new VertexPositionColor(new Vector3(tempX, 0.1f, tempY), Color.Yellow * 0.7f);
                }
            }
            vertexBuffer = new VertexBuffer(grid.GraphicsDevice, VertexPositionColor.VertexDeclaration, vertices.Length, BufferUsage.WriteOnly);
            vertexBuffer.SetData(vertices);
        }

        public void InitializeIndices()
        {
            indices = new short[(grid.Size) * (grid.Size) * 6];
            int counter = 0;
            for (int y = 0; y < grid.Size; y++)
            {
                for (int x = 0; x < grid.Size; x++)
                {
                    int lowerLeft = x + y * (grid.Size + 1);
                    int lowerRight = (x + 1) + y * (grid.Size + 1);
                    int topLeft = x + (y + 1) * (grid.Size + 1);
                    int topRight = (x + 1) + (y + 1) * (grid.Size + 1);

                    indices[counter++] = (short)topLeft;
                    indices[counter++] = (short)lowerRight;
                    indices[counter++] = (short)lowerLeft;

                    indices[counter++] = (short)topLeft;
                    indices[counter++] = (short)topRight;
                    indices[counter++] = (short)lowerRight;
                }
            }
            indexBuffer = new IndexBuffer(grid.GraphicsDevice, IndexElementSize.SixteenBits, indices.Length, BufferUsage.WriteOnly);
            indexBuffer.SetData(indices);
        }

        public void OnMouseMove(int terrainIndex)
        {
            float terrainX = terrainIndex % grid.Terrain.Width,
                  terrainY = terrainIndex / grid.Terrain.Width;
            if (terrainIndex == -1)
                return;
            Position = grid.Terrain.Vertices[terrainIndex].Position;
            if (gridPosition.X != prevGridPosition.X || gridPosition.Y != prevGridPosition.Y)
                InitializeVertices();
            prevGridPosition.X = gridPosition.X;
            prevGridPosition.Y = gridPosition.Y;
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            grid.BasicEffect.World = Matrix.Identity;
            grid.BasicEffect.View = grid.Camera.World;
            grid.BasicEffect.Projection = grid.Camera.Projection;
            grid.BasicEffect.VertexColorEnabled = true;

            RasterizerState rs = new RasterizerState();
            rs.CullMode = CullMode.None;
            grid.GraphicsDevice.RasterizerState = rs;
            grid.GraphicsDevice.SetVertexBuffer(vertexBuffer);
            grid.GraphicsDevice.Indices = indexBuffer;
            grid.GraphicsDevice.BlendState = BlendState.AlphaBlend;

            foreach (EffectPass pass in grid.BasicEffect.CurrentTechnique.Passes)
            {
                pass.Apply();
                grid.GraphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, vertices.Length, 0, indices.Length / 3);
            }
        }
    }
}
