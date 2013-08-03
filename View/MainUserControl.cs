using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using EditorModel;
using EnvDTE;
using EnvDTE80;
using Generator;

namespace View
{
    public partial class MainUserControl : UserControl
    {
        MapModel mapModel;

        DTE2 applicationObject;

        public DTE2 ApplicationObject
        {
            get { return applicationObject; }
            set
            {
                applicationObject = value;
                if (applicationObject != null)
                {
                    ChooseClassForm form = new ChooseClassForm();
                    while (form.ShowDialog() != DialogResult.OK) ;
                    ClassManager.applicationObject = applicationObject;
                    classManager = new Generator.ClassManager(mapModel, form.ProjectName, form.ClassName, form.IsOpen);
                    if (form.IsOpen)
                    {
                        Dictionary<string, DrawingObject> objects = classManager.ReadCodeLines();
                        foreach (string key in objects.Keys)
                        {
                            string file = objects[key].SourceFile;
                            string name = System.IO.Path.GetFileNameWithoutExtension(file);
                            editor1.AddObject(file, name, objects[key].Position, objects[key].EulerRotation);
                        }
                    }
                }
                else
                {
                    classManager = new ClassManager(mapModel);
                }
            }
        }

        public Editor Editor1
        {
            get
            {
                return editor1;
            }
        }

        private ClassManager classManager;

        public ClassManager _ClassManager
        {
            get { return classManager; }
            set { classManager = value; }
        }

        ProjectItem projectItem;

        public ProjectItem ProjectItem
        {
            get { return projectItem; }
            set { projectItem = value; }
        }

        public MainUserControl()
        {
            InitializeComponent();
        }

        private void MainUserControl_Load(object sender, EventArgs e)
        {
            try
            {
                mapModel = new MapModel();
                editor1.MapModel = mapModel;
                TrueModel.Instance.MapModels.Add(mapModel);

                editor1.MainUserControl = this;
                editor1.Camera.Attach(objectProperties2);
                objectProperties2.Model = editor1.Camera;
                editor1.Camera.Notify();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "\r\n" + ex.StackTrace);
            }
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

        private void createFileStripMenuItem_Click(object sender, EventArgs e)
        {
            classManager.GenerateClass();
        }

        private void openFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
        }
    }
}
