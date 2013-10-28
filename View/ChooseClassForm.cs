using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using XleGenerator;

namespace View
{
    public partial class ChooseClassForm : Form
    {
        private List<string> classes;
        public string ProjectName
        {
            get
            {
                return projectComboBox.Text;
            }
        }

        public string ClassName
        {
            get
            {
                return classComboBox.Text;
            }
        }

        public bool IsOpen
        {
            get
            {
                return classes.Contains(classComboBox.Text);
            }
        }

        public ChooseClassForm()
        {
            InitializeComponent();
        }

        private void ChooseClassForm_Load(object sender, EventArgs e)
        {
            List<string> projects = ClassManager.ListProjects();
            classes = new List<string>();
            projectComboBox.Items.Clear();
            foreach (string project in projects)
                projectComboBox.Items.Add(project);
        }

        private void projectComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            classes = ClassManager.ListClasses(projectComboBox.Text);
            classComboBox.Items.Clear();
            foreach (string className in classes)
                classComboBox.Items.Add(className);
        }
    }
}
