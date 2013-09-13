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
        #region attributes
        #region encapsulated attributes
        private string heightMapFile;
        private string effectFile;
        #endregion
        #region unencapsulated attributes
        #endregion
        #endregion

        #region getters and setters
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
        #endregion

        public Terrain(GraphicsDevice graphicsDevice, Camera camera)
            : base(graphicsDevice, camera, 128, 128)
        {
        }

        public Terrain(GraphicsDevice graphicsDevice, Camera camera, Texture2D heightMap)
            : base(graphicsDevice, camera, heightMap)
        {
        }
    }
}
