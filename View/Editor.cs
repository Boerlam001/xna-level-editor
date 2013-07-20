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
        TrueModel trueModel;
        
        public TrueModel TrueModel
        {
            get { return trueModel; }
            set { trueModel = value; }
        }

        MainUserControl mainUserControl;

        public MainUserControl MainUserControl
        {
            get { return mainUserControl; }
            set
            {
                mainUserControl = value;
            }
        }

        Camera camera;

        public Camera Camera
        {
            get { return camera; }
            set { camera = value; }
        }

        ContentManager contentManager;

        ContentBuilder contentBuilder;

        public ContentBuilder ContentBuilder
        {
            get { return contentBuilder; }
            set { contentBuilder = value; }
        }

        BasicEffect basicEffect;

        private bool moveForward;
        private bool moveBackward;
        private bool moveLeft;
        private bool moveRight;

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
            set { selectedBoundingBox = value; }
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
            set { spriteFont = value; }
        }

        public Editor()
        {
            InitializeComponent();
        }

        void IObserver.Update()
        {
            graphicsDeviceControl1.Invalidate();
        }

        public Model LoadModel()
        {
            if (openFileDialog1.ShowDialog() != DialogResult.OK)
            {
                return null;
            }
            return OpenModel(openFileDialog1.FileName, openFileDialog1.SafeFileName);
        }

        public Model OpenModel(string path, string name)
        {
            //contentManager.Unload();
            //contentBuilder.Clear();
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

        private void Editor_Load(object sender, EventArgs e)
        {
            basicEffect = new BasicEffect(graphicsDeviceControl1.GraphicsDevice);

            contentBuilder = new ContentBuilder();
            contentManager = new ContentManager(graphicsDeviceControl1.Services, contentBuilder.OutputDirectory);

            //importer reference: http://msdn.microsoft.com/en-us/library/bb447762%28v=xnagamestudio.20%29.aspx
            contentBuilder.Add("D:\\SegoeUI.spritefont", "SegoeUI.spritefont", null, "FontDescriptionProcessor");
            string errorBuild = contentBuilder.Build();

            spriteBatch = new SpriteBatch(graphicsDeviceControl1.GraphicsDevice);
            try
            {
                spriteFont = contentManager.Load<SpriteFont>("SegoeUI.spritefont");
            }
            catch (Exception ex)
            {
            }

            camera = new Camera();
            camera.Position = new Vector3(-4, 8, -25);
            camera.AspectRatio = graphicsDeviceControl1.GraphicsDevice.Viewport.AspectRatio;
            camera.Rotate(20, 55, 0);
            camera.Attach(this);

            selected = null;
            selectedBoundingBox = new ModelBoundingBox(graphicsDeviceControl1.GraphicsDevice);
            selectedBoundingBox.SpriteBatch = spriteBatch;
            selectedBoundingBox.SpriteFont = spriteFont;
            selectedBoundingBox.Camera = camera;

            editorMode = new EditorMode_Select(this);

            graphicsDeviceControl1.Invalidate();
        }

        Terrain terrain = new Terrain();
        private string text;
        
        private void graphicsDeviceControl1_Paint(object sender, PaintEventArgs e)
        {
            graphicsDeviceControl1.GraphicsDevice.Clear(Microsoft.Xna.Framework.Color.DarkGray);

            if (trueModel != null)
                foreach (DrawingObject obj in trueModel.Objects)
                    obj.Draw(camera.World, camera.Projection);
            
            basicEffect.View = camera.World;
            basicEffect.Projection = camera.Projection;

            //DrawGrid(basicEffect);
            terrain.Draw(basicEffect, graphicsDeviceControl1.GraphicsDevice);

            if (selected != null)
                selectedBoundingBox.Draw(basicEffect);

            //try
            //{
            //    spriteBatch.Begin();
            //    spriteBatch.DrawString(spriteFont, text, new Vector2(50, 10), Microsoft.Xna.Framework.Color.Black);
            //    spriteBatch.End();
            //}
            //catch (Exception ex)
            //{
            //}
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
            text = e.X + " " + e.Y;
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
    }
}
