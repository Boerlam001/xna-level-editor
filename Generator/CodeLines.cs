using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EditorModel;

namespace Generator
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

        public void Update()
        {
            StringBuilder sb = new StringBuilder();
            code[CodePosition.Variable] = sb.Append("Model ").Append(name).Append(";").ToString();
            sb.Clear();
            code[CodePosition.LoadContent] = sb.Append(name).Append(" = Content.Load<Model>(\"").Append(System.IO.Path.GetFileNameWithoutExtension(model.SourceFile)).Append("\");").ToString();
            sb.Clear();
            code[CodePosition.Draw] = "";
        }
    }
}
