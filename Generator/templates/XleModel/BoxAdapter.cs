using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Jitter;
using Jitter.Collision;
using Jitter.Collision.Shapes;
using Jitter.Dynamics;
using Jitter.LinearMath;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace XleModel
{
    public class BoxAdapter : PhysicsAdapter
    {
        Vector3 size;

        public BoxAdapter(Game game, BaseObject parent, World physicsWorld, Vector3 relativePosition, Vector3 size)
            : base(game, parent, physicsWorld)
        {
            this.size = size;

            Shape shape = new BoxShape(size.X, size.Y, size.Z);
            
            body = new RigidBody(shape);
            RelativePosition = relativePosition;
            Rotation = rotation;
            //body.Position = Helper.ToJitterVector(position);
            //body.Orientation = Helper.ToJitterMatrix(Matrix.CreateFromQuaternion(rotation));

            physicsWorld.AddBody(body);
        }
    }
}
