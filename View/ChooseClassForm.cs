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
    public partial class ChooseClassForm : Form
    {
        public string ProjectName
        {
            get
            {
                return projectTextBox.Text;
            }
        }

        public string ClassName
        {
            get
            {
                return classTextBox.Text;
            }
        }

        public ChooseClassForm()
        {
            InitializeComponent();
        }
    }
}
