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
    public class ConvexHullAdapter : PhysicsAdapter
    {
        Model drawingModel;

        public ConvexHullAdapter(Game game, BaseObject parent, World physicsWorld)
            : base(game, parent, physicsWorld)
        {
            this.drawingModel = (parent as DrawingObject).DrawingModel;
            
            ConvexHullShape shape = ConvexHullHelper.BuildConvexHullShape(drawingModel, parent.Scale);

            body = new RigidBody(shape);

            DrawingObject obj = parent as DrawingObject;
            BoundingBox parentBoundingBox = obj.CreateBoundingBox();
            Vector3 parentShift = obj.Center - new Vector3(0, parentBoundingBox.Min.Y, 0);

            //RelativePosition = Vector3.Zero;
            RelativePosition = -Helper.ToXNAVector(shape.Shift) - parentShift;
            Rotation = rotation;

            //body.Position = Helper.ToJitterVector(position);
            //body.Orientation = Helper.ToJitterMatrix(Matrix.CreateFromQuaternion(rotation));
            
            physicsWorld.AddBody(body);

            //((DrawingObject)parent).Center = -Helper.ToXNAVector(shape.Shift);
        }

        public ConvexHullAdapter(Game game, BaseObject parent, World physicsWorld, Vector3 relativePosition)
            : base(game, parent, physicsWorld)
        {
            this.drawingModel = (parent as DrawingObject).DrawingModel;

            ConvexHullShape shape = ConvexHullHelper.BuildConvexHullShape(drawingModel, parent.Scale);

            body = new RigidBody(shape);

            //DrawingObject obj = parent as DrawingObject;
            //BoundingBox parentBoundingBox = obj.CreateBoundingBox();
            //Vector3 parentShift = obj.Center - new Vector3(0, parentBoundingBox.Min.Y, 0);

            //RelativePosition = Vector3.Zero;
            RelativePosition = relativePosition;
            Rotation = rotation;

            //body.Position = Helper.ToJitterVector(position);
            //body.Orientation = Helper.ToJitterMatrix(Matrix.CreateFromQuaternion(rotation));

            physicsWorld.AddBody(body);

            //((DrawingObject)parent).Center = -Helper.ToXNAVector(shape.Shift);
        }

        public ConvexHullAdapter(Game game, BaseObject parent, World physicsWorld, Model drawingModel, RigidBody body, bool hasCharacterController = false)
            : base(game, parent, physicsWorld, body)
        {
            this.drawingModel = drawingModel;

            ConvexHullShape shape = ConvexHullHelper.BuildConvexHullShape(drawingModel);
            body.Shape = shape;

            //((DrawingObject)parent).Center = -Helper.ToXNAVector(shape.Shift);
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            if (body.Shape is ConvexHullShape)
            {
                ConvexHullShape convexHullShape = body.Shape as ConvexHullShape;
                //((DrawingObject)parent).Center = -Helper.ToXNAVector(convexHullShape.Shift);
            }
        }
    }
}
