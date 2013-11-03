using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using EditorModel;
using EditorModel.QuadTreeTerrain;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using XleGenerator;

namespace View
{
    public partial class Editor : UserControl, IObserver
    {
        #region attributes
        private MapModel mapModel;
        private MainUserControl mainUserControl;
        private Camera camera;
        private ContentBuilder contentBuilder;
        private ModelBoundingBox selectedBoundingBox;
        private EditorMode editorMode;
        private SpriteBatch spriteBatch;
        private SpriteFont spriteFont;
        private Terrain terrain;
        private string text;
        private TerrainBrush terrainBrush;
        private BasicEffect basicEffect;
        private Effect terrainEffect;
        private bool moveForward;
        private bool moveBackward;
        private bool moveLeft;
        private bool moveRight;
        private ContentManager contentManager;
        private Texture2D brushHeightMap;
        private bool mouseMoving;
        private MouseEventArgs mouseEventArgs;
        private int tempMouseX;
        private int tempMouseY;
        private ContentManager heightmapContent;
        private string effectFile;
        private Texture2D grassTexture;
        private Grid grid;
        private GridPointer gridPointer;
        private List<GridPointer> gridPointers;
        private QuadTree quadTreeTerrain;
        private bool viewBody;
        private System.Threading.Timer timer;
        private int easeRemain = 0;
        Vector3 easeSpeed;
        Vector3 easeAngularSpeed;
        #endregion

        public MapModel MapModel
        {
            get { return mapModel; }
            set
            {
                if (mapModel != null)
                {
                    mapModel.PhysicsWorld.Detach(this);
                }
                mapModel = value;
                if (mapModel == null)
                    return;
                if (contentManager != null)
                {
                    mapModel.MainCamera.Texture = contentManager.Load<Texture2D>("video_camera");
                    mapModel.MainCamera.GraphicsDevice = GraphicsDevice;
                    mapModel.MainCamera.Camera = camera;
                    mapModel.MainCamera.BasicEffect = basicEffect;
                }
                mapModel.PhysicsWorld.Attach(this);
                mapModel.PhysicsWorld.Notify();
            }
        }

        public MainUserControl MainUserControl
        {
            get { return mainUserControl; }
            set
            {
                mainUserControl = value;
            }
        }

        public Camera Camera
        {
            get { return camera; }
        }

        public ContentBuilder ContentBuilder
        {
            get { return contentBuilder; }
        }

        public BaseObject Selected
        {
            get
            {
                if (mapModel == null) return null;
                return mapModel.Selected;
            }
            set
            {
                if (mapModel == null) return;
                mapModel.Selected = value;
            }
        }

        public ModelBoundingBox SelectedBoundingBox
        {
            get { return selectedBoundingBox; }
        }

        public GraphicsDevice GraphicsDevice
        {
            get
            {
                return graphicsDeviceControl1.GraphicsDevice;
            }
        }

        public SpriteBatch SpriteBatch
        {
            get { return spriteBatch; }
        }

        public SpriteFont SpriteFont
        {
            get { return spriteFont; }
        }

        public Terrain Terrain
        {
            get { return terrain; }
        }

        public string Text1
        {
            get { return text; }
            set { text = value; }
        }

        public TerrainBrush TerrainBrush
        {
            get { return terrainBrush; }
            set { terrainBrush = value; }
        }

        public Texture2D GrassTexture
        {
            get { return grassTexture; }
            set { grassTexture = value; }
        }

        public Grid Grid
        {
            get { return grid; }
            set { grid = value; }
        }

        public GridPointer GridPointer
        {
            get { return gridPointer; }
            set { gridPointer = value; }
        }

        public List<GridPointer> GridPointers
        {
            get { return gridPointers; }
            set { gridPointers = value; }
        }

