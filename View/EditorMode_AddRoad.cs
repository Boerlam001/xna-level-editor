using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Microsoft.Xna.Framework;
//using Microsoft.Xna.Framework.Graphics;
using EditorModel;

namespace View
{
    public class EditorMode_AddRoad : EditorMode_GridPointing
    {
        int step;
        Vector2 start;

        public EditorMode_AddRoad(Editor editor)
            : base(editor)
        {
            step = 0;
        }

        public override void PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            base.PreviewKeyDown(sender, e);
            if (e.KeyCode == Keys.Escape)
            {
                step = 0;
                editor.GridPointers.Clear();
            }
        }

        public override void MouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            base.MouseDown(sender, e);
            if (e.Button == MouseButtons.Left)
            {
                step = (step + 1) % 3;
                if (step == 1)
                    start = editor.GridPointer.GridPosition;
                if (step == 2)
                {
                    foreach (GridPointer gp in editor.GridPointers)
                    {
                        editor.Grid.AddRoad(gp.GridPosition);
                        editor.Grid.Detach(gp);
                    }
                    editor.GridPointers.Clear();
                    editor.Grid.AddRoad(editor.GridPointer.GridPosition);
                    step = 0;
                    editor.Camera.Notify();
                }
            }
        }

        public override void MouseMove(object sender, MouseEventArgs e)
        {
            base.MouseMove(sender, e);
            if (step == 1)
            {
                Trace();
                editor.Camera.Notify();
            }
        }

        public void Trace()
        {
            Vector2 temp = start;
            int i = 0;
            while (temp != editor.GridPointer.GridPosition)
            {
                if (i < editor.GridPointers.Count)
                    editor.GridPointers[i].GridPosition = temp;
                else
                {
                    GridPointer gridPointerTemp = new GridPointer(editor.Grid);
                    gridPointerTemp.GridPosition = temp;
                    editor.GridPointers.Add(gridPointerTemp);
                }
                Vector2 temp2 = editor.GridPointer.GridPosition - temp;
                if (Math.Abs(temp2.X) > Math.Abs(temp2.Y))
                {
                    if (editor.GridPointer.GridPosition.X > temp.X)
                        temp.X += 1;
                    else
                        temp.X -= 1;
                }
                else
                {
                    if (editor.GridPointer.GridPosition.Y > temp.Y)
                        temp.Y += 1;
                    else
                        temp.Y -= 1;
                }
                i++;
            }

            if (i < editor.GridPointers.Count)
            {
                editor.GridPointers.RemoveRange(i, editor.GridPointers.Count - i);
            }
        }
    }
}
