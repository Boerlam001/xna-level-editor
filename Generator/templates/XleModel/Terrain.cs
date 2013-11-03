using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Jitter;
using Jitter.Collision;
using Jitter.Collision.Shapes;
using Jitter.Dynamics;
using Jitter.LinearMath;

namespace XleModel
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
        #region encapsulated attributes
        private string heightMapFile;
        private string effectFile;
        private RigidBody body;
        private int width;
        private int height;
        private float[,] heightData;
        private VertexPositionNormalTexture[] vertices;
        private short[] indices;
        private VertexBuffer vertexBuffer;
        private Color[] heightMapColors;
        private Texture2D heightMap;
        private Effect effect;
        private BasicEffect basicEffect;
        private float heightColorFactor;
        Texture2D texture;
        #endregion
        #region unencapsulated attributes
        private GraphicsDevice graphicsDevice;
        private IndexBuffer indexBuffer;
        private float minHeight;
        private float maxHeight;
        private Camera camera;
        #endregion
        #endregion

        #region getters and setters
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

        public float HeightColorFactor
        {
            get { return heightColorFactor; }
            set { heightColorFactor = value; }
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

        public float[,] HeightData
        {
            get { return heightData; }
            set { heightData = value; }
        }

        public string HeightMapFile
        {
            get { return heightMapFile; }
            set { heightMapFile = value; }
        }

        public RigidBody Body
        {
            get { return body; }
            set { body = value; }
        }

        public Effect Effect
        {
            get { return effect; }
            set { effect = value; }
        }

        public Texture2D Texture
        {
            get { return texture; }
            set { texture = value; }
        }

        public Color[] HeightMapColors
        {
            get { return heightMapColors; }
            set { heightMapColors = value; }
        }
        #endregion

        public Terrain(GraphicsDevice graphicsDevice, Camera camera, Texture2D heightMap, Game game, bool initialize = true)
            : base(game)
        {
            this.graphicsDevice = graphicsDevice;
            this.camera = camera;
            this.heightMap = heightMap;
            heightColorFactor = 0.2f;
            minHeight = -255 * heightColorFactor / 2;
            maxHeight = minHeight * -1;
            basicEffect = new BasicEffect(graphicsDevice);
            if (initialize)
                InitializeAll(heightMap);
        }

        public Terrain(GraphicsDevice graphicsDevice, Camera camera, int width, int height, Game game, bool initialize = true)
            : base(game)
        {
            this.graphicsDevice = graphicsDevice;
            this.camera = camera;
            this.width = width;
            this.height = height;
            heightColorFactor = 0.2f;
            minHeight = -255 * heightColorFactor / 2;
            maxHeight = minHeight * -1;
            basicEffect = new BasicEffect(graphicsDevice);
            if (initialize)
                InitializeAll();
        }
        
        public Terrain(GraphicsDevice graphicsDevice, Camera camera, Game game, World world)
            : this(graphicsDevice, camera, 128, 128, game)
        {
            TerrainShape terrainShape = new TerrainShape(heightData, 1, 1);
            body = new RigidBody(terrainShape);
            body.Position = new JVector();
            body.IsStatic = true;
            body.Material.KineticFriction = 0.0f;
            world.AddBody(body);
        }

        public Terrain(GraphicsDevice graphicsDevice, Camera camera, Texture2D heightMap, Game game, World world)
            : this(graphicsDevice, camera, heightMap, game)
        {
            TerrainShape terrainShape = new TerrainShape(heightData, 1, 1);
            body = new RigidBody(terrainShape);
            body.Position = new JVector();
            body.IsStatic = true;
            body.Material.KineticFriction = 0.0f;
            world.AddBody(body);
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

        protected virtual void InitializeVertices()
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

        public virtual void CalculateNormals()
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

        public void CopyToBuffers()
        {
            vertexBuffer = new VertexBuffer(graphicsDevice, VertexPositionNormalTexture.VertexDeclaration, vertices.Length, BufferUsage.WriteOnly);
            vertexBuffer.SetData(vertices);

            indexBuffer = new IndexBuffer(graphicsDevice, IndexElementSize.SixteenBits, indices.Length, BufferUsage.WriteOnly);
            indexBuffer.SetData(indices);
        }

        public override void Draw(GameTime gameTime)
        {
            RasterizerState rs = new RasterizerState();
            rs.CullMode = CullMode.None;
            //rs.FillMode = FillMode.WireFrame;
            graphicsDevice.RasterizerState = rs;
            graphicsDevice.SetVertexBuffer(vertexBuffer);
            graphicsDevice.Indices = indexBuffer;

            Vector3 lightDirection = new Vector3(1.0f, -1.0f, -1.0f);
            lightDirection.Normalize();

            if (effect != null)
            {
                if (texture != null)
                {
                    effect.CurrentTechnique = effect.Techniques["Textured"];
                    effect.Parameters["xTexture"].SetValue(texture);
                }
                else
                    effect.CurrentTechnique = effect.Techniques["Colored"];
                effect.Parameters["xView"].SetValue(camera.World);
                effect.Parameters["xProjection"].SetValue(camera.Projection);
                effect.Parameters["xWorld"].SetValue(Matrix.Identity);
                effect.Parameters["xLightDirection"].SetValue(lightDirection);
                effect.Parameters["xAmbient"].SetValue(0.1f);
                effect.Parameters["xEnableLighting"].SetValue(true);
                foreach (EffectPass pass in effect.CurrentTechnique.Passes)
                {
                    pass.Apply();
                    graphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, vertices.Length, 0, indices.Length / 3);
                    //graphicsDevice.DrawUserIndexedPrimitives(PrimitiveType.TriangleList, vertices, 0, vertices.Length, indices, 0, indices.Length / 3, VertexPositionColorNormal.VertexDeclaration);
                }
            }
            else
            {
                basicEffect.World = Matrix.Identity;
                basicEffect.View = camera.World;
                basicEffect.Projection = camera.Projection;
                basicEffect.LightingEnabled = true;
                basicEffect.DirectionalLight0.Enabled = true;
                basicEffect.DirectionalLight0.Direction = lightDirection;
                basicEffect.AmbientLightColor = new Vector3(0.1f);

                if (texture != null)
                {
                    basicEffect.TextureEnabled = true;
                    basicEffect.Texture = texture;
                    basicEffect.VertexColorEnabled = false;
                }
                else
                {
                    basicEffect.VertexColorEnabled = true;
                }
                foreach (EffectPass pass in basicEffect.CurrentTechnique.Passes)
                {
                    pass.Apply();
                    graphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, vertices.Length, 0, indices.Length / 3);
                    //graphicsDevice.DrawUserIndexedPrimitives(PrimitiveType.TriangleList, vertices, 0, vertices.Length, indices, 0, indices.Length / 3, VertexPositionColor.VertexDeclaration);
                }
            }

            graphicsDevice.RasterizerState = RasterizerState.CullCounterClockwise;

            base.Draw(gameTime);
        }
    }
}
