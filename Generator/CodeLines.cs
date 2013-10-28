using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EditorModel;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Jitter;
using Jitter.Collision;
using Jitter.Dynamics;
using Jitter.Collision.Shapes;
using Jitter.LinearMath;

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
                  rotX = model.RotationX, rotY = model.RotationY, rotZ = model.RotationZ,
                  scaleX = model.Scale.X, scaleY = model.Scale.Y, scaleZ = model.Scale.Z;

            sb.
                Append(name).Append(" = new DrawingObject(this, camera, \"").Append(System.IO.Path.GetFileNameWithoutExtension(model.SourceFile)).Append("\", world);\r\n").
                Append(name).Append(".Name = \"").Append(name).Append("\";\r\n").
                Append(name).Append(".Position = new Vector3(").Append(x).Append("f, ").Append(y).Append("f, ").Append(z).Append("f);\r\n").
                Append(name).Append(".EulerRotation = new Vector3(").Append(rotX).Append("f, ").Append(rotY).Append("f, ").Append(rotZ).Append("f);\r\n").
                Append(name).Append(".Scale = new Vector3(").Append(scaleX).Append("f, ").Append(scaleY).Append("f, ").Append(scaleZ).Append("f);\r\n").
                Append(name).Append(".PhysicsEnabled = ").Append((model.PhysicsEnabled) ? "true" : "false").Append(";\r\n").
                Append(name).Append(".PhysicsAdapter.Body.IsActive = ").Append((model.IsActive) ? "true" : "false").Append(";\r\n").
                Append(name).Append(".PhysicsAdapter.Body.IsStatic = ").Append((model.IsStatic) ? "true" : "false").Append(";\r\n");
            if (model.CharacterControllerEnabled)
                sb.Append(name).Append(".PhysicsAdapter.EnableCharacterController();\r\n");
            sb.Append("Components.Add(").Append(name).Append(");\r\n");

            if (model.PhysicsShapeKind == PhysicsShapeKind.BoxShape)
            {
                JVector size = ((BoxShape)model.PhysicsShape).Size;
                Vector3 pos = model.BodyPosition;
                sb.Append(name).Append(".ChangePhysicsAdapter(typeof(BoxAdapter), new object[] { this, ").Append(name).Append(", world, new Vector3(").Append(pos.X).Append("f, ").Append(pos.Y).Append("f, ").Append(pos.Z).Append("f), new Vector3(").Append(size.X).Append("f, ").Append(size.Y).Append("f, ").Append(size.Z).Append("f) });\r\n");
            }
            else if (model.PhysicsShapeKind == PhysicsShapeKind.CapsuleShape)
            {
                float length = ((CapsuleShape)model.PhysicsShape).Length;
                float radius = ((CapsuleShape)model.PhysicsShape).Radius;
                Vector3 pos = model.BodyPosition;
                sb.Append(name).Append(".ChangePhysicsAdapter(typeof(CapsuleAdapter), new object[] { this, ").Append(name).Append(", world, new Vector3(").Append(pos.X).Append("f, ").Append(pos.Y).Append("f, ").Append(pos.Z).Append("f), ").Append(length).Append("f, ").Append(radius).Append("f });\r\n");
            }
            else if (model.PhysicsShapeKind == PhysicsShapeKind.ConvexHullShape)
            {
                Vector3 pos = model.BodyPosition;
                sb.Append(name).Append(".ChangePhysicsAdapter(typeof(ConvexHullAdapter), new object[] { this, ").Append(name).Append(", world, ").Append("new Vector3(").Append(pos.X).Append("f, ").Append(pos.Y).Append("f, ").Append(pos.Z).Append("f) });\r\n");
            }

            foreach (EditorModel.PropertyModel.Script script in model.Scripts)
            {
                sb.Append(name).Append(".AddScript(new ").Append(System.IO.Path.GetFileNameWithoutExtension(script.Name)).Append("());\r\n");
            }

            code[CodePosition.Constructor] = sb.ToString();
            
            sb.Clear();
            code[CodePosition.LoadContent] = sb.ToString();
            sb.Clear();
            code[CodePosition.Draw] = sb.ToString();//sb.Append(name).Append(".Draw(camera.World, camera.Projection);").ToString();
        }
    }
}
