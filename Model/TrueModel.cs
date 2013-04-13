using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EditorModel
{
    public class TrueModel : Subject
    {
        List<DrawingObject> objects;

        public List<DrawingObject> Objects
        {
            get { return objects; }
            set { objects = value; }
        }

        public TrueModel()
        {
            objects = new List<DrawingObject>();
        }
    }
}
