using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using EditorModel;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

namespace View
{
    public class EditorMode_Select : EditorMode
    {
        //flag for dragging object
        int isDrag;
        private bool snapToTerrain = false;
        private Vector3 originalPosition;

        public EditorMode_Select(Editor editor) : base(editor)
        {
            isDrag = -1;
        }

        public EditorMode_Select() : base()
        {
            isDrag = -1;
        }

        public override void PreviewKeyDown(object sender, System.Windows.Forms.PreviewKeyDownEventArgs e)
        {
            base.PreviewKeyDown(sender, e);
        }

        public override void KeyUp(object sender, System.Windows.Forms.KeyEventArgs e)
        {
            base.KeyUp(sender, e);
            
            if (editor.Selected != null && e.KeyCode == Keys.Delete)
            {
                BaseObject obj = editor.Selected;
                editor.DeselectObject();
                editor.DeleteObject(obj);
            }

            if (e.KeyCode == Keys.ShiftKey && editor.Selected != null)
            {
                if (!snapToTerrain)
                {
                    //Vector3 rotationTarget = new Vector3(30, 45, 0);
                    //Matrix rotationMatrixTarget = Matrix.CreateFromYawPitchRoll(MathHelper.ToRadians(rotationTarget.Y), MathHelper.ToRadians(rotationTarget.X), MathHelper.ToRadians(rotationTarget.Z));
                    //Vector3 directionTarget = Vector3.Transform(Vector3.UnitZ, rotationMatrixTarget);
                    //Vector3 target = editor.Selected.Position - directionTarget * 10;
                    //editor.EaseCamera(target, rotationTarget);
                    snapToTerrain = true;
                    originalPosition = editor.Selected.Position;
                }
                else
                {
                    snapToTerrain = false;
                    editor.Selected.Position = originalPosition;
                    editor.Selected.Notify();
                }
            }
        }

        public override void MouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            base.MouseDown(sender, e);

            if (e.Button == System.Windows.Forms.MouseButtons.Left)
            {
                if (editor.MapModel.Objects.Count == 0)
                    return;
                Ray ray = Helper.Pick(editor.GraphicsDevice.Viewport, editor.Camera, e.X, e.Y);
                
                if (editor.Selected != null)
                {
                    if (snapToTerrain)
                    {
                        snapToTerrain = false;
                        return;
                    }

                    isDrag = editor.SelectedBoundingBox.AxisLines.OnMouseDown(e.X, e.Y);

                    if (isDrag != -1)
                        return;

                    editor.DeselectObject();
                }

                float min = float.MaxValue;
                BaseObject temp = null;
                foreach (BaseObject obj in editor.MapModel.Objects)
                {
                    if (obj.RayIntersects(ray, e.X, e.Y))
                    {
                        float dist = Vector3.Distance(ray.Position, obj.Position);
                        if (dist < min)
                        {
                            temp = obj;
                            min = dist;
                        }
                    }
                }

                if (temp != null)
                {
                    editor.SelectObject(temp);
                }
                ((IObserver) editor).UpdateObserver();
            }
        }

        public override void MouseMove(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            base.MouseMove(sender, e);

            if (editor.Selected != null)
            {
                if (isDrag != -1)
                {
                    editor.SelectedBoundingBox.AxisLines.OnMouseMove(e.X, e.Y);
                    editor.Selected.Notify();
                    editor.Camera.Notify();
                }

                if (snapToTerrain)
                {
                    int[,] indices = editor.Terrain.TerrainIndexer.Indices;
                    if (e.X >= 0 && e.X < indices.GetLength(0) && e.Y >= 0 && e.Y < indices.GetLength(1))
                    {
                        int index = indices[e.X, e.Y];
                        if (index != -1)
                        {
                            Vector3 pos = editor.Terrain.Vertices[index].Position;
                            if (editor.Selected is DrawingObject)
                            {
                                DrawingObject obj = editor.Selected as DrawingObject;
                                BoundingBox bbox = editor.SelectedBoundingBox.BoundingBoxBuffer.BoundingBox;
                                Vector3 center = Vector3.Transform(obj.Scale * obj.Center, obj.Rotation);
                                pos.Y += center.Y - (bbox.Min * obj.Scale).Y;
                            }
                            editor.Selected.Position = pos;
                            editor.Selected.Notify();
                        }
                    }
                }
            }
        }

        public override void MouseUp(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            base.MouseUp(sender, e);
            if (isDrag != -1)
            {
                editor.SelectedBoundingBox.AxisLines.OnMouseUp(e.X, e.Y);
                isDrag = -1;
            }
        }

        public override void DragEnter(object sender, System.Windows.Forms.DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                e.Effect = DragDropEffects.Copy;
            }
        }

        public override void DragDrop(object sender, System.Windows.Forms.DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                Array files = (Array)e.Data.GetData(DataFormats.FileDrop);
                foreach (string file in files)
                {
                    string name = System.IO.Path.GetFileName(file).Replace(".", "_");
                    Vector3 put = Helper.Put(editor.GraphicsDevice.Viewport, editor.Camera, e.X, e.Y, 10);
                    BaseObject obj = editor.AddObject(file, name, put, Vector3.Zero, Vector3.One);
                    if (obj != null)
                    {
                        editor.SelectObject(obj);
                        //if (editor.SelectedBoundingBox.Model == obj && obj is DrawingObject)
                        //{
                        //    BoundingBox bbox = editor.SelectedBoundingBox.BoundingBoxBuffer.BoundingBox;
                        //    float length = (bbox.Max - bbox.Min).Length();
                        //    put = Helper.Put(editor.GraphicsDevice.Viewport, editor.Camera, e.X, e.Y, length);
                        //    editor.Selected.Position = put;
                        //    editor.Selected.Notify();
                        //}
                    }
                }
            }
        }
    }
}
