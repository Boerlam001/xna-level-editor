using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace EditorModel
{
    public class Terrain : BaseTerrain
    {
        private TerrainIndexer terrainIndexer;

        public TerrainIndexer TerrainIndexer
        {
            get { return terrainIndexer; }
            set { terrainIndexer = value; }
        }

        private string effectFile;

        public string EffectFile
        {
            get { return effectFile; }
            set { effectFile = value; }
        }

        private string heightMapFile;

        public string HeightMapFile
        {
            get { return heightMapFile; }
            set { heightMapFile = value; }
        }

        public Terrain(GraphicsDevice graphicsDevice)
            : base(graphicsDevice, 128, 128)
        {
        }

        public Terrain(GraphicsDevice graphicsDevice, Texture2D heightMap)
            : base(graphicsDevice, heightMap)
        {
        }

        public void SaveHeightMap()
        {
            if (heightMap == null)
                heightMap = new Texture2D(graphicsDevice, width, height, false, SurfaceFormat.Color);
            heightMap.SetData<Color>(heightMapColors);

            using (System.IO.Stream stream = System.IO.File.OpenWrite(@heightMapFile))
            {
                heightMap.SaveAsPng(stream, width, height);
            }
        }
    }
}