        public bool ViewBody
        {
            get { return viewBody; }
            set
            {
                viewBody = value;
                if (selectedBoundingBox != null)
                    selectedBoundingBox.ViewBody = value;
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

        public Editor()
        {
            InitializeComponent();
        }

        private void Editor_Load(object sender, EventArgs e)
        {
            string errorBuild = "";
            try
            {
                mouseMoving = false;
                mouseEventArgs = null;
                tempMouseX = tempMouseY = -1;
                text = "";
                basicEffect = new BasicEffect(graphicsDeviceControl1.GraphicsDevice);
                contentBuilder = ContentBuilder.Instance;
                contentManager = new ContentManager(graphicsDeviceControl1.Services, contentBuilder.OutputDirectory);
                heightmapContent = new ContentManager(graphicsDeviceControl1.Services, contentBuilder.OutputDirectory);
                spriteBatch = new SpriteBatch(graphicsDeviceControl1.GraphicsDevice);
                //MessageBox.Show(GraphicsDevice.Adapter.Description);

                effectFile = AssemblyDirectory + "\\Assets\\effects.fx";
                //importer reference: http://msdn.microsoft.com/en-us/library/bb447762%28v=xnagamestudio.20%29.aspx
                contentBuilder.Add(AssemblyDirectory + "\\Assets\\SegoeUI.spritefont", "SegoeUI.spritefont", null, "FontDescriptionProcessor");
                contentBuilder.Add(effectFile, "effects", null, "EffectProcessor");
                contentBuilder.Add(AssemblyDirectory + "\\Assets\\brush.bmp", "brush", null, "TextureProcessor");
                contentBuilder.Add(AssemblyDirectory + "\\Assets\\Textures\\grass.dds", "grass", null, "TextureProcessor");
                contentBuilder.Add(AssemblyDirectory + "\\Assets\\video_camera.png", "video_camera", null, "TextureProcessor");
                contentBuilder.Add(AssemblyDirectory + "\\Assets\\Roads\\jalan_raya.fbx", "jalan_raya", null, "ModelProcessor");
                contentBuilder.Add(AssemblyDirectory + "\\Assets\\Roads\\jalan_raya_belok.fbx", "jalan_raya_belok", null, "ModelProcessor");
                contentBuilder.Add(AssemblyDirectory + "\\Assets\\heightmap2.png", "heightmap", null, "TextureProcessor");
                string error = contentBuilder.Build();
                if (!string.IsNullOrEmpty(error))
                {
                    throw new Exception(error);
                }

                errorBuild = contentBuilder.Build();
                spriteFont = contentManager.Load<SpriteFont>("SegoeUI.spritefont");
                terrainEffect = contentManager.Load<Effect>("effects");
                brushHeightMap = contentManager.Load<Texture2D>("brush");
                grassTexture = contentManager.Load<Texture2D>("grass");

                camera = new Camera(GraphicsDevice);
                camera.Position = new Vector3(0, 50, 0);
                camera.AspectRatio = graphicsDeviceControl1.GraphicsDevice.Viewport.AspectRatio;
                camera.Rotate(20, 45, 0);
                camera.Attach(this);
                CheckIsOrthographic();

                string heightMapFile = AssemblyDirectory + "\\test_HeightMap.png";
                terrain = new Terrain(GraphicsDevice, camera, heightMapFile, effectFile, 128, 128);
                terrain.Texture = grassTexture;

                //heightmapContent = new ContentManager(graphicsDeviceControl1.Services, contentBuilder.OutputDirectory);
                //quadTreeTerrain = new QuadTree(Vector3.Zero, 1025, 1025, camera, GraphicsDevice, 1);
                //quadTreeTerrain.Effect.Texture = grassTexture;

                editorMode = new EditorMode_Select(this);

                Selected = null;
                selectedBoundingBox = new ModelBoundingBox(graphicsDeviceControl1.GraphicsDevice, camera, false);
                selectedBoundingBox.SpriteBatch = spriteBatch;
                selectedBoundingBox.SpriteFont = spriteFont;
                CheckActiveTransformMode();

                grid = new Grid(terrain, 8, camera, GraphicsDevice, basicEffect);
                grid.RoadModel = contentManager.Load<Model>("jalan_raya");
                grid.RoadModel_belok = contentManager.Load<Model>("jalan_raya_belok");

                terrainBrush = new TerrainBrush(GraphicsDevice, terrain, brushHeightMap);
                terrainBrush.BasicEffect = basicEffect;

                gridPointer = new GridPointer(grid);
                gridPointers = new List<GridPointer>();

                if (mapModel != null)
                {
                    mapModel.MainCamera.Texture = contentManager.Load<Texture2D>("video_camera");
                    mapModel.MainCamera.GraphicsDevice = GraphicsDevice;
                    mapModel.MainCamera.Camera = camera;
                    mapModel.MainCamera.BasicEffect = basicEffect;
                }

                timer = new System.Threading.Timer((c) => SetGravity(), null, Timeout.Infinite, Timeout.Infinite);

                camera.Notify();
            }
            catch (Exception ex)
            {
                if (!DesignMode)
                {
                    if (mainUserControl != null && mainUserControl.StatusStrip1 != null)
                        mainUserControl.StatusStrip1.Text = ex.Message + "\r\n" + ex.StackTrace;
                    else
                        MessageBox.Show(ex.Message + "\r\n" + ex.StackTrace);
                }
            }
        }

        void UpdateGravity()
        {
            if (gravityToolStripTextBox.TextBox.InvokeRequired)
                gravityToolStripTextBox.TextBox.Invoke(new Action(UpdateGravity));
            else
            {
                gravityToolStripTextBox.Text = mapModel.PhysicsWorld.Gravity.ToString();
                materialCoefficientMixingToolStripComboBox.SelectedIndex = (int)mapModel.PhysicsWorld.MaterialCoefficientMixing;
            }
        }

        public void UpdateObserver()
        {
            if (mapModel != null)
            {
                UpdateGravity();
            }
            graphicsDeviceControl1.Invalidate();
        }

        public void ImportHeightmap(string heightmapFile)
        {
            heightmapContent = new ContentManager(graphicsDeviceControl1.Services, contentBuilder.OutputDirectory);
            contentBuilder.Add(heightmapFile, "heightmap", null, "TextureProcessor");
            contentBuilder.Build();
            camera.Detach(terrain.TerrainIndexer);
            terrain = new Terrain(GraphicsDevice, camera, heightmapContent.Load<Texture2D>("heightmap"), heightmapFile, effectFile);
            terrain.Texture = grassTexture;

            terrainBrush.Terrain = terrain;

            camera.Attach(terrain.TerrainIndexer);
            camera.Notify();
        }

        public BaseObject AddObject(string file, string name, Vector3 position, Vector3 eulerRotation, Vector3 scale, bool physicsEnabled, bool isActive, bool isStatic, bool characterControllerEnabled, EditorModel.PropertyModel.ScriptCollection scripts, PhysicsShapeKind physicsShapeKind, Vector3 bodyPosition)
        {
            DrawingObject obj = new DrawingObject();
            SetPositionRotationScale(ref position, ref eulerRotation, ref scale, obj);
            obj.PhysicsEnabled = physicsEnabled;
            obj.IsActive = isActive;
            obj.IsStatic = isStatic;
            obj.CharacterControllerEnabled = characterControllerEnabled;
            obj.Scripts = scripts;
            obj.PhysicsShapeKind = physicsShapeKind;
            obj.BodyPosition = bodyPosition;
            return AddObject(file, name, obj);
        }

        public BaseObject AddObject(string file, string name, Vector3 position, Vector3 eulerRotation, Vector3 scale)
        {
            DrawingObject obj = new DrawingObject();
            SetPositionRotationScale(ref position, ref eulerRotation, ref scale, obj);
            return AddObject(file, name, obj);
        }

        private static void SetPositionRotationScale(ref Vector3 position, ref Vector3 eulerRotation, ref Vector3 scale, DrawingObject obj)
        {
            obj.Position = position;
            obj.EulerRotation = eulerRotation;
            obj.Scale = scale;
        }

        private void SetPositionRotationScale(DrawingObject obj, Vector3 position, Vector3 eulerRotation, Vector3 scale)
        {
        }

        public BaseObject AddObject(string file, string name, DrawingObject obj)
        {
            string originalName = name;

            for (int i = 1; ; ++i)
            {
                if (!mapModel.NameExists(name))
                    break;
                name = originalName + "_" + i;
            }
            try
            {
                obj.DrawingModel = OpenModel(file);
                if (obj.PhysicsShapeKind == PhysicsShapeKind.ConvexHullShape)
                    obj.PhysicsShapeKind = obj.PhysicsShapeKind;
                else
                    obj.PhysicsShape = obj.PhysicsShape;
                obj.Name = name;
                obj.SourceFile = file;
                obj.Camera = camera;
                obj.GraphicsDevice = GraphicsDevice;
                obj.Attach(this);
                mapModel.Objects.Add(obj);
                XleGenerator.CodeLines codeLines = new XleGenerator.CodeLines();
                codeLines.Model = obj;
                mainUserControl._ClassManager.CodeLinesList.Add(codeLines);
                obj.Notify();
                return obj;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "\r\n" + ex.StackTrace);
                return null;
            }
        }

        public void DeleteObject(BaseObject obj)
        {
            if (obj == null || obj == mapModel.MainCamera)
                return;
            obj.DetachAll();
            List<CodeLines> codeLinesToBeRemovedList = new List<CodeLines>();
            foreach (CodeLines codeLines in mainUserControl._ClassManager.CodeLinesList)
            {
                if (codeLines.Model == obj)
                    codeLinesToBeRemovedList.Add(codeLines);
            }
            foreach (CodeLines codeLinesToBeRemoved in codeLinesToBeRemovedList)
            {
                mainUserControl._ClassManager.CodeLinesList.Remove(codeLinesToBeRemoved);
            }
            mapModel.Objects.Remove(obj);
            camera.Notify();
        }

        public void DeleteObject(string name)
        {
            BaseObject obj = mapModel.getObjectByName(name);
            DeleteObject(obj);
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
                else
                {
                    contentBuilder.Remove(name);
                    throw new ArgumentException(errorBuild);
                }
            }
            else
            {
                Model model = contentManager.Load<Model>(name);
                graphicsDeviceControl1.Invalidate();
                return model;
            }
        }
        
