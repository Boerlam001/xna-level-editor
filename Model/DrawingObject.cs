using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace EditorModel
{
    public class DrawingObject : BaseObject
    {
        Model drawingModel;

        private Matrix[] boneTransforms;

        public Matrix[] BoneTransforms
        {
            get { return boneTransforms; }
            set { boneTransforms = value; }
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

        public DrawingObject()
        {
            
        }

        public void Draw(Matrix view, Matrix projection)
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
                    effect.EnableDefaultLighting();
                    effect.PreferPerPixelLighting = true;
                    effect.SpecularPower = 16;
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
            
            foreach (ModelMesh mesh in drawingModel.Meshes)
            {
                BoundingSphere meshBounds = mesh.BoundingSphere;
                Matrix transform = boneTransforms[mesh.ParentBone.Index];
                Vector3 meshCenter = Vector3.Transform(meshBounds.Center, transform);

                position += meshCenter;
            }

            position /= drawingModel.Meshes.Count;

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
    }
}
