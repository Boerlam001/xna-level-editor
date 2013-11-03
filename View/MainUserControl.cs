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
using Microsoft.Xna.Framework;

namespace View
{
    public partial class MainUserControl : UserControl, IObserver
    {
        private ClassManager classManager;
        DTE2 applicationObject;
        private Window window;
        private bool isOpen;
        ProjectItem projectItem;
        private static string assemblyDirectory;
        private FileSystemWatcher fileSystemWatcher;
        private TreeNode scriptsNode;
        private TreeNode modelsNode;

        public MapModel MapModel
        {
            get
            {
                return editor.MapModel;
            }
            set
            {
                editor.MapModel = value;
            }
        }

        public DTE2 ApplicationObject
        {
            get { return applicationObject; }
            set
            {
                applicationObject = value;
            }
        }
        
        public Window Window
        {
            get { return window; }
            set
            {
                window = value;
            }
        }

        public ObjectProperties ObjectProperties
        {
            get
            {
                return objectProperties;
            }
        }

        public Editor Editor
        {
            get
            {
                return editor;
            }
        }

        public ClassManager _ClassManager
        {
            get { return classManager; }
            set
            {
                classManager = value;
                classManager.MapModel = MapModel;
                if (applicationObject != null && window != null)
                {
                    string heightMapFile = Path.GetDirectoryName(classManager.ContentProject.FullName) + "\\heightmap_" + classManager.Name + ".png",
                           gridMapFile = Path.GetDirectoryName(classManager.ContentProject.FullName) + "\\gridmap_" + classManager.Name + ".png";

                    if (isOpen)
                    {
                        Dictionary<string, BaseObject> objects = classManager.ReadCodeLines();
                        foreach (string key in objects.Keys)
                        {
                            if (objects[key] is DrawingObject)
                            {
                                DrawingObject obj = objects[key] as DrawingObject;
                                string file = obj.SourceFile;
                                string name = System.IO.Path.GetFileNameWithoutExtension(file);
                                if (obj.PhysicsShapeKind == PhysicsShapeKind.ConvexHullShape)
                                    editor.AddObject(file, key, obj.Position, obj.EulerRotation, obj.Scale, obj.PhysicsEnabled, obj.IsActive, obj.IsStatic, obj.CharacterControllerEnabled, obj.Scripts, obj.PhysicsShapeKind, obj.BodyPosition);
                                else
                                    editor.AddObject(file, key, obj);
                            }
                            else if (objects[key] is DrawingCamera)
                            {
                                DrawingCamera cam = objects[key] as DrawingCamera;
                                MapModel.MainCamera.Position = cam.Position;
                                MapModel.MainCamera.EulerRotation = cam.EulerRotation;
                            }
                        }
                        editor.ImportHeightMapAndGridMap(heightMapFile, gridMapFile);
                    }

                    string scriptDir = Path.GetDirectoryName(classManager.CurrentProject.FullName) + "\\Scripts";
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
                    
                    classManager.AddHeightMapToContentProject(isOpen, heightMapFile);
                    classManager.AddGridMapToContentProject(isOpen, gridMapFile);
                    window.Caption = classManager.Name + ".cs";
                }
            }
        }

        public bool IsOpen
        {
            get { return isOpen; }
            set { isOpen = value; }
        }

        public ProjectItem ProjectItem
        {
            get { return projectItem; }
            set { projectItem = value; }
        }

        public ToolStripStatusLabel StatusStrip1
        {
            get
            {
                return toolStripStatusLabel1;
            }
        }

        public FileSystemWatcher FileSystemWatcher
        {
            get { return fileSystemWatcher; }
            set { fileSystemWatcher = value; }
        }

        public MainUserControl()
        {
            InitializeComponent();
        }

