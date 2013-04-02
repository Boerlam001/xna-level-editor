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
using EditorModel;

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

        //Model myModel;
        DrawingObject ball;
        //Vector3 modelPosition;

        bool mouseDown;
        int mouseTempX;
        int mouseTempY;
        BasicEffect basicEffect;

        bool moveForward;
        bool moveBackward;
        bool moveRight;
        bool moveLeft;

        Camera camera;
    
        public MainUserControl()
        {
            InitializeComponent();

            mouseDown    = false;
            moveForward  = false;
            moveBackward = false;
            moveRight    = false;
            moveLeft     = false;

            camera = new Camera();
            camera.Position = new Vector3(0, 0, -10);
            camera.AspectRatio = (float)graphicsDeviceControl1.Width / graphicsDeviceControl1.Height;

            ball = new DrawingObject();
            ball.RotationChanged += ball_RotationChanged;
        }

        private void graphicsDeviceControl1_Paint(object sender, PaintEventArgs e)
        {
            graphicsDeviceControl1.GraphicsDevice.Clear(Microsoft.Xna.Framework.Color.Blue);

            ball.Draw(camera.World, camera.Projection);

            basicEffect.View = camera.World;
            basicEffect.Projection = camera.Projection;
            basicEffect.CurrentTechnique.Passes[0].Apply();
            VertexPositionColor[] vertices;
            for (int i = 0; i < 10; i++)
            {
                vertices = new[] { new VertexPositionColor(new Vector3(i * 2 - 5, -1, -10), Microsoft.Xna.Framework.Color.White), new VertexPositionColor(new Vector3(i * 2 - 5, -1, 10), Microsoft.Xna.Framework.Color.White) };
                graphicsDeviceControl1.GraphicsDevice.DrawUserPrimitives(PrimitiveType.LineList, vertices, 0, 1);
                vertices = new[] {
                    new VertexPositionColor(new Vector3(-10, -1, (i - 1) * 2 - 5), Microsoft.Xna.Framework.Color.White),
                    new VertexPositionColor(new Vector3(10, -1, (i - 1) * 2 - 5), Microsoft.Xna.Framework.Color.White)
                };
                graphicsDeviceControl1.GraphicsDevice.DrawUserPrimitives(PrimitiveType.LineList, vertices, 0, 1);
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            Vector3 position = camera.Position;
            if (moveRight)
                position.X += 1;
            if (moveLeft)
                position.X -= 1;
            if (moveForward)
                position.Z += 1;
            if (moveBackward)
                position.Z -= 1;
            if (moveRight || moveLeft || moveForward || moveBackward)
                camera.Position = position;
            graphicsDeviceControl1.Invalidate();
        }

        private void UserControl1_Load(object sender, EventArgs e)
        {
            contentBuilder = new ContentBuilder();
            content = new ContentManager(graphicsDeviceControl1.Services, contentBuilder.OutputDirectory);
            basicEffect = new BasicEffect(graphicsDeviceControl1.GraphicsDevice);
            graphicsDeviceControl1.KeyUp += graphicsDeviceControl1_KeyUp;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            loadModel();
        }

        private void loadModel()
        {
            if (openFileDialog1.ShowDialog() != DialogResult.OK)
            {
                return;
            }
            contentBuilder.Add(openFileDialog1.FileName, openFileDialog1.SafeFileName, null, "ModelProcessor");
            string errorBuild = contentBuilder.Build();
            if (string.IsNullOrEmpty(errorBuild))
            {
                ball.DrawingModel = content.Load<Model>(openFileDialog1.SafeFileName);
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
            float diffX = (float)(e.X - mouseTempX);
            float diffY = (float)(e.Y - mouseTempY);

            camera.Rotate(diffY / 10, -diffX / 10, 0);

            //camera.RotationX += diffY;
            //camera.RotationY += diffX;
            
            //if (!float.IsNaN(diffY))
            //    camera.RotationX += diffY;
            //if (!float.IsNaN(diffX))
            //    camera.RotationY -= diffX;
            
            mouseTempX = e.X;
            mouseTempY = e.Y;
        }

        private void graphicsDeviceControl1_MouseUp(object sender, MouseEventArgs e)
        {
            mouseDown = false;
        }

        private void graphicsDeviceControl1_KeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            if (e.KeyCode == Keys.W)
                moveForward = true;
            if (e.KeyCode == Keys.S)
                moveBackward = true;
            if (e.KeyCode == Keys.A)
                moveLeft = true;
            if (e.KeyCode == Keys.D)
                moveRight = true;
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
        }

        private void txt_rot_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            if (e.KeyCode != Keys.Enter)
                return;
            if (ball.DrawingModel == null)
                return;
            SetBallRotation();
        }

        private void SetBallRotation()
        {
            float f;
            if (float.TryParse(txt_rotX.Text, out f))
                ball.RotationX = f;
            txt_rotX.Text = ball.RotationX.ToString();
            if (float.TryParse(txt_rotY.Text, out f))
                ball.RotationY = f;
            txt_rotY.Text = ball.RotationY.ToString();
            if (float.TryParse(txt_rotZ.Text, out f))
                ball.RotationZ = f;
            txt_rotZ.Text = ball.RotationZ.ToString();
        }

        private void ball_RotationChanged(object sender, EventArgs e)
        {
            Vector3 s, t;
            Quaternion q;
            ball.World.Decompose(out s, out q, out t);
            txt_qW.Text = q.W.ToString();
            txt_qX.Text = q.X.ToString();
            txt_qY.Text = q.Y.ToString();
            txt_qZ.Text = q.Z.ToString();
            float rX, rY, rZ;
            Helper.QuaternionToEuler(q, out rX, out rY, out rZ);
            txt_rotXFromQ.Text = Math.Round(MathHelper.ToDegrees(rX), 4).ToString();
            txt_rotYFromQ.Text = Math.Round(MathHelper.ToDegrees(rY), 4).ToString();
            txt_rotZFromQ.Text = Math.Round(MathHelper.ToDegrees(rZ), 4).ToString();
        }

        private void txt_q_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            if (e.KeyCode != Keys.Enter)
                return;
            if (ball.DrawingModel == null)
                return;
            SetBallQuaternion();
        }

        private void SetBallQuaternion()
        {
            float f;
            Quaternion q = new Quaternion();
            
            if (float.TryParse(txt_qW.Text, out f))
                q.W = f;
            else
                q.W = ball.Rotation.W;
            
            if (float.TryParse(txt_qX.Text, out f))
                q.X = f;
            else
                q.X = ball.Rotation.X;
            
            if (float.TryParse(txt_qY.Text, out f))
                q.Y = f;
            else
                q.Y = ball.Rotation.Y;
            
            if (float.TryParse(txt_qZ.Text, out f))
                q.Z = f;
            else
                q.Z = ball.Rotation.Z;
            
            ball.Rotation = q;

            txt_qW.Text = q.W.ToString();
            txt_qX.Text = q.X.ToString();
            txt_qY.Text = q.Y.ToString();
            txt_qZ.Text = q.Z.ToString();
        }
    }
}
