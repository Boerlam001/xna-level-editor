using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace EditorModel
{
    public struct VertexPositionColorNormal : IVertexType
    {
        public Vector3 Position;
        public Color Color;
        public Vector3 Normal;
        private static VertexDeclaration vertexDeclaration;

        public static VertexDeclaration VertexDeclaration
        {
            get
            {
                if (vertexDeclaration == null)
                {
                    vertexDeclaration = new VertexDeclaration
                    (
                        new VertexElement(0, VertexElementFormat.Vector3, VertexElementUsage.Position, 0),
                        new VertexElement(sizeof(float) * 3, VertexElementFormat.Color, VertexElementUsage.Color, 0),
                        new VertexElement(sizeof(float) * 3 + 4, VertexElementFormat.Vector3, VertexElementUsage.Normal, 0)
                    );
                }
                return vertexDeclaration;
            }
        }

        VertexDeclaration IVertexType.VertexDeclaration
        {
            get
            {
                if (vertexDeclaration == null)
                {
                    vertexDeclaration = new VertexDeclaration
                    (
                        new VertexElement(0, VertexElementFormat.Vector3, VertexElementUsage.Position, 0),
                        new VertexElement(sizeof(float) * 3, VertexElementFormat.Color, VertexElementUsage.Color, 0),
                        new VertexElement(sizeof(float) * 3 + 4, VertexElementFormat.Vector3, VertexElementUsage.Normal, 0)
                    );
                }
                return vertexDeclaration;
            }
        }
    }

    public class Terrain : BaseObject
    {
        #region attributes
        protected int width;
        protected int height;
        protected float[,] heightData;
        protected VertexBuffer vertexBuffer;
        protected Color[] heightMapColors;
        protected Texture2D heightMap;
        protected BasicEffect basicEffect;
        protected IndexBuffer indexBuffer;
        protected float minHeight;
        protected float maxHeight;
        protected float heightColorFactor;
        private TerrainIndexer terrainIndexer;
        private string heightMapFile;
        private string effectFile;
        VertexPositionNormalTexture[] vertices;
        protected short[] indices;
        Texture2D texture;
        Effect effect;
        #endregion

        public int Width
        {
            get { return width; }
            set { width = value; }
        }

        public int Height
        {
            get { return height; }
            set { height = value; }
        }

        public float[,] HeightData
        {
            get { return heightData; }
            set { heightData = value; }
        }

        public short[] Indices
        {
            get { return indices; }
            set { indices = value; }
        }

        public VertexBuffer VertexBuffer
        {
            get { return vertexBuffer; }
            set { vertexBuffer = value; }
        }

        public Color[] HeightMapColors
        {
            get { return heightMapColors; }
            set { heightMapColors = value; }
        }

        public Texture2D HeightMap
        {
            get { return heightMap; }
            set { heightMap = value; }
        }

        public float HeightColorFactor
        {
            get { return heightColorFactor; }
        }

        public BasicEffect BasicEffect
        {
            get { return basicEffect; }
            set { basicEffect = value; }
        }

        public float MinHeight
        {
            get { return minHeight; }
            set { minHeight = value; }
        }

        public float MaxHeight
        {
            get { return maxHeight; }
            set { maxHeight = value; }
        }

        public TerrainIndexer TerrainIndexer
        {
            get { return terrainIndexer; }
            set { terrainIndexer = value; }
        }

        public VertexPositionNormalTexture[] Vertices
        {
            get { return vertices; }
            set { vertices = value; }
        }

        public string EffectFile
        {
            get { return effectFile; }
            set { effectFile = value; }
        }

        public string HeightMapFile
        {
            get { return heightMapFile; }
            set { heightMapFile = value; }
        }

        public Texture2D Texture
        {
            get { return texture; }
            set { texture = value; }
        }

        public Effect Effect
        {
            get { return effect; }
            set { effect = value; }
        }

        private Terrain(GraphicsDevice graphicsDevice, Camera camera, Texture2D heightMap, bool initialize = true)
        {
            this.GraphicsDevice = graphicsDevice;
            this.Camera = camera;
            this.heightMap = heightMap;
            heightColorFactor = 0.2f;
            minHeight = -255 * heightColorFactor / 2;
            maxHeight = minHeight * -1;
            basicEffect = new BasicEffect(graphicsDevice);
            if (initialize)
                InitializeAll(heightMap);
        }

        private Terrain(GraphicsDevice graphicsDevice, Camera camera, int width, int height, bool initialize = true)
        {
            this.GraphicsDevice = graphicsDevice;
            this.Camera = camera;
            this.width = width;
            this.height = height;
            heightColorFactor = 0.2f;
            minHeight = -255 * heightColorFactor / 2;
            maxHeight = minHeight * -1;
            basicEffect = new BasicEffect(graphicsDevice);
            if (initialize)
                InitializeAll();
        }

        public Terrain(GraphicsDevice graphicsDevice, Camera camera, string heightMapFile, string effectFile)
            : this(graphicsDevice, camera, 128, 128)
        {
            terrainIndexer = new TerrainIndexer(this, camera, graphicsDevice);
            this.heightMapFile = heightMapFile;
            this.effectFile = effectFile;
        }

        public Terrain(GraphicsDevice graphicsDevice, Camera camera, Texture2D heightMap, string heightMapFile, string effectFile)
            : this(graphicsDevice, camera, heightMap)
        {
            terrainIndexer = new TerrainIndexer(this, camera, graphicsDevice);
            this.heightMapFile = heightMapFile;
            this.effectFile = effectFile;
        }

        public Terrain(GraphicsDevice graphicsDevice, Camera camera, string heightMapFile, string effectFile, int width, int height)
            : this(graphicsDevice, camera, width, height)
        {
            terrainIndexer = new TerrainIndexer(this, camera, graphicsDevice);
            this.heightMapFile = heightMapFile;
            this.effectFile = effectFile;
        }

        protected void InitializeAll()
        {
            LoadHeightData();
            InitializeVertices();
            InitializeIndices();
            CalculateNormals();
            CopyToBuffers();
        }

        protected void InitializeAll(Texture2D heightMap)
        {
            LoadHeightData(heightMap);
            InitializeVertices();
            InitializeIndices();
            CalculateNormals();
            CopyToBuffers();
        }

        protected virtual void LoadHeightData()
        {
            heightMapColors = new Color[width * height];
            heightData = new float[width, height];
            float tempHeight = 0;
            float tempColor = (tempHeight - minHeight) / heightColorFactor;
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    heightMapColors[x + y * width].R = (byte)tempColor;
                    heightMapColors[x + y * width].G = (byte)tempColor;
                    heightMapColors[x + y * width].B = (byte)tempColor;
                    heightMapColors[x + y * width].A = 255;
                    heightData[x, y] = tempHeight;
                }
            }
        }

        protected virtual void LoadHeightData(Texture2D heightMap)
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
                    heightData[x, y] = heightMapColors[x + y * width].R * heightColorFactor + minHeight;
                }
            }
        }

        protected void InitializeVertices()
        {
            vertices = new VertexPositionNormalTexture[width * height];
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    vertices[x + y * width].Position = new Vector3(x, heightData[x, y], y);
                    vertices[x + y * width].TextureCoordinate.X = (float)x / 30.0f;
                    vertices[x + y * width].TextureCoordinate.Y = (float)y / 30.0f;
                }
            }
        }

        protected virtual void InitializeIndices()
        {
            indices = new short[(width - 1) * (height - 1) * 6];
            int counter = 0;
            for (int x = 0; x < width - 1; x++)
            {
                for (int y = 0; y < height - 1; y++)
                {
                    int lowerLeft = y + x * width;
                    int lowerRight = (y + 1) + x * width;
                    int topLeft = y + (x + 1) * width;
                    int topRight = (y + 1) + (x + 1) * width;

                    indices[counter++] = (short)topLeft;
                    indices[counter++] = (short)lowerRight;
                    indices[counter++] = (short)lowerLeft;

                    indices[counter++] = (short)topLeft;
                    indices[counter++] = (short)topRight;
                    indices[counter++] = (short)lowerRight;
                }
            }
        }

        public void CalculateNormals()
        {
            for (int j = 0; j < vertices.Length; j++)
                vertices[j].Normal = new Vector3(0, 0, 0);

            for (int j = 0; j < indices.Length / 3; j++)
            {
                int index1 = indices[j * 3];
                int index2 = indices[j * 3 + 1];
                int index3 = indices[j * 3 + 2];

                if (index1 < 0 || index1 >= vertices.Length ||
                    index2 < 0 || index2 >= vertices.Length ||
                    index3 < 0 || index3 >= vertices.Length)
                    continue;

                Vector3 side1 = vertices[index1].Position - vertices[index3].Position;
                Vector3 side2 = vertices[index1].Position - vertices[index2].Position;
                Vector3 normal = Vector3.Cross(side1, side2);

                vertices[index1].Normal -= normal;
                vertices[index2].Normal -= normal;
                vertices[index3].Normal -= normal;
            }

            for (int j = 0; j < vertices.Length; j++)
                vertices[j].Normal.Normalize();
        }

        public virtual void CopyToBuffers()
        {
            indexBuffer = new IndexBuffer(GraphicsDevice, IndexElementSize.SixteenBits, indices.Length, BufferUsage.WriteOnly);
            indexBuffer.SetData(indices);

            vertexBuffer = new VertexBuffer(GraphicsDevice, VertexPositionNormalTexture.VertexDeclaration, vertices.Length, BufferUsage.WriteOnly);
            vertexBuffer.SetData(vertices);
        }

        public void Draw()
        {
            RasterizerState rs = new RasterizerState();
            rs.CullMode = CullMode.None;
            //rs.FillMode = FillMode.WireFrame;
            GraphicsDevice.RasterizerState = rs;
            GraphicsDevice.SetVertexBuffer(vertexBuffer);
            GraphicsDevice.Indices = indexBuffer;

            Vector3 lightDirection = Camera.Direction;//Vector3.Transform(new Vector3(0, 0, 1), Matrix.CreateFromQuaternion(camera.Rotation));// new Vector3(1.0f, -1.0f, 1.0f);
            //lightDirection.Normalize();

            if (effect != null)
            {
                effect.CurrentTechnique = effect.Techniques["Textured"];
                if (texture != null)
                    effect.Parameters["xTexture"].SetValue(texture);
                effect.Parameters["xView"].SetValue(Camera.World);
                effect.Parameters["xProjection"].SetValue(Camera.Projection);
                effect.Parameters["xWorld"].SetValue(Matrix.Identity);
                effect.Parameters["xLightDirection"].SetValue(lightDirection);
                effect.Parameters["xAmbient"].SetValue(0.1f);
                effect.Parameters["xEnableLighting"].SetValue(true);
                foreach (EffectPass pass in effect.CurrentTechnique.Passes)
                {
                    pass.Apply();
                    GraphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, vertices.Length, 0, indices.Length / 3);
                    //graphicsDevice.DrawUserIndexedPrimitives(PrimitiveType.TriangleList, vertices, 0, vertices.Length, indices, 0, indices.Length / 3, VertexPositionColorNormal.VertexDeclaration);
                }
            }
            else
            {
                basicEffect.World = Matrix.Identity;
                basicEffect.View = Camera.World;
                basicEffect.Projection = Camera.Projection;
                basicEffect.LightingEnabled = true;
                basicEffect.TextureEnabled = true;
                basicEffect.Texture = texture;
                foreach (EffectPass pass in basicEffect.CurrentTechnique.Passes)
                {
                    pass.Apply();
                    GraphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, vertices.Length, 0, indices.Length / 3);
                    //graphicsDevice.DrawUserIndexedPrimitives(PrimitiveType.TriangleList, vertices, 0, vertices.Length, indices, 0, indices.Length / 3, VertexPositionColor.VertexDeclaration);
                }
            }

            GraphicsDevice.RasterizerState = RasterizerState.CullCounterClockwise;
        }

        public void SaveHeightMap()
        {
            if (heightMap == null)
                heightMap = new Texture2D(GraphicsDevice, width, height, false, SurfaceFormat.Color);
            heightMap.SetData<Color>(heightMapColors);

            using (System.IO.Stream stream = System.IO.File.OpenWrite(@heightMapFile))
            {
                heightMap.SaveAsPng(stream, width, height);
            }
        }
    }
}
