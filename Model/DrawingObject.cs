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

        public Model DrawingModel
        {
            get { return drawingModel; }
            set { drawingModel = value; }
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
                    effect.World = world;
                    effect.View = view;
                    effect.Projection = projection;
                    effect.EnableDefaultLighting();
                    effect.PreferPerPixelLighting = true;
                    effect.SpecularPower = 16;
                }
                mesh.Draw();
            }
        }
    }
}
