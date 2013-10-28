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
    public class CapsuleAdapter : PhysicsAdapter
    {
        public CapsuleAdapter(Game game, BaseObject parent, World physicsWorld, Vector3 relativePosition, float length, float radius)
            : base(game, parent, physicsWorld)
        {
            CapsuleShape shape = new CapsuleShape(length, radius);

            body = new RigidBody(shape);
            RelativePosition = relativePosition;
            Rotation = rotation;
            //body.Position = Helper.ToJitterVector(position);
            //body.Orientation = Helper.ToJitterMatrix(Matrix.CreateFromQuaternion(rotation));

            physicsWorld.AddBody(body);
        }
    }
}
