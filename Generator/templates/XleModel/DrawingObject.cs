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
    public class DrawingObject : BaseObject
    {
        #region attributes
        private Model drawingModel;
        private Matrix[] boneTransforms;
        private string sourceFile;
        private Vector3 center;
        private RigidBody body;
        private World physicsWorld;
        class BodyGeoEventArgs : EventArgs
        {
            public bool SetBodyGeo { get; set; }
            public BodyGeoEventArgs(bool setBodyGeo = true)
            {
                SetBodyGeo = setBodyGeo;
            }
        }
        #endregion

        #region getters and setters
        public Vector3 Center
        {
            get { return center; }
            set
            {
                center = value;
                OnPositionChanged(this, null);
            }
        }

        public Matrix[] BoneTransforms
        {
            get { return boneTransforms; }
            set { boneTransforms = value; }
        }

        public string SourceFile
        {
            get { return sourceFile; }
            set { sourceFile = value; }
        }

        public Model DrawingModel
        {
            get { return drawingModel; }
            set
            {
                drawingModel = value;
                if (drawingModel != null)
                {
                    MeasureModel();
                    ConvexHullShape shape = ConvexHullHelper.BuildConvexHullShape(drawingModel);
                    body = new RigidBody(shape);
                    body.Position = Helper.ToJitterVector(position);
                    body.Orientation = Helper.ToJitterMatrix(Matrix.CreateFromQuaternion(rotation));
                    Center = -Helper.ToXNAVector(shape.Shift);
                    physicsWorld.AddBody(body);
                }
            }
        }

        public RigidBody Body
        {
            get { return body; }
            set { body = value; }
        }
        #endregion

        public DrawingObject(Game game, World physicsWorld)
            : base(game)
        {
            this.physicsWorld = physicsWorld;
        }

        public override void Update(GameTime gameTime)
        {
            ConvexHullShape convexHullShape = body.Shape as ConvexHullShape;
            position = Helper.ToXNAVector(body.Position);
            Matrix rotationMatrix = Helper.ToXNAMatrix(body.Orientation);
            Vector3 scale, translation;
            rotationMatrix.Decompose(out scale, out rotation, out translation);
            center = -Helper.ToXNAVector(convexHullShape.Shift);
            OnRotationChanged(this, new BodyGeoEventArgs(false));
            OnPositionChanged(this, new BodyGeoEventArgs(false));
            base.Update(gameTime);
        }

        public void Draw(Matrix view, Matrix projection, bool lightDirectionEnabled = false, Vector3 lightDirection = new Vector3())
        {
            if (drawingModel == null)
                return;
            foreach (ModelMesh mesh in drawingModel.Meshes)
            {
                foreach (BasicEffect effect in mesh.Effects)
                {
                    effect.World = boneTransforms[mesh.ParentBone.Index] * world;
                    effect.View = view;
                    effect.Projection = projection;
                    effect.SpecularPower = 16;
                    if (lightDirectionEnabled)
                    {
                        effect.DirectionalLight0.Enabled = lightDirectionEnabled;
                        effect.DirectionalLight0.Direction = lightDirection;
                    }
                    else
                    {
                        effect.EnableDefaultLighting();
                        effect.PreferPerPixelLighting = true;
                    }
                }
                mesh.Draw();
            }
        }

        void MeasureModel()
        {
            // Look up the absolute bone transforms for this model.
            boneTransforms = new Matrix[drawingModel.Bones.Count];

            drawingModel.CopyAbsoluteBoneTransformsTo(boneTransforms);

            // Compute an (approximate) model center position by
            // averaging the center of each mesh bounding sphere.

            Vector3 centerTemp = new Vector3();
            foreach (ModelMesh mesh in drawingModel.Meshes)
            {
                BoundingSphere meshBounds = mesh.BoundingSphere;
                Matrix transform = boneTransforms[mesh.ParentBone.Index];
                Vector3 meshCenter = Vector3.Transform(meshBounds.Center, transform);
            
                centerTemp += meshCenter;
            }
            centerTemp /= drawingModel.Meshes.Count;
            
            center = centerTemp;
            position += centerTemp;

            // Now we know the center point, we can compute the model radius
            // by examining the radius of each mesh bounding sphere.
            //modelRadius = 0;
            //
            //foreach (ModelMesh mesh in drawingModel.Meshes)
            //{
            //    BoundingSphere meshBounds = mesh.BoundingSphere;
            //    Matrix transform = boneTransforms[mesh.ParentBone.Index];
            //    Vector3 meshCenter = Vector3.Transform(meshBounds.Center, transform);
            //
            //    float transformScale = transform.Forward.Length();
            //
            //    float meshRadius = (meshCenter - modelCenter).Length() +
            //                       (meshBounds.Radius * transformScale);
            //
            //    modelRadius = Math.Max(modelRadius, meshRadius);
            //}
        }

        public bool RayIntersects(Ray ray)
        {
            foreach (ModelMesh mesh in drawingModel.Meshes)
            {
                BoundingSphere sphere = mesh.BoundingSphere.Transform(boneTransforms[mesh.ParentBone.Index] * world);//TransformBoundingSphere(mesh.BoundingSphere, boneTransforms[mesh.ParentBone.Index] * world);
                if (sphere.Intersects(ray) != null)
                    return true;
            }
            return false;
        }

        private static BoundingSphere TransformBoundingSphere(BoundingSphere sphere, Matrix transform)
        {
            BoundingSphere transformedSphere;
            Vector3 scale3 = new Vector3(sphere.Radius, sphere.Radius, sphere.Radius);
            scale3 = Vector3.TransformNormal(scale3, transform);
            transformedSphere.Radius = Math.Max(scale3.X, Math.Max(scale3.Y, scale3.Z));
            transformedSphere.Center = Vector3.Transform(sphere.Center, transform);

            return transformedSphere;
        }

        private static BoundingBox? GetBoundingBox(ModelMeshPart meshPart, Matrix transform)
        {
            if (meshPart.VertexBuffer == null)
                return null;

            VertexDeclaration vd = meshPart.VertexBuffer.VertexDeclaration;
            VertexElement[] elements = vd.GetVertexElements();
            VertexElementUsage usage = VertexElementUsage.Position;

            Func<VertexElement, bool> elementPredicate = ve => ve.VertexElementUsage == usage && ve.VertexElementFormat == VertexElementFormat.Vector3;
            if (!elements.Any(elementPredicate))
                return null;

            VertexElement element = elements.First(elementPredicate);

            Vector3[] positions = new Vector3[meshPart.NumVertices];
            meshPart.VertexBuffer.GetData((meshPart.VertexOffset * vd.VertexStride) + element.Offset, positions, 0, positions.Length, vd.VertexStride);

            if (positions == null)
                return null;

            Vector3[] transformedPositions = new Vector3[positions.Length];
            Vector3.Transform(positions, ref transform, transformedPositions);

            return BoundingBox.CreateFromPoints(transformedPositions);
        }

        public BoundingBox CreateBoundingBox()
        {
            BoundingBox result = new BoundingBox();
            foreach (ModelMesh mesh in drawingModel.Meshes)
                foreach (ModelMeshPart meshPart in mesh.MeshParts)
                {
                    BoundingBox? meshPartBoundingBox = GetBoundingBox(meshPart, boneTransforms[mesh.ParentBone.Index]);
                    if (meshPartBoundingBox != null)
                        result = BoundingBox.CreateMerged(result, meshPartBoundingBox.Value);
                }
            return result;
        }

        protected override void OnRotationChanged(object sender, EventArgs e)
        {
            base.OnRotationChanged(sender, e);
            Vector3 v = Vector3.Transform(center, rotation);
            world = Matrix.CreateScale(scale) * Matrix.CreateFromQuaternion(rotation) * Matrix.CreateTranslation(position - v);
            if (body != null && (e == null || (e != null && e is BodyGeoEventArgs && (e as BodyGeoEventArgs).SetBodyGeo)))
                body.Orientation = Helper.ToJitterMatrix(Matrix.CreateFromQuaternion(rotation));
        }

        protected override void OnPositionChanged(object sender, EventArgs e)
        {
            base.OnPositionChanged(sender, e);
            Vector3 v = Vector3.Transform(center, rotation);
            world = Matrix.CreateScale(scale) * Matrix.CreateFromQuaternion(rotation) * Matrix.CreateTranslation(position - v);
            if (body != null && (e == null || (e != null && e is BodyGeoEventArgs && (e as BodyGeoEventArgs).SetBodyGeo)))
                body.Position = Helper.ToJitterVector(position);
        }
    }
}
