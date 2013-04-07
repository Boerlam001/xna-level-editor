using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using EditorModel;

namespace View
{
    public partial class MainUserControl : UserControl
    {
        TrueModel trueModel;

        public MainUserControl()
        {
            InitializeComponent();
        }

        private void MainUserControl_Load(object sender, EventArgs e)
        {
            trueModel = new TrueModel();
            for (int i = 0; i < 1; i++)
            {
                DrawingObject obj = new DrawingObject();
                obj.Attach(editor1);
                obj.Attach(objectProperties1);
                objectProperties1.Model = obj;
                trueModel.Objects.Add(obj);
                obj.Notify();
            }
            editor1.TrueModel = trueModel;

            editor1.MainUserControl = this;
            editor1.Camera.Attach(objectProperties2);
            objectProperties2.Model = editor1.Camera;
            editor1.Camera.Notify();
        }

        public ObjectProperties ObjectProperties1
        {
            get
            {
                return objectProperties1;
            }
        }

        public ObjectProperties ObjectProperties2
        {
            get
            {
                return objectProperties2;
            }
        }
    }
}
