using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using EditorModel;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

namespace View
{
    public partial class Editor : UserControl, IObserver
    {
        private MapModel mapModel;
        
        public MapModel MapModel
        {
            get { return mapModel; }
            set { mapModel = value; }
        }

        private MainUserControl mainUserControl;

        public MainUserControl MainUserControl
        {
            get { return mainUserControl; }
            set
            {
                mainUserControl = value;
            }
        }

        private Camera camera;

        public Camera Camera
        {
            get { return camera; }
        }

        private ContentBuilder contentBuilder;

        public ContentBuilder ContentBuilder
        {
            get { return contentBuilder; }
        }

        private DrawingObject selected;

        public DrawingObject Selected
        {
            get { return selected; }
            set { selected = value; }
        }

        private ModelBoundingBox selectedBoundingBox;

        public ModelBoundingBox SelectedBoundingBox
        {
            get { return selectedBoundingBox; }
        }

        private EditorMode editorMode;

        public GraphicsDevice GraphicsDevice
        {
            get
            {
                return graphicsDeviceControl1.GraphicsDevice;
            }
        }

        private SpriteBatch spriteBatch;

        public SpriteBatch SpriteBatch
        {
            get { return spriteBatch; }
        }

        private SpriteFont spriteFont;

        public SpriteFont SpriteFont
        {
            get { return spriteFont; }
        }

        private Terrain terrain;

        public Terrain Terrain
        {
            get { return terrain; }
        }

        private BasicEffect basicEffect;
        private Effect terrainEffect;
        private bool moveForward;
        private bool moveBackward;
        private bool moveLeft;
        private bool moveRight;
        private ContentManager contentManager;
        private Texture2D heightMap;

        public Editor()
        {
            InitializeComponent();
        }

        private void Editor_Load(object sender, EventArgs e)
        {            
            basicEffect = new BasicEffect(graphicsDeviceControl1.GraphicsDevice);
            contentBuilder = ContentBuilder.Instance;
            contentManager = new ContentManager(graphicsDeviceControl1.Services, contentBuilder.OutputDirectory);
            spriteBatch = new SpriteBatch(graphicsDeviceControl1.GraphicsDevice);
            
            string errorBuild = "";
            try
            {
                //importer reference: http://msdn.microsoft.com/en-us/library/bb447762%28v=xnagamestudio.20%29.aspx
                contentBuilder.Add(AssemblyDirectory + "\\SegoeUI.spritefont", "SegoeUI.spritefont", null, "FontDescriptionProcessor");
                contentBuilder.Add(AssemblyDirectory + "\\effects.fx", "effects", null, "EffectProcessor");
                contentBuilder.Add(AssemblyDirectory + "\\heightmap.bmp", "heightmap", null, "TextureProcessor");
                errorBuild = contentBuilder.Build();
                spriteFont = contentManager.Load<SpriteFont>("SegoeUI.spritefont");
                terrainEffect = contentManager.Load<Effect>("effects");
                heightMap = contentManager.Load<Texture2D>("heightmap");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "\r\n" + ex.StackTrace + "\r\n" + errorBuild);
            }

            terrain = new Terrain(heightMap);

            camera = new Camera();
            camera.Position = new Vector3(0, terrain.HeightData[0, 0] + 1, 0);
            camera.AspectRatio = graphicsDeviceControl1.GraphicsDevice.Viewport.AspectRatio;
            camera.Rotate(0, 135, 0);
            camera.Attach(this);

            selected = null;
            selectedBoundingBox = new ModelBoundingBox(graphicsDeviceControl1.GraphicsDevice, camera);
            selectedBoundingBox.SpriteBatch = spriteBatch;
            selectedBoundingBox.SpriteFont = spriteFont;
            CheckActiveTransformMode();

            editorMode = new EditorMode_Select(this);

            graphicsDeviceControl1.Invalidate();
        }

        void IObserver.Update()
        {
            graphicsDeviceControl1.Invalidate();
        }

