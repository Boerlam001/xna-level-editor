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
            this.editor1 = new View.Editor();
            this.objectProperties1 = new View.ObjectProperties();
            this.objectProperties2 = new View.ObjectProperties();
            this.menuStrip = new System.Windows.Forms.MenuStrip();
            this.createFileStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.menuStrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // editor1
            // 
            this.editor1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.editor1.Location = new System.Drawing.Point(4, 27);
            this.editor1.MainUserControl = null;
            this.editor1.MapModel = null;
            this.editor1.Name = "editor1";
            this.editor1.Selected = null;
            this.editor1.Size = new System.Drawing.Size(658, 331);
            this.editor1.TabIndex = 0;
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
            // menuStrip
            // 
            this.menuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.createFileStripMenuItem});
            this.menuStrip.Location = new System.Drawing.Point(0, 0);
            this.menuStrip.Name = "menuStrip";
            this.menuStrip.Size = new System.Drawing.Size(674, 24);
            this.menuStrip.TabIndex = 3;
            this.menuStrip.Text = "menuStrip1";
            // 
            // createFileStripMenuItem
            // 
            this.createFileStripMenuItem.Name = "createFileStripMenuItem";
            this.createFileStripMenuItem.Size = new System.Drawing.Size(74, 20);
            this.createFileStripMenuItem.Text = "Create File";
            this.createFileStripMenuItem.Click += new System.EventHandler(this.createFileStripMenuItem_Click);
            // 
            // MainUserControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.objectProperties2);
            this.Controls.Add(this.objectProperties1);
            this.Controls.Add(this.editor1);
            this.Controls.Add(this.menuStrip);
            this.Name = "MainUserControl";
            this.Size = new System.Drawing.Size(674, 526);
            this.Load += new System.EventHandler(this.MainUserControl_Load);
            this.menuStrip.ResumeLayout(false);
            this.menuStrip.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private Editor editor1;
        private ObjectProperties objectProperties1;
        private ObjectProperties objectProperties2;
        private System.Windows.Forms.MenuStrip menuStrip;
        private System.Windows.Forms.ToolStripMenuItem createFileStripMenuItem;
    }
}
