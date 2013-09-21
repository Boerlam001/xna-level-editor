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
    public class Terrain : BaseTerrain
    {
        #region attributes
        #region encapsulated attributes
        private string heightMapFile;
        private string effectFile;
        private RigidBody body;
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

        public RigidBody Body
        {
            get { return body; }
            set { body = value; }
        }
        #endregion

        public Terrain(GraphicsDevice graphicsDevice, Camera camera, Game game, World world)
            : base(graphicsDevice, camera, 128, 128, game)
        {
            TerrainShape terrainShape = new TerrainShape(heightData, 1, 1);
            body = new RigidBody(terrainShape);
            body.Position = new JVector();
            body.IsStatic = true;
            body.Material.KineticFriction = 0.0f;
            world.AddBody(body);
        }

        public Terrain(GraphicsDevice graphicsDevice, Camera camera, Texture2D heightMap, Game game, World world)
            : base(graphicsDevice, camera, heightMap, game)
        {
            TerrainShape terrainShape = new TerrainShape(heightData, 1, 1);
            body = new RigidBody(terrainShape);
            body.Position = new JVector();
            body.IsStatic = true;
            body.Material.KineticFriction = 0.0f;
            world.AddBody(body);
        }
    }
}
