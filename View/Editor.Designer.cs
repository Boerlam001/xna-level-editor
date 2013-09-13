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
            this.toolStrip = new System.Windows.Forms.ToolStrip();
            this.selectModeToolStripButton = new System.Windows.Forms.ToolStripButton();
            this.translateModeToolStripButton = new System.Windows.Forms.ToolStripButton();
            this.rotateModeToolStripButton = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.terrainIncreaseToolStripButton = new System.Windows.Forms.ToolStripButton();
            this.terrainDecreaseToolStripButton = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.graphicsDeviceControl1 = new View.GraphicsDeviceControl();
            this.toolStrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // openFileDialog1
            // 
            this.openFileDialog1.FileName = "openFileDialog1";
            // 
            // timer1
            // 
            this.timer1.Interval = 20;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // toolStrip
            // 
            this.toolStrip.Dock = System.Windows.Forms.DockStyle.Left;
            this.toolStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.selectModeToolStripButton,
            this.translateModeToolStripButton,
            this.rotateModeToolStripButton,
            this.toolStripSeparator1,
            this.terrainIncreaseToolStripButton,
            this.terrainDecreaseToolStripButton,
            this.toolStripSeparator2});
            this.toolStrip.Location = new System.Drawing.Point(0, 0);
            this.toolStrip.Name = "toolStrip";
            this.toolStrip.Size = new System.Drawing.Size(24, 342);
            this.toolStrip.TabIndex = 1;
            this.toolStrip.Text = "toolStrip1";
            // 
            // selectModeToolStripButton
            // 
            this.selectModeToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.selectModeToolStripButton.Image = ((System.Drawing.Image)(resources.GetObject("selectModeToolStripButton.Image")));
            this.selectModeToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.selectModeToolStripButton.Name = "selectModeToolStripButton";
            this.selectModeToolStripButton.Size = new System.Drawing.Size(21, 20);
            this.selectModeToolStripButton.Text = "toolStripButton1";
            this.selectModeToolStripButton.Click += new System.EventHandler(this.selectModeStripButton_Click);
            // 
            // translateModeToolStripButton
            // 
            this.translateModeToolStripButton.Checked = true;
            this.translateModeToolStripButton.CheckState = System.Windows.Forms.CheckState.Checked;
            this.translateModeToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.translateModeToolStripButton.Image = ((System.Drawing.Image)(resources.GetObject("translateModeToolStripButton.Image")));
            this.translateModeToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.translateModeToolStripButton.Name = "translateModeToolStripButton";
            this.translateModeToolStripButton.Size = new System.Drawing.Size(21, 20);
            this.translateModeToolStripButton.Text = "translateModeToolStripButton";
            this.translateModeToolStripButton.Click += new System.EventHandler(this.translateModeToolStripButton_Click);
            // 
            // rotateModeToolStripButton
            // 
            this.rotateModeToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.rotateModeToolStripButton.Image = ((System.Drawing.Image)(resources.GetObject("rotateModeToolStripButton.Image")));
            this.rotateModeToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.rotateModeToolStripButton.Name = "rotateModeToolStripButton";
            this.rotateModeToolStripButton.Size = new System.Drawing.Size(21, 20);
            this.rotateModeToolStripButton.Text = "rotateModeToolStripButton";
            this.rotateModeToolStripButton.Click += new System.EventHandler(this.rotateModeToolStripButton_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(21, 6);
            // 
            // terrainIncreaseToolStripButton
            // 
            this.terrainIncreaseToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.terrainIncreaseToolStripButton.Image = ((System.Drawing.Image)(resources.GetObject("terrainIncreaseToolStripButton.Image")));
            this.terrainIncreaseToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.terrainIncreaseToolStripButton.Name = "terrainIncreaseToolStripButton";
            this.terrainIncreaseToolStripButton.Size = new System.Drawing.Size(21, 20);
            this.terrainIncreaseToolStripButton.Text = "toolStripButton4";
            this.terrainIncreaseToolStripButton.Click += new System.EventHandler(this.terrainIncreaseToolStripButton_Click);
            // 
            // terrainDecreaseToolStripButton
            // 
            this.terrainDecreaseToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.terrainDecreaseToolStripButton.Image = ((System.Drawing.Image)(resources.GetObject("terrainDecreaseToolStripButton.Image")));
            this.terrainDecreaseToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.terrainDecreaseToolStripButton.Name = "terrainDecreaseToolStripButton";
            this.terrainDecreaseToolStripButton.Size = new System.Drawing.Size(21, 20);
            this.terrainDecreaseToolStripButton.Text = "toolStripButton5";
            this.terrainDecreaseToolStripButton.Click += new System.EventHandler(this.terrainDecreaseToolStripButton_Click);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(21, 6);
            // 
            // graphicsDeviceControl1
            // 
            this.graphicsDeviceControl1.AllowDrop = true;
            this.graphicsDeviceControl1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.graphicsDeviceControl1.Location = new System.Drawing.Point(27, 0);
            this.graphicsDeviceControl1.Name = "graphicsDeviceControl1";
            this.graphicsDeviceControl1.Size = new System.Drawing.Size(355, 342);
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
            // Editor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.toolStrip);
            this.Controls.Add(this.graphicsDeviceControl1);
            this.Name = "Editor";
            this.Size = new System.Drawing.Size(382, 342);
            this.Load += new System.EventHandler(this.Editor_Load);
            this.toolStrip.ResumeLayout(false);
            this.toolStrip.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private GraphicsDeviceControl graphicsDeviceControl1;
        private System.Windows.Forms.ToolStrip toolStrip;
        private System.Windows.Forms.ToolStripButton selectModeToolStripButton;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripButton terrainIncreaseToolStripButton;
        private System.Windows.Forms.ToolStripButton terrainDecreaseToolStripButton;
        public System.Windows.Forms.OpenFileDialog openFileDialog1;
        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripButton translateModeToolStripButton;
        private System.Windows.Forms.ToolStripButton rotateModeToolStripButton;
    }
}
