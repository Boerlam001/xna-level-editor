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
        }

        public override void KeyUp(object sender, System.Windows.Forms.KeyEventArgs e)
        {
            if (editor.Selected != null && e.KeyCode == Keys.Delete)
            {
                BaseObject obj = editor.Selected;
                editor.DeselectObject();
                editor.DeleteObject(obj);
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
            //editor.Text1 += e.X + " " + e.Y + "\r\n";
            //base.MouseMove(sender, e);
            diffX = (float)(e.X - mouseX);
            diffY = (float)(e.Y - mouseY);

            if (editor.Selected != null)
            {
                if (isDrag != -1)
                {
                    editor.SelectedBoundingBox.AxisLines.OnMouseMove(e.X, e.Y);
                    editor.Selected.Notify();
                    editor.Camera.Notify();
                }
            }
            mouseX = e.X;
            mouseY = e.Y;

            if (isRotate)
            {
                editor.Camera.Rotate(diffY / 10, -diffX / 10, 0);
                editor.Camera.Notify();
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
                    editor.AddObject(file, name, Helper.Put(editor.GraphicsDevice.Viewport, editor.Camera, e.X, e.Y, 3), Vector3.Zero);
                }
            }
        }
    }
}
