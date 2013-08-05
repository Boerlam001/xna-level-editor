using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using EditorModel;

namespace View
{
    public class EditorMode_IncreaseHeight : EditorMode
    {
        public override void PreviewKeyDown(object sender, System.Windows.Forms.PreviewKeyDownEventArgs e)
        {
            //throw new NotImplementedException();
        }

        public override void KeyUp(object sender, System.Windows.Forms.KeyEventArgs e)
        {
            //throw new NotImplementedException();
        }

        public override void DragEnter(object sender, System.Windows.Forms.DragEventArgs e)
        {
            //throw new NotImplementedException();
        }

        public override void DragDrop(object sender, System.Windows.Forms.DragEventArgs e)
        {
            //throw new NotImplementedException();
        }

        public override void MouseMove(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            base.MouseMove(sender, e);
            BoundingFrustum boundingFrustum = Helper.UnprojectRectangle(new Rectangle(e.X - 1, e.Y - 1, 2, 2), editor.GraphicsDevice.Viewport, editor.Camera.Projection, editor.Camera.World);
            foreach (VertexPositionColorNormal vertex in editor.Terrain.Vertices)
            {
                ContainmentType containmentType = boundingFrustum.Contains(vertex.Position);
                if (containmentType == ContainmentType.Contains || containmentType == ContainmentType.Intersects)
                {
                    //containmentType.
                }
            }
        }
    }
}
