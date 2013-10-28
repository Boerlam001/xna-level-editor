using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace XleModel
{
    public abstract class ControllerScript
    {
        public Game Game
        {
            get;
            set;
        }

        public BaseObject Parent
        {
            get;
            set;
        }

        public abstract void Start();

        public abstract void Update(GameTime gameTime);

        public virtual void LateUpdate()
        {
        }

        public virtual void CollisionDetected(BaseObject other)
        {

        }
    }
}
