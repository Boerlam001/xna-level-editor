using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using EnvDTE80;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

namespace View
{
    public partial class MainUserControl : UserControl
    {
        private DTE2 applicationObject;

        public DTE2 ApplicationObject
        {
            get { return applicationObject; }
            set { applicationObject = value; }
        }

        ContentManager content;
        ContentBuilder contentBuilder;

        Model myModel;
        Vector3 modelPosition;
        float modelRotation;
        Vector3 cameraPosition;
        float cameraRotationY;
        float cameraRotationX;
        float fieldOfViewAngle;
        float nearPlaneDistance;
        float farPlaneDistance;
        bool mouseDown;
        int mouseTempX;
        int mouseTempY;
        BasicEffect basicEffect;
    
        public MainUserControl()
        {
            InitializeComponent();
            modelPosition = new Vector3(0, 0, 0);
            modelRotation = 0;
            cameraPosition = new Vector3(0, 5, 10);
            Vector3 cameraTarget = modelPosition;
            //Quaternion q = Quaternion.Identity;
            //Matrix rotationVector = Matrix.CreateLookAt(cameraPosition, modelPosition, Vector3.UnitY).Decompose(null, q, null);
            cameraRotationX = 0;
            cameraRotationY = 180;
            fieldOfViewAngle = MathHelper.ToRadians(45);
            nearPlaneDistance = 0.01f;
            farPlaneDistance = 100f;

            mouseDown = false;
        }

        private void graphicsDeviceControl1_Paint(object sender, PaintEventArgs e)
        {
            graphicsDeviceControl1.GraphicsDevice.Clear(Microsoft.Xna.Framework.Color.CornflowerBlue);
            if (myModel == null)
                return;
            
            Matrix world = Matrix.CreateTranslation(modelPosition) * Matrix.CreateRotationY(MathHelper.ToRadians(modelRotation));

            Matrix cameraRotationMatrix = Matrix.CreateRotationX(MathHelper.ToRadians(cameraRotationX)) * Matrix.CreateRotationY(MathHelper.ToRadians(cameraRotationY));
            Vector3 cameraTarget = cameraPosition + Vector3.Transform(cameraPosition, cameraRotationMatrix);
            Matrix view = Matrix.CreateLookAt(cameraPosition, cameraTarget, Vector3.UnitY);
            
            textBox1.Text =
                "cameraPosition: " + cameraPosition.X + " " + cameraPosition.Y + " " + cameraPosition.Z + "\r\n" +
                "cameraTarget: " + cameraTarget.X + " " + cameraTarget.Y + " " + cameraTarget.Z + "\r\n" +
                "cameraRotation: " + cameraRotationX + " " + cameraRotationY;

            float aspectRatio = (float)graphicsDeviceControl1.Width / graphicsDeviceControl1.Height;
            Matrix projection = Matrix.CreatePerspectiveFieldOfView(fieldOfViewAngle, aspectRatio, nearPlaneDistance, farPlaneDistance);

            foreach (ModelMesh mesh in myModel.Meshes)
            {
                foreach (BasicEffect effect in mesh.Effects)
                {
                    effect.World = world;
                    effect.View = view;
                    effect.Projection = projection;
                    effect.EnableDefaultLighting();
                    effect.PreferPerPixelLighting = true;
                    effect.SpecularPower = 16;
                }
                mesh.Draw();
            }

            basicEffect.View = view;
            basicEffect.Projection = projection;
            basicEffect.CurrentTechnique.Passes[0].Apply();
            for (int i = 0; i < 10; i++)
            {
                var vertices = new[] { new VertexPositionColor(new Vector3(i * 2 - 5, -1, -10), Microsoft.Xna.Framework.Color.White), new VertexPositionColor(new Vector3(i * 2 - 5, -1, 10), Microsoft.Xna.Framework.Color.White) };
                graphicsDeviceControl1.GraphicsDevice.DrawUserPrimitives(PrimitiveType.LineList, vertices, 0, 1);
                vertices = new[] { new VertexPositionColor(new Vector3(-10, -1, (i - 1) * 2 - 5), Microsoft.Xna.Framework.Color.White), new VertexPositionColor(new Vector3(10, -1, (i - 1) * 2 - 5), Microsoft.Xna.Framework.Color.White) };
                graphicsDeviceControl1.GraphicsDevice.DrawUserPrimitives(PrimitiveType.LineList, vertices, 0, 1);
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            graphicsDeviceControl1.Refresh();
        }

        private void UserControl1_Load(object sender, EventArgs e)
        {
            int x = 0;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            loadModel();
        }

        private void loadModel()
        {
            contentBuilder = new ContentBuilder();
            content = new ContentManager(graphicsDeviceControl1.Services, contentBuilder.OutputDirectory);
            if (openFileDialog1.ShowDialog() != DialogResult.OK)
            {
                return;
            }
            contentBuilder.Add(openFileDialog1.FileName, "Kenny", null, "ModelProcessor");
            string errorBuild = contentBuilder.Build();
            if (string.IsNullOrEmpty(errorBuild))
            {
                myModel = content.Load<Model>("Kenny");
                basicEffect = new BasicEffect(graphicsDeviceControl1.GraphicsDevice);
                timer1.Enabled = true;
            }
        }

        private void splitter1_SplitterMoved(object sender, SplitterEventArgs e)
        {
            MessageBox.Show(graphicsDeviceControl1.Width.ToString());
        }

        private void toolStripButton6_Click(object sender, EventArgs e)
        {
            loadModel();
        }
                
        private void graphicsDeviceControl1_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button != System.Windows.Forms.MouseButtons.Right)
                return;
            mouseDown = true;
            mouseTempX = e.X;
            mouseTempY = e.Y;
        }

        private void graphicsDeviceControl1_MouseMove(object sender, MouseEventArgs e)
        {
            if (!mouseDown)
                return;
            float diffX = (float)(e.X - mouseTempX) / 10;
            float diffY = (float)(e.Y - mouseTempY) / 10;
            cameraRotationX = (cameraRotationX + diffY) % 360;
            if (cameraRotationX < 0)
                cameraRotationX = 360 - cameraRotationX;
            if (cameraRotationX > 90 && cameraRotationX < 270)
                cameraRotationX = (cameraRotationX + 90) % 360;
            cameraRotationY = (cameraRotationY - diffX) % 360;
            //cameraPosition.X += diffX;
            //cameraPosition.Y -= diffY;
            //cameraTarget.X += diffX;
            //cameraTarget.Y -= diffY;
            mouseTempX = e.X;
            mouseTempY = e.Y;
        }

        private void graphicsDeviceControl1_MouseUp(object sender, MouseEventArgs e)
        {
            mouseDown = false;
        }
    }
}
