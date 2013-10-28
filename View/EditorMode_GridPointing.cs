using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace View
{
    public class EditorMode_GridPointing : EditorMode_Terrain
    {
        public EditorMode_GridPointing(Editor editor)
            : base(editor)
        {
        }

        public override void MouseMove(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            base.MouseMove(sender, e);
            if (editor.Camera.IsMoving)
                return;
            Vector2 pos = new Vector2(e.X, e.Y);
            if (OutOfBounds(pos))
            {
                return;
            }
            editor.GridPointer.OnMouseMove(editor.Terrain.TerrainIndexer.Indices[e.X, e.Y]);
            editor.Camera.Notify();
        }
    }
}
