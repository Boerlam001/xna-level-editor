using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace EditorModel.QuadTreeTerrain
{
    //reference: http://www.dustinhorne.com/post/2011/08/28/XNA-Terrain-with-LOD-Part-3-Structuring-the-QuadTree
    public class QuadTree
    {
        private QuadNode _rootNode;
        private TreeVertexCollection _vertices;
        private BufferManager _buffers;
        private Vector3 _position;
        private int _topNodeSize;

        private Vector3 _cameraPosition;
        private Vector3 _lastCameraPosition;

        public short[] Indices;

        public Matrix View;
        public Matrix Projection;

        public GraphicsDevice Device;

        public int TopNodeSize { get { return _topNodeSize; } }
        public QuadNode RootNode { get { return _rootNode; } }
        public TreeVertexCollection Vertices { get { return _vertices; } }

        public BasicEffect Effect;
        private Camera camera;
        public int MinimumDepth;

        private QuadNode _activeNode;

        internal int IndexCount { get; set; }

        public Vector3 CameraPosition
        {
            get { return _cameraPosition; }
            set { _cameraPosition = value; }
        }

        internal BoundingFrustum ViewFrustrum { get; set; }

        public QuadTree(Vector3 position, int width, int height, Camera camera, GraphicsDevice device, int scale)
        {
            Device = device;
            _position = position;
            _topNodeSize = (width - 1);

            _vertices = new TreeVertexCollection(position, width, scale);
            _buffers = new BufferManager(_vertices.Vertices, device);
            _rootNode = new QuadNode(NodeType.FullNode, _topNodeSize, 1, null, this, 0);
            this.camera = camera;
            View = camera.World;
            Projection = camera.Projection;

            ViewFrustrum = new BoundingFrustum(View * Projection);

            //Construct an array large enough to hold all of the indices we'll need.
            Indices = new short[((width + 1) * (height + 1)) * 3];


            Effect = new BasicEffect(device);
            Effect.EnableDefaultLighting();
            Effect.FogEnabled = true;
            Effect.FogStart = 300f;
            Effect.FogEnd = 1000f;
            Effect.FogColor = Color.Black.ToVector3();
            Effect.TextureEnabled = true;
            Effect.Texture = new Texture2D(device, 100, 100);
            Effect.Projection = Projection;
            Effect.View = View;
            Effect.World = Matrix.Identity;
        }

        public QuadTree(Vector3 position, Texture2D heightMap, Camera camera, GraphicsDevice device, int scale)
        {
            Device = device;
            _position = position;
            _topNodeSize = (heightMap.Width - 1);

            _vertices = new TreeVertexCollection(position, heightMap, scale);
            _buffers = new BufferManager(_vertices.Vertices, device);
            _rootNode = new QuadNode(NodeType.FullNode, _topNodeSize, 1, null, this, 0);
            this.camera = camera;
            View = camera.World;
            Projection = camera.Projection;

            ViewFrustrum = new BoundingFrustum(View * Projection);

            //Construct an array large enough to hold all of the indices we'll need.
            Indices = new short[((heightMap.Width + 1) * (heightMap.Height + 1)) * 3];


            Effect = new BasicEffect(device);
            Effect.EnableDefaultLighting();
            Effect.FogEnabled = true;
            Effect.FogStart = 300f;
            Effect.FogEnd = 1000f;
            Effect.FogColor = Color.Black.ToVector3();
            Effect.TextureEnabled = true;
            Effect.Texture = new Texture2D(device, 100, 100);
            Effect.Projection = Projection;
            Effect.View = View;
            Effect.World = Matrix.Identity;
        }

        public void Update(GameTime gameTime)
        {
            View = camera.World;
            Projection = camera.Projection;
            CameraPosition = camera.Position;

            //Only update if the camera position has changed
            if (_cameraPosition == _lastCameraPosition)
                return;

            //Effect.View = View;
            //Effect.Projection = Projection;

            _lastCameraPosition = _cameraPosition;
            IndexCount = 0;

            _rootNode.EnforceMinimumDepth();

            _activeNode = _rootNode.DeepestNodeWithPoint(CameraPosition);

            if (_activeNode != null)
            {
                _activeNode.Split();
            }

            _rootNode.SetActiveVertices();

            _buffers.UpdateIndexBuffer(Indices, IndexCount);
            _buffers.SwapBuffer();
        }

        public void Draw(GameTime gameTime)
        {
            Device.SetVertexBuffer(_buffers.VertexBuffer);
            Device.Indices = _buffers.IndexBuffer;

            Effect.View = View;
            Effect.Projection = Projection;

            RasterizerState rs = new RasterizerState();
            rs.CullMode = CullMode.None;
            rs.FillMode = FillMode.WireFrame;
            Device.RasterizerState = rs;

            foreach (EffectPass pass in Effect.CurrentTechnique.Passes)
            {
                pass.Apply();
                Device.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, _vertices.Vertices.Length, 0, IndexCount / 3);
            }

            Device.RasterizerState = RasterizerState.CullCounterClockwise;
        }

        internal void UpdateBuffer(short vIndex)
        {
            Indices[IndexCount] = vIndex;
            IndexCount++;
        }
    }
}
