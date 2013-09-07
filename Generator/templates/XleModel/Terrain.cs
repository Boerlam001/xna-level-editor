using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace XleModel
{
    public class Terrain : BaseTerrain
    {
        public Terrain(GraphicsDevice graphicsDevice)
            : base(graphicsDevice, 128, 128)
        {
        }

        public Terrain(GraphicsDevice graphicsDevice, Texture2D heightMap)
            : base(graphicsDevice, heightMap)
        {
        }
    }
}
