namespace View
{
    partial class Editor
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Editor));
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.graphicsDeviceControl1 = new View.GraphicsDeviceControl();
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.toolStripButton1 = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripButton4 = new System.Windows.Forms.ToolStripButton();
            this.toolStripButton5 = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.translateModeToolStripButton = new System.Windows.Forms.ToolStripButton();
            this.rotateModeToolStripButton = new System.Windows.Forms.ToolStripButton();
            this.graphicsDeviceControl1.SuspendLayout();
            this.toolStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // openFileDialog1
            // 
            this.openFileDialog1.FileName = "openFileDialog1";
            // 
            // timer1
            // 
            this.timer1.Interval = 30;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // graphicsDeviceControl1
            // 
            this.graphicsDeviceControl1.AllowDrop = true;
            this.graphicsDeviceControl1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.graphicsDeviceControl1.Controls.Add(this.toolStrip1);
            this.graphicsDeviceControl1.Location = new System.Drawing.Point(3, 3);
            this.graphicsDeviceControl1.Name = "graphicsDeviceControl1";
            this.graphicsDeviceControl1.Size = new System.Drawing.Size(373, 336);
            this.graphicsDeviceControl1.TabIndex = 0;
            this.graphicsDeviceControl1.TabStop = true;
            this.graphicsDeviceControl1.KeyUp += new System.Windows.Forms.KeyEventHandler(this.graphicsDeviceControl1_KeyUp);
            this.graphicsDeviceControl1.DragDrop += new System.Windows.Forms.DragEventHandler(this.graphicsDeviceControl1_DragDrop);
            this.graphicsDeviceControl1.DragEnter += new System.Windows.Forms.DragEventHandler(this.graphicsDeviceControl1_DragEnter);
            this.graphicsDeviceControl1.Paint += new System.Windows.Forms.PaintEventHandler(this.graphicsDeviceControl1_Paint);
            this.graphicsDeviceControl1.MouseDown += new System.Windows.Forms.MouseEventHandler(this.graphicsDeviceControl1_MouseDown);
            this.graphicsDeviceControl1.MouseMove += new System.Windows.Forms.MouseEventHandler(this.graphicsDeviceControl1_MouseMove);
            this.graphicsDeviceControl1.MouseUp += new System.Windows.Forms.MouseEventHandler(this.graphicsDeviceControl1_MouseUp);
            this.graphicsDeviceControl1.PreviewKeyDown += new System.Windows.Forms.PreviewKeyDownEventHandler(this.graphicsDeviceControl1_PreviewKeyDown);
            this.graphicsDeviceControl1.Resize += new System.EventHandler(this.graphicsDeviceControl1_Resize);
            // 
            // toolStrip1
            // 
            this.toolStrip1.Dock = System.Windows.Forms.DockStyle.Left;
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripButton1,
            this.translateModeToolStripButton,
            this.rotateModeToolStripButton,
            this.toolStripSeparator1,
            this.toolStripButton4,
            this.toolStripButton5,
            this.toolStripSeparator2});
            this.toolStrip1.Location = new System.Drawing.Point(0, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(32, 336);
            this.toolStrip1.TabIndex = 1;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // toolStripButton1
            // 
            this.toolStripButton1.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButton1.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButton1.Image")));
            this.toolStripButton1.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButton1.Name = "toolStripButton1";
            this.toolStripButton1.Size = new System.Drawing.Size(29, 20);
            this.toolStripButton1.Text = "toolStripButton1";
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(29, 6);
            // 
            // toolStripButton4
            // 
            this.toolStripButton4.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButton4.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButton4.Image")));
            this.toolStripButton4.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButton4.Name = "toolStripButton4";
            this.toolStripButton4.Size = new System.Drawing.Size(29, 20);
            this.toolStripButton4.Text = "toolStripButton4";
            // 
            // toolStripButton5
            // 
            this.toolStripButton5.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButton5.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButton5.Image")));
            this.toolStripButton5.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButton5.Name = "toolStripButton5";
            this.toolStripButton5.Size = new System.Drawing.Size(29, 20);
            this.toolStripButton5.Text = "toolStripButton5";
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(29, 6);
            // 
            // translateModeToolStripButton
            // 
            this.translateModeToolStripButton.Checked = true;
            this.translateModeToolStripButton.CheckState = System.Windows.Forms.CheckState.Checked;
            this.translateModeToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.translateModeToolStripButton.Image = ((System.Drawing.Image)(resources.GetObject("translateModeToolStripButton.Image")));
            this.translateModeToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.translateModeToolStripButton.Name = "translateModeToolStripButton";
            this.translateModeToolStripButton.Size = new System.Drawing.Size(29, 20);
            this.translateModeToolStripButton.Text = "translateModeToolStripButton";
            this.translateModeToolStripButton.Click += new System.EventHandler(this.translateModeToolStripButton_Click);
            // 
            // rotateModeToolStripButton
            // 
            this.rotateModeToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.rotateModeToolStripButton.Image = ((System.Drawing.Image)(resources.GetObject("rotateModeToolStripButton.Image")));
            this.rotateModeToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.rotateModeToolStripButton.Name = "rotateModeToolStripButton";
            this.rotateModeToolStripButton.Size = new System.Drawing.Size(29, 20);
            this.rotateModeToolStripButton.Text = "rotateModeToolStripButton";
            this.rotateModeToolStripButton.Click += new System.EventHandler(this.rotateModeToolStripButton_Click);
            // 
            // Editor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.graphicsDeviceControl1);
            this.Name = "Editor";
            this.Size = new System.Drawing.Size(382, 342);
            this.Load += new System.EventHandler(this.Editor_Load);
            this.Resize += new System.EventHandler(this.Editor_Resize);
            this.graphicsDeviceControl1.ResumeLayout(false);
            this.graphicsDeviceControl1.PerformLayout();
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private GraphicsDeviceControl graphicsDeviceControl1;
        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripButton toolStripButton1;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripButton toolStripButton4;
        private System.Windows.Forms.ToolStripButton toolStripButton5;
        private System.Windows.Forms.OpenFileDialog openFileDialog1;
        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripButton translateModeToolStripButton;
        private System.Windows.Forms.ToolStripButton rotateModeToolStripButton;
    }
}
