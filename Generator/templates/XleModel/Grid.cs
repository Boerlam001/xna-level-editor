using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace XleModel
{
    public class Grid : GameComponent
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
        float positionY = 0.0f;
        string gridMapFile;
        Texture2D gridMap;
        private Jitter.World physicsWorld;

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

        public float PositionY
        {
            get { return positionY; }
            set { positionY = value; }
        }

        public string GridMapFile
        {
            get { return gridMapFile; }
            set { gridMapFile = value; }
        }

        public Texture2D GridMap
        {
            get { return gridMap; }
            set { gridMap = value; }
        }

        public Grid(Game game, Terrain terrain, int size, Camera camera, GraphicsDevice graphicsDevice, BasicEffect basicEffect, Jitter.World physicsWorld)
            : base(game)
        {
            this.terrain = terrain;
            this.size = size;
            this.camera = camera;
            this.graphicsDevice = graphicsDevice;
            this.basicEffect = basicEffect;
            InitializeVertices();
            this.gridObjects = new GridObject[width, height];
            this.physicsWorld = physicsWorld;
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
                    vertices[i] = new VertexPositionColor(new Vector3(x * size, positionY + 0.1f, 0), Color.Yellow);
                    i++;
                    vertices[i] = new VertexPositionColor(new Vector3(x * size, positionY + 0.1f, (height - 1) * size), Color.Yellow);
                    i++;
                }
                else
                {
                    int z = i / 2 - width;
                    vertices[i] = new VertexPositionColor(new Vector3(0, positionY + 0.1f, z * size), Color.Yellow);
                    i++;
                    vertices[i] = new VertexPositionColor(new Vector3((width - 1) * size, positionY + 0.1f, z * size), Color.Yellow);
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
                if (!GridOutOfBounds(gridPosition))// && gridObjects[(int)gridPosition.X, (int)gridPosition.Y] == null)
                {
                    FlattenPointedTerrain(gridPosition);

                    if (gridObjects[(int)gridPosition.X, (int)gridPosition.Y] != null)
                        Game.Components.Remove(gridObjects[(int)gridPosition.X, (int)gridPosition.Y]);

                    Road newRoad = new Road(Game, physicsWorld, this, roadModel, basicEffect);
                    newRoad.GridPosition = gridPosition;
                    gridObjects[(int)gridPosition.X, (int)gridPosition.Y] = newRoad;

                    Game.Components.Add(newRoad);

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

        public void FlattenPointedTerrain(Vector2 gridPosition)
        {
            float startX = gridPosition.X * size, startY = gridPosition.Y * size;
            for (int x = 0; x < size + 1; x++)
            {
                for (int y = 0; y < size + 1; y++)
                {
                    float tempX = startX + x, tempY = startY + y;
                    int i = x + y * (size + 1), j = (int)(tempX + tempY * terrain.Width);
                    if (j < terrain.Vertices.Length)
                    {
                        float newHeight = positionY;
                        float newHeightColor = (newHeight - terrain.MinHeight) / terrain.HeightColorFactor;
                        if (newHeightColor >= 0 && newHeightColor <= 255)
                        {
                            terrain.Vertices[j].Position.Y = terrain.HeightData[(int)tempX, (int)tempY] = newHeight;
                            terrain.HeightMapColors[j].R = (byte)newHeightColor;
                            terrain.HeightMapColors[j].G = (byte)newHeightColor;
                            terrain.HeightMapColors[j].B = (byte)newHeightColor;
                            terrain.HeightMapColors[j].A = 255;
                        }
                    }
                }
            }
            terrain.CalculateNormals();
            terrain.CopyToBuffers();
        }

        public void ExportGridMap()
        {
            if (gridMap == null)
                gridMap = new Texture2D(GraphicsDevice, width, height, false, SurfaceFormat.Color);
            if (string.IsNullOrEmpty(gridMapFile))
                return;
            Color[] gridMapColors = new Color[width * height];
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    if (gridObjects[x, y] == null)
                        gridMapColors[x + y * width] = new Color(0, 0, 0);
                    else if (gridObjects[x, y] is Road)
                        gridMapColors[x + y * width] = new Color(10, 10, 10);
                }
            }
            gridMap.SetData<Color>(gridMapColors);

            using (System.IO.Stream stream = System.IO.File.OpenWrite(gridMapFile))
            {
                gridMap.SaveAsPng(stream, width, height);
            }
        }

        public void ImportGridMap()
        {
            Color[] heightMapColors = new Color[width * height];
            gridMap.GetData(heightMapColors);
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    if (gridObjects[x, y] == null)
                    {
                        if (heightMapColors[x + y * width].R >= 1 && heightMapColors[x + y * width].R <= 10)
                            AddRoad(new Vector2(x, y));
                    }
                    else
                    {
                        if (heightMapColors[x + y * width].R == 0)
                        {
                            gridObjects[x, y] = null;
                        }
                    }
                }
            }
        }
    }
}
