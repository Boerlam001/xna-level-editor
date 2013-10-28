using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace EditorModel.QuadTreeTerrain
{
    //reference: http://www.dustinhorne.com/post/2011/08/25/XNA-Terrain-with-LOD-Part-2-Creating-the-Data-Objects
    internal class BufferManager
    {
        int _active = 0;
        internal VertexBuffer VertexBuffer;
        IndexBuffer[] _IndexBuffers;
        GraphicsDevice _device;

        internal BufferManager(VertexPositionNormalTexture[] vertices, GraphicsDevice device)
        {
            _device = device;

            VertexBuffer = new VertexBuffer(device, VertexPositionNormalTexture.VertexDeclaration, vertices.Length, BufferUsage.WriteOnly);
            VertexBuffer.SetData(vertices);

            _IndexBuffers = new IndexBuffer[]
                    {
                            new IndexBuffer(_device, IndexElementSize.SixteenBits, 100000, BufferUsage.WriteOnly),
                            new IndexBuffer(_device, IndexElementSize.SixteenBits, 100000, BufferUsage.WriteOnly)
                    };

        }


        internal IndexBuffer IndexBuffer
        {
            get { return _IndexBuffers[_active]; }
        }

        internal void UpdateIndexBuffer(short[] indices, int indexCount)
        {
            int inactive = _active == 0 ? 1 : 0;

            _IndexBuffers[inactive].SetData(indices, 0, indexCount);

        }

        internal void SwapBuffer()
        {
            _active = _active == 0 ? 1 : 0; ;
        }
    }
}
