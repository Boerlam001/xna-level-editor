﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace View
{
    public class EditorMode_Terrain_IncreaseHeight : EditorMode_Terrain
    {
        public EditorMode_Terrain_IncreaseHeight(Editor editor)
            : base(editor)
        {
        }

        public override void MouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            base.MouseDown(sender, e);
            if (e.Button == System.Windows.Forms.MouseButtons.Left)
            {
                //editor.TerrainBrush.Increase();
                MouseMove(sender, e);
                //editor.Camera.Notify();
            }
        }

        public override void MouseMove(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            base.MouseMove(sender, e);
            if (isMouseDown && !isRotate) //left click
            {
                editor.TerrainBrush.Increase();
                editor.Camera.Notify();
            }
        }
    }
}
