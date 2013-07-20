using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

namespace EditorModel
{
    public class BoundingBoxBuffer : BaseObject
    {
        private ModelBoundingBox parent;

        public ModelBoundingBox Parent
        {
            get { return parent; }
            set { parent = value; }
        }

        private VertexBuffer vertexBuffer;
        private int vertexCount;
        private IndexBuffer indices;
        private int primitiveCount;

        private GraphicsDevice graphicsDevice;

        public GraphicsDevice GraphicsDevice
        {
            get { return graphicsDevice; }
            set
            {
                graphicsDevice = value;
                vertexBuffer = new VertexBuffer(graphicsDevice, typeof(VertexPositionColor), vertexCount, BufferUsage.WriteOnly);
            }
        }

        private BoundingBox boundingBox;

        public BoundingBox BoundingBox
        {
            get { return boundingBox; }
            set
            {
                boundingBox = value;

                List<VertexPositionColor> vertices = new List<VertexPositionColor>();

                float ratio = 5.0f;

                Vector3 xOffset = new Vector3((boundingBox.Max.X - boundingBox.Min.X) / ratio, 0, 0);
                Vector3 yOffset = new Vector3(0, (boundingBox.Max.Y - boundingBox.Min.Y) / ratio, 0);
                Vector3 zOffset = new Vector3(0, 0, (boundingBox.Max.Z - boundingBox.Min.Z) / ratio);
                Vector3[] corners = boundingBox.GetCorners();

                // Corner 1.
                AddVertex(vertices, corners[0]);
                AddVertex(vertices, corners[0] + xOffset);
                AddVertex(vertices, corners[0]);
                AddVertex(vertices, corners[0] - yOffset);
                AddVertex(vertices, corners[0]);
                AddVertex(vertices, corners[0] - zOffset);

                // Corner 2.
                AddVertex(vertices, corners[1]);
                AddVertex(vertices, corners[1] - xOffset);
                AddVertex(vertices, corners[1]);
                AddVertex(vertices, corners[1] - yOffset);
                AddVertex(vertices, corners[1]);
                AddVertex(vertices, corners[1] - zOffset);

                // Corner 3.
                AddVertex(vertices, corners[2]);
                AddVertex(vertices, corners[2] - xOffset);
                AddVertex(vertices, corners[2]);
                AddVertex(vertices, corners[2] + yOffset);
                AddVertex(vertices, corners[2]);
                AddVertex(vertices, corners[2] - zOffset);

                // Corner 4.
                AddVertex(vertices, corners[3]);
                AddVertex(vertices, corners[3] + xOffset);
                AddVertex(vertices, corners[3]);
                AddVertex(vertices, corners[3] + yOffset);
                AddVertex(vertices, corners[3]);
                AddVertex(vertices, corners[3] - zOffset);

                // Corner 5.
                AddVertex(vertices, corners[4]);
                AddVertex(vertices, corners[4] + xOffset);
                AddVertex(vertices, corners[4]);
                AddVertex(vertices, corners[4] - yOffset);
                AddVertex(vertices, corners[4]);
                AddVertex(vertices, corners[4] + zOffset);

                // Corner 6.
                AddVertex(vertices, corners[5]);
                AddVertex(vertices, corners[5] - xOffset);
                AddVertex(vertices, corners[5]);
                AddVertex(vertices, corners[5] - yOffset);
                AddVertex(vertices, corners[5]);
                AddVertex(vertices, corners[5] + zOffset);

                // Corner 7.
                AddVertex(vertices, corners[6]);
                AddVertex(vertices, corners[6] - xOffset);
                AddVertex(vertices, corners[6]);
                AddVertex(vertices, corners[6] + yOffset);
                AddVertex(vertices, corners[6]);
                AddVertex(vertices, corners[6] + zOffset);

                // Corner 8.
                AddVertex(vertices, corners[7]);
                AddVertex(vertices, corners[7] + xOffset);
                AddVertex(vertices, corners[7]);
                AddVertex(vertices, corners[7] + yOffset);
                AddVertex(vertices, corners[7]);
                AddVertex(vertices, corners[7] + zOffset);

                vertexBuffer.SetData(vertices.ToArray());

                IndexBuffer indexBuffer = new IndexBuffer(graphicsDevice, IndexElementSize.SixteenBits, vertexCount, BufferUsage.WriteOnly);
                indexBuffer.SetData(Enumerable.Range(0, vertexCount).Select(i => (short)i).ToArray());
                indices = indexBuffer;

                //axis
                ratio = 3;
                xOffset = new Vector3(ratio, 0, 0);
                yOffset = new Vector3(0, ratio, 0);
                zOffset = new Vector3(0, 0, ratio);
            }
        }

        private static void AddVertex(List<VertexPositionColor> vertices, Vector3 position)
        {
            vertices.Add(new VertexPositionColor(position, Color.Green));
        }

        public BoundingBoxBuffer(GraphicsDevice graphicsDevice) : base()
        {
            primitiveCount = 24;
            vertexCount = 48;
            this.graphicsDevice = graphicsDevice;
            vertexBuffer = new VertexBuffer(graphicsDevice, typeof(VertexPositionColor), vertexCount, BufferUsage.WriteOnly);
        }

        public void Draw(BasicEffect effect)
        {
            graphicsDevice.SetVertexBuffer(vertexBuffer);
            graphicsDevice.Indices = indices;
            effect.World = world;

            effect.VertexColorEnabled = true;

            DepthStencilState d = new DepthStencilState();
            d.DepthBufferEnable = false;

            foreach (EffectPass pass in effect.CurrentTechnique.Passes)
            {
                pass.Apply();
                graphicsDevice.DrawIndexedPrimitives(PrimitiveType.LineList, 0, 0, vertexCount, 0, primitiveCount);
            }

            effect.World = Matrix.Identity;
            graphicsDevice.SetVertexBuffer(null);
            graphicsDevice.Indices = null;
            effect.World = Matrix.Identity;
        }
    }
}
