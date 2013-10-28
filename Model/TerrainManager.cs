using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace EditorModel
{
    public class TerrainManager
    {
        Terrain[] terrain;

        public TerrainManager(GraphicsDevice graphicsDevice, Camera camera, string heightMapFile, string effectFile)
        {
        }

        public TerrainManager(GraphicsDevice graphicsDevice, Camera camera, Texture2D heightMap, string heightMapFile, string effectFile)
        {
        }

        public TerrainManager(GraphicsDevice graphicsDevice, Camera camera, string heightMapFile, string effectFile, int width, int height)
        {
        }
    }
}
