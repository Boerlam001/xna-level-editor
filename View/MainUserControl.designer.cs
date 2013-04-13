namespace View
{
    partial class MainUserControl
    {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            EditorModel.Camera camera1 = new EditorModel.Camera();
            this.editor1 = new View.Editor();
            this.objectProperties1 = new View.ObjectProperties();
            this.objectProperties2 = new View.ObjectProperties();
            this.SuspendLayout();
            // 
            // editor1
            // 
            this.editor1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            camera1.AspectRatio = 1.864943F;
            camera1.FarPlaneDistance = 100F;
            camera1.FieldOfViewAngle = 0.7853982F;
            camera1.Name = "camera";
            camera1.NearPlaneDistance = 0.01F;
            camera1.Position = new Microsoft.Xna.Framework.Vector3(0F, 0F, -10F);
            camera1.Rotation = new Microsoft.Xna.Framework.Quaternion(0F, 0F, 0F, 1F);
            camera1.RotationX = 0F;
            camera1.RotationY = 0F;
            camera1.RotationZ = 0F;
            this.editor1.Camera = camera1;
            this.editor1.Location = new System.Drawing.Point(4, 4);
            this.editor1.MainUserControl = null;
            this.editor1.Name = "editor1";
            this.editor1.Selected = null;
            this.editor1.Size = new System.Drawing.Size(658, 354);
            this.editor1.TabIndex = 0;
            this.editor1.TrueModel = null;
            // 
            // objectProperties1
            // 
            this.objectProperties1.Location = new System.Drawing.Point(0, 364);
            this.objectProperties1.Model = null;
            this.objectProperties1.Name = "objectProperties1";
            this.objectProperties1.Size = new System.Drawing.Size(328, 148);
            this.objectProperties1.TabIndex = 1;
            // 
            // objectProperties2
            // 
            this.objectProperties2.Location = new System.Drawing.Point(334, 364);
            this.objectProperties2.Model = null;
            this.objectProperties2.Name = "objectProperties2";
            this.objectProperties2.Size = new System.Drawing.Size(328, 148);
            this.objectProperties2.TabIndex = 2;
            // 
            // MainUserControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.objectProperties2);
            this.Controls.Add(this.objectProperties1);
            this.Controls.Add(this.editor1);
            this.Name = "MainUserControl";
            this.Size = new System.Drawing.Size(674, 526);
            this.Load += new System.EventHandler(this.MainUserControl_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private Editor editor1;
        private ObjectProperties objectProperties1;
        private ObjectProperties objectProperties2;
    }
}