        public void AddObject(string file, string name, Vector3 position, Vector3 eulerRotation)
        {
            DrawingObject obj = new DrawingObject();

            string originalName = name;

            for (int i = 1; ; ++i)
            {
                if (!mapModel.NameExists(name))
                    break;
                name = originalName + "_" + i;
            }
            
            obj.DrawingModel = OpenModel(file);
            obj.Name = name;
            obj.Position = position;
            obj.EulerRotation = eulerRotation;
            obj.SourceFile = file;
            obj.Attach(this);
            mapModel.Objects.Add(obj);
            Generator.CodeLines codeLines = new Generator.CodeLines();
            codeLines.Model = obj;
            mainUserControl._ClassManager.CodeLinesList.Add(codeLines);
            obj.Notify();
        }

        public Model LoadModel()
        {
            if (openFileDialog1.ShowDialog() != DialogResult.OK)
            {
                return null;
            }
            return OpenModel(openFileDialog1.FileName);
        }

        public Model OpenModel(string path)
        {
            bool isAdded = false;
            foreach (Microsoft.Build.Evaluation.ProjectItem item in contentBuilder.ProjectItems)
            {
                foreach (Microsoft.Build.Evaluation.ProjectMetadata metadata in item.Metadata)
                {
                    if (metadata.Name == "Link" && metadata.EvaluatedValue == System.IO.Path.GetFileName(path))
                    {
                        isAdded = true;
                    }
                }
            }

            string name = System.IO.Path.GetFileNameWithoutExtension(path);

            if (!isAdded)
            {
                contentBuilder.Add(path, name, null, "ModelProcessor");
                string errorBuild = contentBuilder.Build();
                if (string.IsNullOrEmpty(errorBuild))
                {
                    Model model = contentManager.Load<Model>(name);
                    graphicsDeviceControl1.Invalidate();
                    return model;
                }
            }
            else
            {
                Model model = contentManager.Load<Model>(name);
                graphicsDeviceControl1.Invalidate();
                return model;
            }
            return null;
        }
        
        private void graphicsDeviceControl1_Paint(object sender, PaintEventArgs e)
        {
            graphicsDeviceControl1.GraphicsDevice.Clear(ClearOptions.Target | ClearOptions.DepthBuffer, Microsoft.Xna.Framework.Color.Black, 1.0f, 0);

            if (mapModel != null)
                foreach (DrawingObject obj in mapModel.Objects)
                    obj.Draw(camera.World, camera.Projection);
            
            basicEffect.View = camera.World;
            basicEffect.Projection = camera.Projection;

            try
            {
                terrain.Draw(basicEffect, terrainEffect, graphicsDeviceControl1.GraphicsDevice, camera);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "\r\n" + ex.StackTrace);
            }

