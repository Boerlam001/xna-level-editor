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
using SkinnedModel;

namespace XleModel
{
    public class BodyGeoEventArgs : EventArgs
    {
        public bool SetBodyGeo { get; set; }
        public BodyGeoEventArgs(bool setBodyGeo = true)
        {
            SetBodyGeo = setBodyGeo;
        }
    }

    public class DrawingObject : BaseObject
    {
        #region attributes
        private Model drawingModel;
        private Matrix[] boneTransforms;
        private Matrix[] skinTransforms;
        private Matrix[] worldTransforms;
        private string sourceFile;
        private Vector3 center;
        private string assetName;
        private Camera camera;
        private bool lightDirectionEnabled;
        private Vector3 lightDirection;
        private float modelRadius;
        private SkinningData skinningData;
        private AnimationClip clip;
        private TimeSpan currentTimeValue;
        private int currentKeyframe;
        private string[] clipNames;
        private int currentClip;
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

        public string AssetName
        {
            get { return assetName; }
            set { assetName = value; }
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

                    if (drawingModel.Tag is SkinningData)
                    {
                        skinningData = drawingModel.Tag as SkinningData;
                        clipNames = skinningData.AnimationClips.Keys.ToArray();
                        currentClip = 0;
                        clip = skinningData.AnimationClips[clipNames[currentClip]];
                        currentKeyframe = 0;
                        currentTimeValue = TimeSpan.Zero;

                        boneTransforms = new Matrix[skinningData.BindPose.Count];
                        skinTransforms = new Matrix[skinningData.BindPose.Count];
                        worldTransforms = new Matrix[skinningData.BindPose.Count];
                    }
                }
            }
        }

        public Camera Camera
        {
            get { return camera; }
            set { camera = value; }
        }

        public string[] ClipNames
        {
            get { return clipNames; }
            set { clipNames = value; }
        }

        public int CurrentClip
        {
            get { return currentClip; }
            set { currentClip = value; }
        }
        #endregion

        public DrawingObject(Game game, Camera camera, string assetName, World physicsWorld)
            : base(game, physicsWorld)
        {
            this.camera = camera;
            this.assetName = assetName;
            lightDirectionEnabled = false;
            lightDirection = new Vector3();
        }

        protected override void LoadContent()
        {
            if (!string.IsNullOrEmpty(assetName))
                DrawingModel = Game.Content.Load<Model>(assetName);
            base.LoadContent();
        }

        public override void Update(GameTime gameTime)
        {
            if (drawingModel.Tag is SkinningData)
            {
                UpdateBoneTransforms(gameTime.ElapsedGameTime, true);
                UpdateWorldTransforms(world);
                UpdateSkinTransforms();
            }
            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            if (drawingModel != null)
            {
                foreach (ModelMesh mesh in drawingModel.Meshes)
                {
                    if (drawingModel.Tag is SkinningData)
                    {
                        foreach (SkinnedEffect effect in mesh.Effects)
                        {
                            effect.SetBoneTransforms(skinTransforms);
                            effect.View = camera.World;
                            effect.Projection = camera.Projection;
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
                            if (GraphicsDevice.BlendState == BlendState.AlphaBlend)
                            {
                                effect.Alpha = 1;
                            }
                            //else
                            //{
                            //    effect.Alpha = 0;
                            //}
                        }
                    }
                    else
                    {
                        foreach (BasicEffect effect in mesh.Effects)
                        {

                            effect.World = boneTransforms[mesh.ParentBone.Index] * world;
                            effect.View = camera.World;
                            effect.Projection = camera.Projection;
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
                            if (GraphicsDevice.BlendState == BlendState.AlphaBlend)
                            {
                                effect.Alpha = 1;
                            }
                            //else
                            //{
                            //    effect.Alpha = 0;
                            //}
                        }
                    }
                    mesh.Draw();
                }
            }
            base.Draw(gameTime);
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
            
            OnPositionChanged(this, new BodyGeoEventArgs(false));

            // Now we know the center point, we can compute the model radius
            // by examining the radius of each mesh bounding sphere.
            modelRadius = 0;
            
            foreach (ModelMesh mesh in drawingModel.Meshes)
            {
                BoundingSphere meshBounds = mesh.BoundingSphere;
                Matrix transform = boneTransforms[mesh.ParentBone.Index];
                Vector3 meshCenter = Vector3.Transform(meshBounds.Center, transform);
            
                float transformScale = transform.Forward.Length();
            
                float meshRadius = (meshCenter - center).Length() +
                                   (meshBounds.Radius * transformScale);
            
                modelRadius = Math.Max(modelRadius, meshRadius);
            }
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

        /// <summary>
        /// Helper used by the Update method to refresh the BoneTransforms data.
        /// </summary>
        public void UpdateBoneTransforms(TimeSpan time, bool relativeToCurrentTime)
        {
            if (clip == null)
                throw new InvalidOperationException(
                            "AnimationPlayer.Update was called before StartClip");

            // Update the animation position.
            if (relativeToCurrentTime)
            {
                time += currentTimeValue;

                // If we reached the end, loop back to the start.
                while (time >= clip.Duration)
                    time -= clip.Duration;
            }

            if ((time < TimeSpan.Zero) || (time >= clip.Duration))
                throw new ArgumentOutOfRangeException("time");

            // If the position moved backwards, reset the keyframe index.
            if (time < currentTimeValue)
            {
                currentKeyframe = 0;
                skinningData.BindPose.CopyTo(boneTransforms, 0);
            }

            currentTimeValue = time;

            // Read keyframe matrices.
            IList<Keyframe> keyframes = clip.Keyframes;

            while (currentKeyframe < keyframes.Count)
            {
                Keyframe keyframe = keyframes[currentKeyframe];

                // Stop when we've read up to the current time position.
                if (keyframe.Time > currentTimeValue)
                    break;

                // Use this keyframe.
                boneTransforms[keyframe.Bone] = keyframe.Transform;

                currentKeyframe++;
            }
        }

        /// <summary>
        /// Helper used by the Update method to refresh the SkinTransforms data.
        /// </summary>
        public void UpdateSkinTransforms()
        {
            for (int bone = 0; bone < skinTransforms.Length; bone++)
            {
                skinTransforms[bone] = skinningData.InverseBindPose[bone] *
                                            worldTransforms[bone];
            }
        }

        /// <summary>
        /// Helper used by the Update method to refresh the WorldTransforms data.
        /// </summary>
        public void UpdateWorldTransforms(Matrix rootTransform)
        {
            // Root bone.
            worldTransforms[0] = boneTransforms[0] * rootTransform;

            // Child bones.
            for (int bone = 1; bone < worldTransforms.Length; bone++)
            {
                int parentBone = skinningData.SkeletonHierarchy[bone];

                worldTransforms[bone] = boneTransforms[bone] *
                                             worldTransforms[parentBone];
            }
        }

        public void ChangeClip(string key)
        {
            if (drawingModel.Tag is SkinningData)
            {
                try
                {
                    if (clip != skinningData.AnimationClips[key])
                    {
                        int temp = -1, i = 0;
                        foreach (string name in clipNames)
                        {
                            if (name == key)
                            {
                                temp = i;
                                break;
                            }
                            i++;
                        }
                        if (temp == -1)
                            return;
                        currentClip = temp;
                        clip = skinningData.AnimationClips[key];
                        currentKeyframe = 0;
                        currentTimeValue = TimeSpan.Zero;
                    }
                }
                catch { }
            }
        }

        public void ChangeClip(int key)
        {
            if (drawingModel.Tag is SkinningData)
            {
                try
                {
                    if (key >= clipNames.Length)
                        return;
                    currentClip = key;
                    clip = skinningData.AnimationClips[clipNames[key]];
                    currentKeyframe = 0;
                    currentTimeValue = TimeSpan.Zero;
                }
                catch { }
            }
        }

        public void NextClip()
        {
            if (drawingModel.Tag is SkinningData)
            {
                try
                {
                    currentClip = (currentClip + 1) % clipNames.Length;
                    clip = skinningData.AnimationClips[clipNames[currentClip]];
                    currentKeyframe = 0;
                    currentTimeValue = TimeSpan.Zero;
                }
                catch { }
            }
        }

        protected override void OnRotationChanged(object sender, EventArgs e)
        {
            base.OnRotationChanged(sender, e);
            Vector3 v = Vector3.Transform(center, rotation);
            world = Matrix.CreateScale(scale) * Matrix.CreateFromQuaternion(rotation) * Matrix.CreateTranslation(position - v);
        }

        protected override void OnPositionChanged(object sender, EventArgs e)
        {
            base.OnPositionChanged(sender, e);
            Vector3 v = Vector3.Transform(scale * center, rotation);
            world = Matrix.CreateScale(scale) * Matrix.CreateFromQuaternion(rotation) * Matrix.CreateTranslation(position - v);
        }
    }
}
