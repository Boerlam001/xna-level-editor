using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace View
{
    public class EditorMode_Terrain_IncreaseHeight : EditorMode_Terrain
    {
        public override void MouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            base.MouseDown(sender, e);
            editor.TerrainBrush.Increase();
            MouseMove(sender, e);
            editor.Camera.Notify();
        }
    }
}
