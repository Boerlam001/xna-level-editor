using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EditorModel;

namespace XleGenerator
{
    public class CodeLines : IObserver
    {
        DrawingObject model;

        public DrawingObject Model
        {
            get { return model; }
            set
            {
                if (model != null)
                    model.Detach(this);
                model = value;
                model.Attach(this);
                name = model.Name.Replace(".", "_");
                name = Char.ToLowerInvariant(name[0]) + name.Substring(1);
            }
        }

        public enum CodePosition
        {
            Variable,
            Constructor,
            LoadContent,
            Draw
        }

        Dictionary<CodePosition, string> code;

        public Dictionary<CodePosition, string> Code
        {
            get { return code; }
        }

        string name;

        public CodeLines()
        {
            code = new Dictionary<CodePosition, string>();
        }

        public void UpdateObserver()
        {
            StringBuilder sb = new StringBuilder();
            code[CodePosition.Variable] = sb.Append("DrawingObject ").Append(name).Append(";").ToString();
            sb.Clear();
            float x = model.Position.X, y = model.Position.Y, z = model.Position.Z,
                  rotX = model.RotationX, rotY = model.RotationY, rotZ = model.RotationZ;
            code[CodePosition.Constructor] =
                sb.
                Append(name).Append(" = new DrawingObject(this, camera, \"").Append(System.IO.Path.GetFileNameWithoutExtension(model.SourceFile)).Append("\", world);\r\n").
                //Append(name).Append(".DrawingModel = Content.Load<Model>(\"").Append(System.IO.Path.GetFileNameWithoutExtension(model.SourceFile)).Append("\");\r\n").
                Append(name).Append(".Position = new Vector3(").Append(x).Append("f, ").Append(y).Append("f, ").Append(z).Append("f);\r\n").
                Append(name).Append(".EulerRotation = new Vector3(").Append(rotX).Append("f, ").Append(rotY).Append("f, ").Append(rotZ).Append("f);\r\n").
                Append("Components.Add(").Append(name).Append(");\r\n").
                ToString();
            sb.Clear();
            code[CodePosition.LoadContent] = sb.ToString();
            sb.Clear();
            code[CodePosition.Draw] = sb.ToString();//sb.Append(name).Append(".Draw(camera.World, camera.Projection);").ToString();
        }
    }
}
