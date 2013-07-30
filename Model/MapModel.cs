using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EditorModel
{
    public class MapModel : Subject
    {
        List<DrawingObject> objects;

        public List<DrawingObject> Objects
        {
            get { return objects; }
            set { objects = value; }
        }

        public MapModel()
        {
            objects = new List<DrawingObject>();
        }

        public bool NameExists(string name)
        {
            foreach (DrawingObject obj in objects)
            {
                if (obj.Name == name)
                    return true;
            }
            return false;
        }
    }
}
