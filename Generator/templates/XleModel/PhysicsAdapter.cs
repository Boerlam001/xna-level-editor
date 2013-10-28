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
    public abstract class PhysicsAdapter : BaseObject
    {
        protected RigidBody body;
        protected CharacterController characterController;
        private float tempMass;
        private float tempRestitution;
        private float tempKinecticFriction;

        public RigidBody Body
        {
            get { return body; }
            set
            {
                body = value;
                Update(null);
            }
        }

        public CharacterController CharacterController
        {
            get { return characterController; }
            set { characterController = value; }
        }

        public PhysicsAdapter(Game game, BaseObject parent, World physicsWorld) : base(game, physicsWorld)
        {
            this.parent = parent;
            Position = parent.Position;
            Rotation = parent.Rotation;
        }

        public PhysicsAdapter(Game game, BaseObject parent, World physicsWorld, RigidBody body)
            : this(game, parent, physicsWorld)
        {
            Body = body;
            if (!physicsWorld.RigidBodies.Contains(body) && parent.PhysicsEnabled)
                physicsWorld.AddBody(body);
        }

        public override void Update(GameTime gameTime)
        {
            position = Helper.ToXNAVector(body.Position);
            Matrix rotationMatrix = Helper.ToXNAMatrix(body.Orientation);
            Vector3 scale, translation;
            rotationMatrix.Decompose(out scale, out rotation, out translation);

            base.Update(gameTime);
        }

        public void EnableCharacterController()
        {
            if (characterController != null)
                return;

            DisableCharacterController();

            tempMass = body.Mass;
            tempRestitution = body.Material.Restitution;
            tempKinecticFriction = body.Material.KineticFriction;

            body.SetMassProperties(JMatrix.Zero, 1.0f, true);
            body.Material.Restitution = 0.0f;
            body.Material.KineticFriction = 0.0f;

            characterController = new CharacterController(physicsWorld, body);
            characterController.Stiff = 0.25f;
            
            if (parent.PhysicsEnabled)
                physicsWorld.AddConstraint(characterController);
        }

        public void DisableCharacterController()
        {
            if (characterController != null)
            {
                body.Mass = tempMass;
                body.Material.Restitution = tempRestitution;
                body.Material.KineticFriction = tempKinecticFriction;

                physicsWorld.RemoveConstraint(characterController);
            }
        }
    }
}
