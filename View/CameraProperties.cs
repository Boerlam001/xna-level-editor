using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace View
{
    public partial class CameraProperties : Form
    {
        public ObjectProperties ObjectProperties
        {
            get
            {
                return objectProperties1;
            }
        }

        public CameraProperties()
        {
            InitializeComponent();
        }

        private void CameraProperties_FormClosing(object sender, FormClosingEventArgs e)
        {
            objectProperties1.Model.Detach(objectProperties1);
        }
    }
}
