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
using XleGenerator;
using System.IO;

namespace View
{
    public partial class MainUserControl : UserControl, IObserver
    {
        MapModel mapModel;

        DTE2 applicationObject;

        public DTE2 ApplicationObject
        {
            get { return applicationObject; }
            set
            {
                applicationObject = value;
            }
        }

        private Window window;
        
        public Window Window
        {
            get { return window; }
            set
            {
                window = value;
            }
        }

        public ObjectProperties ObjectProperties1
        {
            get
            {
                return objectProperties1;
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
            set
            {
                classManager = value;
                classManager.MapModel = mapModel;
                if (applicationObject != null && window != null)
                {
                    //ChooseClassForm form = new ChooseClassForm();
                    //while (form.ShowDialog() != DialogResult.OK) ;
                    //ClassManager.applicationObject = applicationObject;
                    //classManager = new Generator.ClassManager(form.ProjectName, form.ClassName, form.IsOpen);

                    string heightMapFile = Path.GetDirectoryName(classManager.ContentProject.FullName) + "\\heightmap_" + classManager.Name + ".png";

                    if (isOpen)
                    {
                        Dictionary<string, DrawingObject> objects = classManager.ReadCodeLines();
                        foreach (string key in objects.Keys)
                        {
                            string file = objects[key].SourceFile;
                            string name = System.IO.Path.GetFileNameWithoutExtension(file);
                            editor1.AddObject(file, name, objects[key].Position, objects[key].EulerRotation);
                        }
                        if (File.Exists(heightMapFile))
                            editor1.ImportHeightmap(heightMapFile);
                        fileSystemWatcher.Path = Path.GetDirectoryName(classManager.CurrentProject.FullName) + "\\Scripts";
                    }
                    
                    classManager.AddHeightMapToContentProject(editor1.Terrain, isOpen, heightMapFile);
                    window.Caption = classManager.Name + ".cs";
                }
            }
        }

        private bool isOpen;

        public bool IsOpen
        {
            get { return isOpen; }
            set { isOpen = value; }
        }

        ProjectItem projectItem;
        private static string assemblyDirectory;
        private FileSystemWatcher fileSystemWatcher;

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
                mapModel.Attach(this);
                editor1.MapModel = mapModel;
                TrueModel.Instance.MapModels.Add(mapModel);

                editor1.MainUserControl = this;
                editor1.Camera.Notify();
                
                classManager = new ClassManager();
                assetTreeView.Nodes.Add("Models");
                assetTreeView.Nodes.Add("Scripts");

                string scriptDir = AssemblyDirectory + "\\Scripts";
                if (!Directory.Exists(scriptDir))
                    Directory.CreateDirectory(scriptDir);
                fileSystemWatcher = new FileSystemWatcher(scriptDir);
                fileSystemWatcher.IncludeSubdirectories = true;
                fileSystemWatcher.Changed += new FileSystemEventHandler(fileSystemWatcher_Changed);
                fileSystemWatcher.Created += new FileSystemEventHandler(fileSystemWatcher_Created);
                fileSystemWatcher.Deleted += new FileSystemEventHandler(fileSystemWatcher_Deleted);
                fileSystemWatcher.Renamed += new RenamedEventHandler(fileSystemWatcher_Renamed);
                fileSystemWatcher.EnableRaisingEvents = true;
                UpdateScriptAssets();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "\r\n" + ex.StackTrace);
            }
        }

        void fileSystemWatcher_Renamed(object sender, RenamedEventArgs e)
        {
            UpdateScriptAssets();
        }

        void fileSystemWatcher_Deleted(object sender, FileSystemEventArgs e)
        {
            UpdateScriptAssets();
        }

        void fileSystemWatcher_Created(object sender, FileSystemEventArgs e)
        {
            UpdateScriptAssets();
        }

        void fileSystemWatcher_Changed(object sender, FileSystemEventArgs e)
        {
            UpdateScriptAssets();
        }

        private delegate void AddScriptNodeDelegate(TreeNodeCollection nodes, string value);

        void AddScriptNode(TreeNodeCollection nodes, string value)
        {
            if (!assetTreeView.InvokeRequired)
                nodes.Add(value);
            else
                assetTreeView.Invoke(new AddScriptNodeDelegate(AddScriptNode), new object[2] { nodes, value });
        }

        private delegate void ClearScriptsNodeDelegate(TreeNodeCollection nodes);

        void ClearScripsNode(TreeNodeCollection nodes)
        {
            if (!assetTreeView.InvokeRequired)
                nodes.Clear();
            else
                assetTreeView.Invoke(new ClearScriptsNodeDelegate(ClearScripsNode), new object[1] { nodes });
        }

        private void UpdateScriptAssets()
        {
            if (Directory.Exists(fileSystemWatcher.Path))
            {
                try
                {
                    ClearScripsNode(assetTreeView.Nodes[1].Nodes);
                    foreach (string file in Directory.EnumerateFiles(fileSystemWatcher.Path))
                    {
                        AddScriptNode(assetTreeView.Nodes[1].Nodes, file);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message + "\r\n" + ex.StackTrace);
                }
            }
        }

        private void CheckFileStatus()
        {
            if (!classManager.ClassFile.Saved)
            {
                window.Caption = classManager.Name + "*";
            }
            else
            {
                window.Caption = classManager.Name;
            }
        }

        private void createFileStripMenuItem_Click(object sender, EventArgs e)
        {
            classManager.GenerateClass();
        }

        private void openFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
        }

        private void saveHeightmapToolStripMenuItem_Click(object sender, EventArgs e)
        {
            classManager.AddHeightMapToContentProject(editor1.Terrain);
        }

        public void UpdateObserver()
        {
            objectTreeView.Nodes.Clear();
            foreach (DrawingObject obj in mapModel.Objects)
            {
                objectTreeView.Nodes.Add(obj.Name);
                bool assetListed = false;
                foreach (TreeNode node in assetTreeView.Nodes[0].Nodes)
                {
                    if (node.Text == obj.SourceFile)
                    {
                        assetListed = true;
                    }
                }
                if (!assetListed)
                {
                    assetTreeView.Nodes[0].Nodes.Add(obj.SourceFile);
                }
            }
        }

        public static string AssemblyDirectory
        {
            get
            {
                if (assemblyDirectory == null)
                {
                    string codeBase = System.Reflection.Assembly.GetExecutingAssembly().CodeBase;
                    UriBuilder uri = new UriBuilder(codeBase);
                    string path = Uri.UnescapeDataString(uri.Path);
                    assemblyDirectory = System.IO.Path.GetDirectoryName(path);
                }
                return assemblyDirectory;
            }
        }

        private void objectTreeView_AfterSelect(object sender, TreeViewEventArgs e)
        {
            DrawingObject temp = mapModel.getObjectByName(e.Node.Text);
            if (temp != null)
            {
                editor1.DeselectObject();
                editor1.SelectObject(temp);
            }
        }
    }
}