        private void graphicsDeviceControl1_Paint(object sender, PaintEventArgs e)
        {
            try
            {
                graphicsDeviceControl1.GraphicsDevice.Clear(ClearOptions.Target | ClearOptions.DepthBuffer, Microsoft.Xna.Framework.Color.CornflowerBlue, 1.0f, 0);

                GraphicsDevice.BlendState = BlendState.Opaque;
                GraphicsDevice.DepthStencilState = DepthStencilState.Default;
                GraphicsDevice.SamplerStates[0] = SamplerState.LinearWrap;

                if (mapModel != null)
                    foreach (BaseObject obj in mapModel.Objects)
                        obj.Draw(spriteBatch, false, camera.Direction);

                basicEffect.View = camera.World;
                basicEffect.Projection = camera.Projection;

                terrain.Draw();
                //quadTreeTerrain.Update(null);
                //quadTreeTerrain.Draw(null);

                if (editorMode is EditorMode_Terrain)
                {
                    if (editorMode is EditorMode_GridPointing)
                    {
                        grid.Draw();
                        gridPointer.Draw(spriteBatch);
                        text += gridPointer.Text + " " + gridPointers.Count + "\r\n";
                        foreach (GridPointer gp in gridPointers)
                            gp.Draw(spriteBatch);
                    }
                    else
                    {
                        terrainBrush.Draw();
                        text += terrainBrush.Text;
                    }
                }

                for (int x = 0; x < grid.Width; x++)
                {
                    for (int y = 0; y < grid.Height; y++)
                        if (grid.GridObjects[x, y] != null)
                            grid.GridObjects[x, y].Draw(spriteBatch);
                }
                if (Selected != null)
                {
                    selectedBoundingBox.Draw(ref basicEffect);
                    text += selectedBoundingBox.AxisLines.Text + "\r\n";
                }

                text += camera.Zoom + "\r\n";

                spriteBatch.Begin();
                foreach (BaseObject obj in mapModel.Objects)
                    obj.DrawSprite(spriteBatch);
                if (spriteFont != null)
                {
                    text += timer1.Enabled + "\r\n";
                    if (mouseEventArgs != null)
                    {
                        text += mouseEventArgs.X + "," + mouseEventArgs.Y + "\r\n";
                    }
                    //spriteBatch.DrawString(spriteFont, text, new Vector2(50, 50), Microsoft.Xna.Framework.Color.White, 0.0f, Vector2.Zero, 1.0f, SpriteEffects.None, 0.0f);
                }
                spriteBatch.End();

                text = "";
            }
            catch (Exception ex)
            {
                if (!DesignMode)
                {
                    if (mainUserControl != null && mainUserControl.StatusStrip1 != null)
                        mainUserControl.StatusStrip1.Text = ex.Message + "\r\n" + ex.StackTrace;
                    else
                        MessageBox.Show(ex.Message + "\r\n" + ex.StackTrace);
                }
            }
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
            editorMode.PreviewKeyDown(sender, e);
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
            {
                timer1.Enabled = false;
                camera.IsMoving = false;
                camera.Notify();
            }
            editorMode.KeyUp(sender, e);
        }

