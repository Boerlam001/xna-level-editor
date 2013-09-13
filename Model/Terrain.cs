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
        #region attributes
        #region encapsulated attributes
        private TerrainIndexer terrainIndexer;
        private string heightMapFile;
        private string effectFile;
        #endregion
        #region unencapsulated attributes
        #endregion
        #endregion

        #region getters and setters
        public TerrainIndexer TerrainIndexer
        {
            get { return terrainIndexer; }
            set { terrainIndexer = value; }
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
        #endregion

        public Terrain(GraphicsDevice graphicsDevice, Camera camera, string heightMapFile, string effectFile)
            : base(graphicsDevice, camera, 128, 128)
        {
            terrainIndexer = new TerrainIndexer(this, camera, graphicsDevice);
            this.heightMapFile = heightMapFile;
            this.effectFile = effectFile;
        }

        public Terrain(GraphicsDevice graphicsDevice, Camera camera, Texture2D heightMap, string heightMapFile, string effectFile)
            : base(graphicsDevice, camera, heightMap)
        {
            terrainIndexer = new TerrainIndexer(this, camera, graphicsDevice);
            this.heightMapFile = heightMapFile;
            this.effectFile = effectFile;
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