            if (selected != null)
                selectedBoundingBox.Draw(ref basicEffect);
        }

        private void graphicsDeviceControl1_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            if (e.KeyCode == Keys.W)
                moveForward = true;
            if (e.KeyCode == Keys.S)
                moveBackward = true;
            if (e.KeyCode == Keys.A)
                moveLeft = true;
            if (e.KeyCode == Keys.D)
                moveRight = true;
            if (moveForward || moveBackward || moveLeft || moveRight)
                timer1.Enabled = true;
        }

        private void graphicsDeviceControl1_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.W)
                moveForward = false;
            if (e.KeyCode == Keys.S)
                moveBackward = false;
            if (e.KeyCode == Keys.A)
                moveLeft = false;
            if (e.KeyCode == Keys.D)
                moveRight = false;
            if (!(moveForward || moveBackward || moveLeft || moveRight))
                timer1.Enabled = false;
        }

        private void graphicsDeviceControl1_MouseDown(object sender, MouseEventArgs e)
        {
            editorMode.MouseDown(sender, e);
        }

        private void graphicsDeviceControl1_MouseMove(object sender, MouseEventArgs e)
        {
            editorMode.MouseMove(sender, e);
        }

        private void graphicsDeviceControl1_MouseUp(object sender, MouseEventArgs e)
        {
            editorMode.MouseUp(sender, e);
        }
        
        private void graphicsDeviceControl1_DragEnter(object sender, DragEventArgs e)
        {
            editorMode.DragEnter(sender, e);
        }

        private void graphicsDeviceControl1_DragDrop(object sender, DragEventArgs e)
        {
            editorMode.DragDrop(sender, e);
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            float speed = (float)0.1;
            if (moveRight)
                camera.MoveRight(speed);
            if (moveLeft)
                camera.MoveRight(-speed);
            if (moveForward)
                camera.MoveForward(speed);
            if (moveBackward)
                camera.MoveForward(-speed);
            if (moveRight || moveLeft || moveForward || moveBackward)
                camera.Notify();
        }

        private void graphicsDeviceControl1_Resize(object sender, EventArgs e)
        {
            if (camera == null || graphicsDeviceControl1.GraphicsDevice == null)
                return;
            camera.AspectRatio = graphicsDeviceControl1.GraphicsDevice.Viewport.AspectRatio;
            camera.Notify();
        }

        private void translateModeToolStripButton_Click(object sender, EventArgs e)
        {
            selectedBoundingBox.ChangeAxisLines(1);
            CheckActiveTransformMode();
        }

        private void rotateModeToolStripButton_Click(object sender, EventArgs e)
        {
            selectedBoundingBox.ChangeAxisLines(2);
            CheckActiveTransformMode();
        }

        private void CheckActiveTransformMode()
        {
            if (selectedBoundingBox.AxisLines.GetType() == typeof(RotationAxisLines))
            {
                translateModeToolStripButton.Checked = false;
                translateModeToolStripButton.CheckState = CheckState.Unchecked;
                rotateModeToolStripButton.Checked = true;
                rotateModeToolStripButton.CheckState = CheckState.Checked;
            }
            else
            {
                translateModeToolStripButton.Checked = true;
                translateModeToolStripButton.CheckState = CheckState.Checked;
                rotateModeToolStripButton.Checked = false;
                rotateModeToolStripButton.CheckState = CheckState.Unchecked;
            }
        }

        public static string AssemblyDirectory
        {
            get
            {
                string codeBase = System.Reflection.Assembly.GetExecutingAssembly().CodeBase;
                UriBuilder uri = new UriBuilder(codeBase);
                string path = Uri.UnescapeDataString(uri.Path);
                return System.IO.Path.GetDirectoryName(path);
            }
        }

        private void Editor_Resize(object sender, EventArgs e)
        {
            graphicsDeviceControl1.Invalidate();
        }

        private void DrawGrid(BasicEffect basicEffect)
        {
            basicEffect.World = Matrix.Identity;
            basicEffect.VertexColorEnabled = true;

            List<VertexPositionColor> vertices = new List<VertexPositionColor>();
            for (int i = 0; i < 10; i++)
            {
                vertices.Add(new VertexPositionColor(new Vector3((i - 2) * 2 - 5, -1, -10), Microsoft.Xna.Framework.Color.White));
                vertices.Add(new VertexPositionColor(new Vector3((i - 2) * 2 - 5, -1, 10), Microsoft.Xna.Framework.Color.White));
                vertices.Add(new VertexPositionColor(new Vector3(-10, -1, (i - 2) * 2 - 5), Microsoft.Xna.Framework.Color.White));
                vertices.Add(new VertexPositionColor(new Vector3(10, -1, (i - 2) * 2 - 5), Microsoft.Xna.Framework.Color.White));
            }

            VertexPositionColor[] vs = vertices.ToArray();
            foreach (EffectPass pass in basicEffect.CurrentTechnique.Passes)
            {
                pass.Apply();
                graphicsDeviceControl1.GraphicsDevice.DrawUserPrimitives(PrimitiveType.LineList, vs, 0, vertices.Count / 2);
            }
        }
    }
}
