using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using EditorModel;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

namespace View
{
    public abstract class EditorMode
    {
        protected Editor editor;

        public Editor Editor
        {
            get { return editor; }
            set { editor = value; }
        }

        protected static bool isMouseDown;

        public static bool IsMouseDown
        {
            get { return EditorMode.isMouseDown; }
            set { EditorMode.isMouseDown = value; }
        }

        protected static bool isRotate;

        public static bool IsRotate
        {
            get { return EditorMode.isRotate; }
            set { EditorMode.isRotate = value; }
        }

        protected static int mouseX;

        public static int MouseX
        {
            get { return EditorMode.mouseX; }
            set { EditorMode.mouseX = value; }
        }

        protected static int mouseY;

        public static int MouseY
        {
            get { return EditorMode.mouseY; }
            set { EditorMode.mouseY = value; }
        }

        protected string text;

        public string Text
        {
            get { return text; }
            set { text = value; }
        }

        protected static float diffX;
        protected static float diffY;

        public EditorMode()
        {
            isMouseDown = isRotate = false;
            mouseX = mouseY = 0;
            text = "";
        }

        public EditorMode(Editor editor)
        {
            isMouseDown = isRotate = false;
            mouseX = mouseY = 0;
            this.editor = editor;
            text = "";
        }

        public abstract void PreviewKeyDown(object sender, PreviewKeyDownEventArgs e);
        public abstract void KeyUp(object sender, KeyEventArgs e);

        public virtual void MouseDown(object sender, MouseEventArgs e)
        {
            isMouseDown = true;
            mouseX = e.X;
            mouseY = e.Y;
            if (e.Button == System.Windows.Forms.MouseButtons.Right)
            {
                isRotate = true;
            }
            if (e.Button == System.Windows.Forms.MouseButtons.Left)
            {
                //if (editor.TrueModel.Objects.Count == 0)
                //    return;
                //Ray ray = editor.Pick(e.X, e.Y);
                //DrawingObject obj = editor.TrueModel.Objects[0];
                //obj.Position = editor.Put(e.X, e.Y, 3);
                //obj.Notify();
            }
        }
        
        public virtual void MouseMove(object sender, MouseEventArgs e)
        {
            diffX = (float)(e.X - mouseX);
            diffY = (float)(e.Y - mouseY);

            mouseX = e.X;
            mouseY = e.Y;

            if (isRotate)
            {
                editor.Camera.Rotate(diffY / 10, -diffX / 10, 0);
                editor.Camera.Notify();
            }
        }

        public virtual void MouseUp(object sender, MouseEventArgs e)
        {
            isMouseDown = false;
            if (e.Button == System.Windows.Forms.MouseButtons.Right)
            {
                isRotate = false;
                editor.Camera.IsMoving = false;
                editor.Camera.Notify();
            }
        }
               
        public abstract void DragEnter(object sender, DragEventArgs e);
        public abstract void DragDrop(object sender, DragEventArgs e);
    }
}
