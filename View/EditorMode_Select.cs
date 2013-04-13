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
        public EditorMode_Select(Editor editor) : base(editor)
        {
        }

        public EditorMode_Select()
        {
        }

        public override void PreviewKeyDown(object sender, System.Windows.Forms.PreviewKeyDownEventArgs e)
        {
            throw new NotImplementedException();
        }

        public override void KeyUp(object sender, System.Windows.Forms.KeyEventArgs e)
        {
            throw new NotImplementedException();
        }

        public override void MouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            base.MouseDown(sender, e);

            if (e.Button == System.Windows.Forms.MouseButtons.Left)
            {
                if (editor.TrueModel.Objects.Count == 0)
                    return;
                Ray ray = editor.Pick(e.X, e.Y);

                if (editor.Selected != null)
                {
                    editor.Selected.Detach(editor.MainUserControl.ObjectProperties1);
                    editor.MainUserControl.ObjectProperties1.Model = null;
                    editor.Selected = null;
                }

                float min = float.MaxValue;
                foreach (DrawingObject obj in editor.TrueModel.Objects)
                {
                    if (obj.RayIntersects(ray))
                    {
                        float dist = Vector3.Distance(ray.Position, obj.Position);
                        if (dist < min)
                        {
                            editor.Selected = obj;
                            min = dist;
                        }
                    }
                }

                if (editor.Selected != null)
                {
                    editor.Selected.Attach(editor.SelectedBoundingBox);
                    editor.SelectedBoundingBox.Model = editor.Selected;
                    editor.Selected.Attach(editor.MainUserControl.ObjectProperties1);
                    editor.MainUserControl.ObjectProperties1.Model = editor.Selected;
                    editor.Selected.Notify();
                }
                ((IObserver) editor).Update();
            }
        }

        public override void MouseMove(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            base.MouseMove(sender, e);
        }

        public override void MouseUp(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            base.MouseUp(sender, e);
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
                    DrawingObject obj = new DrawingObject();
                    obj.Position = editor.Put(mouseX, mouseY, 3);
                    string name = file.Substring(file.LastIndexOf('\\') + 1);
                    obj.DrawingModel = editor.OpenModel(file, name);
                    obj.Name = name;
                    obj.Attach(editor);
                    editor.TrueModel.Objects.Add(obj);
                    obj.Notify();
                }
            }
        }
    }
}
