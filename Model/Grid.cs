using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace EditorModel
{
    public class Grid : Subject
    {
        VertexPositionColor[] vertices;
        Terrain terrain;
        int size;
        VertexBuffer vertexBuffer;
        GraphicsDevice graphicsDevice;
        BasicEffect basicEffect;
        Camera camera;
        private int width;
        private int height;
        GridObject[,] gridObjects;
        Model roadModel, roadModel_belok;

        public Terrain Terrain
        {
            get { return terrain; }
            set { terrain = value; }
        }

        public int Size
        {
            get { return size; }
            set
            {
                size = value;
                InitializeVertices();
            }
        }

        public BasicEffect BasicEffect
        {
            get { return basicEffect; }
            set { basicEffect = value; }
        }

        public GraphicsDevice GraphicsDevice
        {
            get { return graphicsDevice; }
            set { graphicsDevice = value; }
        }

        public Camera Camera
        {
            get { return camera; }
            set { camera = value; }
        }

        public GridObject[,] GridObjects
        {
            get { return gridObjects; }
            set { gridObjects = value; }
        }

        public Model RoadModel
        {
            get { return roadModel; }
            set { roadModel = value; }
        }

        public Model RoadModel_belok
        {
            get { return roadModel_belok; }
            set { roadModel_belok = value; }
        }

        public int Width
        {
            get { return width; }
        }

        public int Height
        {
            get { return height; }
        }

        public Grid(Terrain terrain, int size, Camera camera, GraphicsDevice graphicsDevice, BasicEffect basicEffect)
        {
            this.terrain = terrain;
            this.size = size;
            this.camera = camera;
            this.graphicsDevice = graphicsDevice;
            this.basicEffect = basicEffect;
            InitializeVertices();
            this.gridObjects = new GridObject[width, height];
        }

        public void InitializeVertices()
        {
            width = (int)Math.Ceiling((double)terrain.Width / size) + 1;
            height = (int)Math.Ceiling((double)terrain.Height / size) + 1;
            vertices = new VertexPositionColor[(width + height) * 2];
            for (int i = 0; i < vertices.Length;)
            {
                if (i < width * 2)
                {
                    int x = i / 2;
                    vertices[i] = new VertexPositionColor(new Vector3(x * size, 0.1f, 0), Color.Yellow);
                    i++;
                    vertices[i] = new VertexPositionColor(new Vector3(x * size, 0.1f, (height - 1) * size), Color.Yellow);
                    i++;
                }
                else
                {
                    int z = i / 2 - width;
                    vertices[i] = new VertexPositionColor(new Vector3(0, 0.1f, z * size), Color.Yellow);
                    i++;
                    vertices[i] = new VertexPositionColor(new Vector3((width - 1) * size, 0.1f, z * size), Color.Yellow);
                    i++;
                }
            }
            vertexBuffer = new VertexBuffer(graphicsDevice, VertexPositionColor.VertexDeclaration, (width + height) * 2, BufferUsage.WriteOnly);
            vertexBuffer.SetData(vertices);
        }

        public void Draw()
        {
            basicEffect.World = Matrix.Identity;
            basicEffect.View = camera.World;
            basicEffect.Projection = camera.Projection;
            basicEffect.VertexColorEnabled = true;

            graphicsDevice.SetVertexBuffer(vertexBuffer);

            foreach (EffectPass pass in basicEffect.CurrentTechnique.Passes)
            {
                pass.Apply();
                graphicsDevice.DrawPrimitives(PrimitiveType.LineList, 0, width + height);
            }
        }

        public bool GridOutOfBounds(Vector2 gridPosition)
        {
            return !(gridPosition.X >= 0 && gridPosition.X < width && gridPosition.Y >= 0 && gridPosition.Y < height);
        }

        public void AddRoad(Vector2 gridPosition)
        {
            if (roadModel != null)
                if (!GridOutOfBounds(gridPosition) && gridObjects[(int)gridPosition.X, (int)gridPosition.Y] == null)
                {
                    Road newRoad = new Road(this, roadModel, basicEffect);
                    newRoad.GridPosition = gridPosition;
                    newRoad.Camera = camera;
                    newRoad.GraphicsDevice = graphicsDevice;
                    gridObjects[(int)gridPosition.X, (int)gridPosition.Y] = newRoad;

                    newRoad.CheckOrientation();
                    
                    Vector2 top =    newRoad.GridPosition + new Vector2(0, -1),
                            bottom = newRoad.GridPosition + new Vector2(0, 1),
                            left =   newRoad.GridPosition + new Vector2(-1, 0),
                            right =  newRoad.GridPosition + new Vector2(1, 0);
                    if (!GridOutOfBounds(top) && gridObjects[(int)top.X, (int)top.Y] != null)
                        gridObjects[(int)top.X, (int)top.Y].CheckOrientation();
                    if (!GridOutOfBounds(bottom) && gridObjects[(int)bottom.X, (int)bottom.Y] != null)
                        gridObjects[(int)bottom.X, (int)bottom.Y].CheckOrientation();
                    if (!GridOutOfBounds(left) && gridObjects[(int)left.X, (int)left.Y] != null)
                        gridObjects[(int)left.X, (int)left.Y].CheckOrientation();
                    if (!GridOutOfBounds(right) && gridObjects[(int)right.X, (int)right.Y] != null)
                        gridObjects[(int)right.X, (int)right.Y].CheckOrientation();
                }
        }
    }
}
