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

        public Model DrawingModel
        {
            get { return drawingModel; }
            set
            {
                drawingModel = value;
                if (drawingModel != null)
                    MeasureModel();
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
    }
}