        private void graphicsDeviceControl1_MouseDown(object sender, MouseEventArgs e)
        {
            editorMode.MouseDown(sender, e);
        }

        private void graphicsDeviceControl1_MouseMove(object sender, MouseEventArgs e)
        {
            try
            {
                if (!mouseMoving)
                    mouseMoving = true;

                if (!timer1.Enabled)
                    timer1.Enabled = true;

                mouseEventArgs = e;
            }
            catch (Exception ex)
            {
                if (!DesignMode)
                    mainUserControl.StatusStrip1.Text = ex.Message + "\r\n" + ex.StackTrace;
            }
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
            float speed = (float)1;
            if (moveRight)
                camera.MoveRight(speed);
            if (moveLeft)
                camera.MoveRight(-speed);
            if (moveForward)
            {
                if (!camera.IsOrthographic)
                    camera.MoveForward(speed);
                else
                    camera.Zoom -= 5;
            }
            if (moveBackward)
            {
                if (!camera.IsOrthographic)
                    camera.MoveForward(-speed);
                else
                    camera.Zoom += 5;
            }
            if (mouseMoving)
            {
                if (mouseEventArgs != null && mouseEventArgs.X == tempMouseX && mouseEventArgs.Y == tempMouseY)
                {
                    mouseMoving = false;
                    mouseEventArgs = null;
                    tempMouseX = tempMouseY = -1;
                    timer1.Enabled = false;
                    camera.Notify();
                }
                else
                {
                    editorMode.MouseMove(sender, mouseEventArgs);

                    if (mouseEventArgs != null)
                    {
                        tempMouseX = mouseEventArgs.X;
                        tempMouseY = mouseEventArgs.Y;
                    }
                }
            }
            if (easeRemain > 0)
            {
                if (easeAngularSpeed == Vector3.Zero)
                    camera.Move(easeSpeed);
                else
                    camera.Move(easeSpeed, easeAngularSpeed);
                easeRemain--;
                if (easeRemain == 0)
                {
                    easeRemain = 0;
                    timer1.Enabled = false;
                    camera.IsMoving = false;
                }
                else
                    timer1.Enabled = true;
            }
            //if (moveRight || moveLeft || moveForward || moveBackward || easeRemain > 0)
            camera.Notify();
        }

