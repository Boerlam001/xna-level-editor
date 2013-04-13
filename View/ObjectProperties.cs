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
    public partial class ObjectProperties : UserControl, IObserver
    {
        BaseObject model;

        public BaseObject Model
        {
            get { return model; }
            set { model = value; }
        }

        public ObjectProperties()
        {
            InitializeComponent();
        }

        void IObserver.Update()
        {
            txt_name.Text = model.Name;

            txt_posX.Text = model.Position.X.ToString();
            txt_posY.Text = model.Position.Y.ToString();
            txt_posZ.Text = model.Position.Z.ToString();

            txt_rotX.Text = model.RotationX.ToString();
            txt_rotY.Text = model.RotationY.ToString();
            txt_rotZ.Text = model.RotationZ.ToString();

            txt_qW.Text = model.Rotation.W.ToString();
            txt_qX.Text = model.Rotation.X.ToString();
            txt_qY.Text = model.Rotation.Y.ToString();
            txt_qZ.Text = model.Rotation.Z.ToString();
        }

        private void txt_rot_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            if (e.KeyCode != Keys.Enter)
                return;

            float f;
            if (float.TryParse(txt_rotX.Text, out f))
                model.RotationX = f;
            txt_rotX.Text = model.RotationX.ToString();
            if (float.TryParse(txt_rotY.Text, out f))
                model.RotationY = f;
            txt_rotY.Text = model.RotationY.ToString();
            if (float.TryParse(txt_rotZ.Text, out f))
                model.RotationZ = f;
            txt_rotZ.Text = model.RotationZ.ToString();
            model.Notify();
        }

        private void txt_q_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            if (e.KeyCode != Keys.Enter)
                return;

            float f;
            Quaternion q = new Quaternion();

            if (float.TryParse(txt_qW.Text, out f))
                q.W = f;
            else
                q.W = model.Rotation.W;

            if (float.TryParse(txt_qX.Text, out f))
                q.X = f;
            else
                q.X = model.Rotation.X;

            if (float.TryParse(txt_qY.Text, out f))
                q.Y = f;
            else
                q.Y = model.Rotation.Y;

            if (float.TryParse(txt_qZ.Text, out f))
                q.Z = f;
            else
                q.Z = model.Rotation.Z;

            model.Rotation = q;

            txt_qW.Text = q.W.ToString();
            txt_qX.Text = q.X.ToString();
            txt_qY.Text = q.Y.ToString();
            txt_qZ.Text = q.Z.ToString();

            model.Notify();
        }

        private void txt_pos_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            if (e.KeyCode != Keys.Enter)
                return;

            float f;
            Vector3 p = model.Position;

            if (float.TryParse(txt_posX.Text, out f))
                p.X = f;
            else
                p.X = model.Position.X;

            if (float.TryParse(txt_posY.Text, out f))
                p.Y = f;
            else
                p.Y = model.Position.Y;

            if (float.TryParse(txt_posZ.Text, out f))
                p.Z = f;
            else
                p.Z = model.Position.Z;

            model.Position = p;

            txt_posX.Text = p.X.ToString();
            txt_posY.Text = p.Y.ToString();
            txt_posZ.Text = p.Z.ToString();

            model.Notify();
        }

    }
}