        private void MainUserControl_Load(object sender, EventArgs e)
        {
            try
            {
                objectProperties.MainUserControl = this;
                
                MapModel.Attach(this);
                //TrueModel.Instance.MapModels.Add(MapModel);

                editor.MainUserControl = this;
                editor.Camera.Notify();
                
                classManager = new ClassManager();
                modelsNode = assetTreeView.Nodes.Add("Models");
                scriptsNode = assetTreeView.Nodes.Add("Scripts");

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

                UpdateObserver();
            }
            catch (Exception ex)
            {
                if (!DesignMode)
                    toolStripStatusLabel1.Text = ex.Message + "\r\n" + ex.StackTrace;
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
            {
                TreeNode node = nodes.Add(Path.GetFileName(value));
                node.Tag = value;
            }
            else
                assetTreeView.Invoke(new AddScriptNodeDelegate(AddScriptNode), new object[2] { nodes, value });
        }

        private delegate void ClearScriptsNodeDelegate(TreeNodeCollection nodes);

        void ClearScriptsNode(TreeNodeCollection nodes)
        {
            if (!assetTreeView.InvokeRequired)
                nodes.Clear();
            else
                assetTreeView.Invoke(new ClearScriptsNodeDelegate(ClearScriptsNode), new object[1] { nodes });
        }

        private void UpdateScriptAssets()
        {
            if (Directory.Exists(fileSystemWatcher.Path))
            {
                try
                {
                    ClearScriptsNode(scriptsNode.Nodes);
                    foreach (string file in Directory.EnumerateFiles(fileSystemWatcher.Path))
                    {
                        AddScriptNode(scriptsNode.Nodes, file);
                    }
                }
                catch (Exception ex)
                {
                    toolStripStatusLabel1.Text = ex.Message + "\r\n" + ex.StackTrace;
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
            classManager.AddHeightMapToContentProject();
            classManager.AddGridMapToContentProject();
        }

        private void openFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
        }

        public void UpdateObserver()
        {
            objectTreeView.Nodes.Clear();
            foreach (BaseObject obj in MapModel.Objects)
            {
                DrawingObject dObj = (obj is DrawingObject) ? obj as DrawingObject : null;
                objectTreeView.Nodes.Add(obj.Name);
                if (dObj == null)
                    continue;
                bool assetListed = false;
                foreach (TreeNode node in modelsNode.Nodes)
                {
                    if (node.Tag as string == dObj.SourceFile)
                    {
                        assetListed = true;
                    }
                }
                if (!assetListed)
                {
                    TreeNode node = modelsNode.Nodes.Add(Path.GetFileName(dObj.SourceFile));
                    node.Tag = dObj.SourceFile;
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

        }

        private void cameraPropertiesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CameraProperties cameraProperties = new CameraProperties();
            cameraProperties.ObjectProperties.Model = editor.Camera;
            editor.Camera.Attach(cameraProperties.ObjectProperties);
            cameraProperties.ObjectProperties.MainUserControl = this;
            cameraProperties.Show();
        }

        private void assetTreeView_ItemDrag(object sender, ItemDragEventArgs e)
        {
            if (e.Item is TreeNode)
            {
                TreeNode node = e.Item as TreeNode;
                DataObject data = new DataObject();
                data.SetData(DataFormats.FileDrop, true, new object[] { node.Tag });
                DoDragDrop(data, DragDropEffects.Copy);
            }
        }

        private void assetTreeView_DragEnter(object sender, DragEventArgs e)
        {
            e.Effect = DragDropEffects.Copy;
        }

        private void objectTreeView_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            BaseObject temp = MapModel.getObjectByName(e.Node.Text);
            if (temp != null)
            {
                editor.DeselectObject();
                editor.SelectObject(temp);
                editor.Camera.Notify();
            }
        }

        private void objectTreeView_NodeMouseDoubleClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            BaseObject temp = MapModel.getObjectByName(e.Node.Text);
            if (temp != null)
            {
                float length = 10;
                Vector3 target = temp.Position;
                if (temp == editor.SelectedBoundingBox.Model && temp is DrawingObject)
                {
                    DrawingObject obj = temp as DrawingObject;
                    target -= obj.Center * obj.Scale * obj.Direction;
                    BoundingBox bbox = editor.SelectedBoundingBox.BoundingBoxBuffer.BoundingBox;
                    length = ((bbox.Max - bbox.Min) * obj.Scale).Length();
                    if (length < 10)
                        length = 10;
                }

                target -= editor.Camera.Direction * length;
                
                editor.EaseCamera(target);
            }
        }
    }
}