        private void graphicsDeviceControl1_Resize(object sender, EventArgs e)
        {
            if (camera == null || graphicsDeviceControl1.GraphicsDevice == null)
                return;
            camera.AspectRatio = graphicsDeviceControl1.GraphicsDevice.Viewport.AspectRatio;
            terrain.TerrainIndexer.Resize();
            camera.Notify();
        }

        private void translateModeToolStripButton_Click(object sender, EventArgs e)
        {
            if (Selected != null)
                selectedBoundingBox.ChangeAxisLines(1);
            CheckActiveTransformMode();
        }

        private void rotateModeToolStripButton_Click(object sender, EventArgs e)
        {
            if (Selected != null)
                selectedBoundingBox.ChangeAxisLines(2);
            CheckActiveTransformMode();
        }

        private void scaleModeToolStripButton_Click(object sender, EventArgs e)
        {
            if (Selected != null)
                selectedBoundingBox.ChangeAxisLines(3);
            CheckActiveTransformMode();
        }

        private void CheckActiveTransformMode()
        {
            if (Selected == null)
            {
                translateModeToolStripButton.Checked = false;
                translateModeToolStripButton.CheckState = CheckState.Unchecked;
                rotateModeToolStripButton.Checked = false;
                rotateModeToolStripButton.CheckState = CheckState.Unchecked;
                scaleModeToolStripButton.Checked = false;
                scaleModeToolStripButton.CheckState = CheckState.Unchecked;
            }
            if (selectedBoundingBox.AxisLines.GetType() == typeof(RotationAxisLines))
            {
                translateModeToolStripButton.Checked = false;
                translateModeToolStripButton.CheckState = CheckState.Unchecked;
                rotateModeToolStripButton.Checked = true;
                rotateModeToolStripButton.CheckState = CheckState.Checked;
                scaleModeToolStripButton.Checked = false;
                scaleModeToolStripButton.CheckState = CheckState.Unchecked;
            }
            else if (selectedBoundingBox.AxisLines.GetType() == typeof(ScaleAxisLines))
            {
                translateModeToolStripButton.Checked = false;
                translateModeToolStripButton.CheckState = CheckState.Unchecked;
                rotateModeToolStripButton.Checked = false;
                rotateModeToolStripButton.CheckState = CheckState.Unchecked;
                scaleModeToolStripButton.Checked = true;
                scaleModeToolStripButton.CheckState = CheckState.Checked;
            }
            else
            {
                translateModeToolStripButton.Checked = true;
                translateModeToolStripButton.CheckState = CheckState.Checked;
                rotateModeToolStripButton.Checked = false;
                rotateModeToolStripButton.CheckState = CheckState.Unchecked;
                scaleModeToolStripButton.Checked = false;
                scaleModeToolStripButton.CheckState = CheckState.Unchecked;
            }
            camera.Notify();
        }

