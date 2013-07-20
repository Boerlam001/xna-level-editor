using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Generator;

namespace View
{
    public partial class CodeBrowser : Form
    {
        ClassManager cm;

        public ClassManager Cm
        {
            get { return cm; }
            set { cm = value; }
        }

        public CodeBrowser()
        {
            InitializeComponent();
        }

        private void CodeBrowser_Load(object sender, EventArgs e)
        {
            
        }

        private void button1_Click(object sender, EventArgs e)
        {
            cm.LoadClasses();
            textBox1.Text = cm.Output;
        }
    }
}
