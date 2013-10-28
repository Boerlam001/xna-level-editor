using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.ComponentModel.Design;

namespace EditorModel.PropertyModel
{
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public class Script
    {
        string name;
        string path;

        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        [EditorAttribute(typeof(System.Windows.Forms.Design.FileNameEditor), typeof(System.Drawing.Design.UITypeEditor))]
        public string Path
        {
            get { return path; }
            set { path = value; }
        }
    }

    
}
