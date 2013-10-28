using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace EditorModel
{
    public class DrawingObject : BaseObject
    {
        #region attributes
        private Model drawingModel;
        private Matrix[] boneTransforms;
        private string sourceFile;
        private Vector3 center;
        #endregion

        #region getters and setters
        public Matrix[] BoneTransforms
        {
            get { return boneTransforms; }
            set { boneTransforms = value; }
        }

        //[Category("File")]
        [Description("Source file for model")]
        [EditorAttribute(typeof(System.Windows.Forms.Design.FileNameEditor), typeof(System.Drawing.Design.UITypeEditor))]
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
                }
            }
        }

        [Browsable(false)]
        public Vector3 Center
        {
            get { return center; }
            set { center = value; }
        }
        #endregion

        public DrawingObject() : base()
        {
            
        }

        public override void Draw(SpriteBatch spriteBatch, bool lightDirectionEnabled = false, Vector3 lightDirection = new Vector3())
        {
            if (drawingModel == null)
                return;
            foreach (ModelMesh mesh in drawingModel.Meshes)
            {
                foreach (BasicEffect effect in mesh.Effects)
                {
                    effect.World = boneTransforms[mesh.ParentBone.Index] * world;
                    effect.View = Camera.World;
                    effect.Projection = Camera.Projection;
                    effect.EnableDefaultLighting();
                    effect.PreferPerPixelLighting = true;
                    effect.SpecularPower = 16;
                    if (lightDirectionEnabled)
                    {
                        effect.DirectionalLight0.Enabled = lightDirectionEnabled;
                        effect.DirectionalLight0.Direction = lightDirection;
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

            center = new Vector3();
            foreach (ModelMesh mesh in drawingModel.Meshes)
            {
                BoundingSphere meshBounds = mesh.BoundingSphere;
                Matrix transform = boneTransforms[mesh.ParentBone.Index];
                Vector3 meshCenter = Vector3.Transform(meshBounds.Center, transform);

                center += meshCenter;
            }

            center /= drawingModel.Meshes.Count;
            OnPositionChanged(this, null);

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

        public override bool RayIntersects(Ray ray, float mouseX, float mouseY)
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
            world = Matrix.CreateScale(scale) * Matrix.CreateFromYawPitchRoll(MathHelper.ToRadians(rotationY), MathHelper.ToRadians(rotationX), MathHelper.ToRadians(rotationZ)) * Matrix.CreateTranslation(position - v);
        }

        protected override void OnPositionChanged(object sender, EventArgs e)
        {
            base.OnPositionChanged(sender, e);
            Vector3 v = Vector3.Transform(center, rotation);
            world = Matrix.CreateScale(scale) * Matrix.CreateFromYawPitchRoll(MathHelper.ToRadians(rotationY), MathHelper.ToRadians(rotationX), MathHelper.ToRadians(rotationZ)) * Matrix.CreateTranslation(position - v);
        }
    }
}