        private void selectModeStripButton_Click(object sender, EventArgs e)
        {
            editorMode = new EditorMode_Select(this);
            CheckEditorMode();
        }

        private void terrainIncreaseToolStripButton_Click(object sender, EventArgs e)
        {
            try
            {
                editorMode = new EditorMode_Terrain_IncreaseHeight(this);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "\r\n" + ex.StackTrace);
            }
            CheckEditorMode();
        }

        private void terrainDecreaseToolStripButton_Click(object sender, EventArgs e)
        {
            try
            {
                editorMode = new EditorMode_Terrain_DecreaseHeight(this);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "\r\n" + ex.StackTrace);
            }
            CheckEditorMode();
        }

        private void gridStripButton_Click(object sender, EventArgs e)
        {
            try
            {
                editorMode = new EditorMode_AddRoad(this);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "\r\n" + ex.StackTrace);
            }
            CheckEditorMode();
        }

        private void CheckEditorMode()
        {
            if (Selected != null)
            {
                Selected.Detach(mainUserControl.ObjectProperties);
                mainUserControl.ObjectProperties.Model = null;
                Selected = null;
            }

            selectModeToolStripButton.Checked = false;
            selectModeToolStripButton.CheckState = CheckState.Unchecked;
            terrainIncreaseToolStripButton.Checked = false;
            terrainIncreaseToolStripButton.CheckState = CheckState.Unchecked;
            terrainDecreaseToolStripButton.Checked = false;
            terrainDecreaseToolStripButton.CheckState = CheckState.Unchecked;
            addRoadStripButton.Checked = false;
            addRoadStripButton.CheckState = CheckState.Unchecked;

            if (editorMode.GetType() == typeof(EditorMode_Select))
            {
                selectModeToolStripButton.Checked = true;
                selectModeToolStripButton.CheckState = CheckState.Checked;
            }
            if (editorMode.GetType() == typeof(EditorMode_Terrain_IncreaseHeight))
            {
                terrainIncreaseToolStripButton.Checked = true;
                terrainIncreaseToolStripButton.CheckState = CheckState.Checked;
            }
            if (editorMode.GetType() == typeof(EditorMode_Terrain_DecreaseHeight))
            {
                terrainDecreaseToolStripButton.Checked = true;
                terrainDecreaseToolStripButton.CheckState = CheckState.Checked;
            }
            if (editorMode.GetType() == typeof(EditorMode_AddRoad))
            {
                addRoadStripButton.Checked = true;
                addRoadStripButton.CheckState = CheckState.Checked;
            }

            camera.Notify();
        }

        private void orthogonalStripButton_Click(object sender, EventArgs e)
        {
            camera.IsOrthographic = !camera.IsOrthographic;
            CheckIsOrthographic();
            if (Selected != null && (camera.Position - Selected.Position).Length() < 10)
            {
                float length = 10;
                Vector3 target = Selected.Position;
                if (Selected == selectedBoundingBox.Model && Selected is DrawingObject)
                {
                    DrawingObject obj = Selected as DrawingObject;
                    BoundingBox bbox = selectedBoundingBox.BoundingBoxBuffer.BoundingBox;
                    length = ((bbox.Max - bbox.Min) * obj.Scale).Length();
                    if (length < 10)
                        length = 10;
                }

                camera.Position -= camera.Direction * length;
            }
            camera.Notify();
        }

        private void CheckIsOrthographic()
        {
            if (camera.IsOrthographic)
            {
                orthogonalStripButton.Checked = true;
                orthogonalStripButton.CheckState = CheckState.Checked;
            }
            else
            {
                orthogonalStripButton.Checked = false;
                orthogonalStripButton.CheckState = CheckState.Unchecked;
            }
        }

        public void SelectObject(BaseObject temp)
        {
            Selected = temp;
            selectedBoundingBox.Model = Selected;
            Selected.Attach(mainUserControl.ObjectProperties);
            mainUserControl.ObjectProperties.Model = Selected;
            Selected.Notify();
        }

        public void DeselectObject()
        {
            if (Selected != null)
            {
                Selected.Detach(mainUserControl.ObjectProperties);
                mainUserControl.ObjectProperties.Model = null;
                Selected = null;
            }
        }

        private void materialCoefficientMixingToolStripComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            mapModel.PhysicsWorld.MaterialCoefficientMixing = (MaterialCoefficientMixing)materialCoefficientMixingToolStripComboBox.SelectedIndex;
            mapModel.PhysicsWorld.Notify();
        }

        void SetGravity()
        {
            float gravity;
            if (float.TryParse(gravityToolStripTextBox.Text, out gravity))
            {
                mapModel.PhysicsWorld.Gravity = gravity;
            }
            mapModel.PhysicsWorld.Notify();
        }

        public void EaseCamera(Vector3 targetPosition)
        {
            EaseCamera(false, targetPosition);
        }

        public void EaseCamera(Vector3 targetPosition, Vector3 targetRotation)
        {
            EaseCamera(true, targetPosition, targetRotation);
        }

        public void EaseCamera(bool easeAngular, Vector3 targetPosition, Vector3 targetRotation = new Vector3())
        {
            Vector3 dist = targetPosition - camera.Position;
            easeSpeed = Vector3.Normalize(targetPosition - camera.Position) * 10.0f;
            easeRemain = (int)(dist.Length() / easeSpeed.Length());
            if (easeRemain < 5)
                easeRemain = 5;
            else if (easeRemain > 500 / timer1.Interval)
                easeRemain = 500 / timer1.Interval;

            easeSpeed = (targetPosition - camera.Position) / easeRemain;
            if (easeAngular)
            {
                targetRotation = new Vector3(targetRotation.X % 360, targetRotation.Y % 360, targetRotation.Z % 360);
                if (targetRotation.X < 0)
                    targetRotation.X = 360 + targetRotation.X;
                if (targetRotation.Y < 0)
                    targetRotation.Y = 360 + targetRotation.Y;
                if (targetRotation.Z < 0)
                    targetRotation.Z = 360 + targetRotation.Z;
                easeAngularSpeed = (targetRotation - camera.EulerRotation) / easeRemain;
            }
            else
            {
                easeAngularSpeed = Vector3.Zero;
            }
            
            timer1.Enabled = true;
        }

        private void gravityToolStripTextBox_TextChanged(object sender, EventArgs e)
        {
            timer.Change(250, Timeout.Infinite);
        }

        private void graphicsDeviceControl1_MouseEnter(object sender, EventArgs e)
        {
            graphicsDeviceControl1.Focus();
        }

        private void xOrthogonalStripButton_Click(object sender, EventArgs e)
        {
            OrthographicsThenEase(new Vector3(0, -90, 0));
        }

        private void yOrthogonalStripButton_Click(object sender, EventArgs e)
        {
            OrthographicsThenEase(new Vector3(90, 0, 0));
        }

        private void zOrthogonalStripButton_Click(object sender, EventArgs e)
        {
            OrthographicsThenEase(new Vector3(0, 180, 0));
        }

        private void OrthographicsThenEase(Vector3 targetRotation)
        {
            if (Selected == selectedBoundingBox.Model && Selected is DrawingObject)
            {
                DrawingObject obj = Selected as DrawingObject;
                BoundingBox bbox = selectedBoundingBox.BoundingBoxBuffer.BoundingBox;
                float length = ((bbox.Max - bbox.Min) * obj.Scale).Length();
                if (length < 10)
                    length = 10;
                camera.Zoom = length * 3.5f;
            }
            camera.IsOrthographic = true;
            CheckIsOrthographic();
            EaseCamera((Selected == null) ? camera.Position : Selected.Position, targetRotation);
        }
    }
}
