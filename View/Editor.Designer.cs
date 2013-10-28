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
            this.scaleModeToolStripButton = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.terrainIncreaseToolStripButton = new System.Windows.Forms.ToolStripButton();
            this.terrainDecreaseToolStripButton = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.addRoadStripButton = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.orthogonalStripButton = new System.Windows.Forms.ToolStripButton();
            this.miniToolStrip = new System.Windows.Forms.ToolStrip();
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.toolStripContainer1 = new System.Windows.Forms.ToolStripContainer();
            this.physicsWorldToolStrip = new System.Windows.Forms.ToolStrip();
            this.gravityStripLabel = new System.Windows.Forms.ToolStripLabel();
            this.gravityToolStripTextBox = new System.Windows.Forms.ToolStripTextBox();
            this.materialCoefficientMixingtoolStripLabel = new System.Windows.Forms.ToolStripLabel();
            this.materialCoefficientMixingToolStripComboBox = new System.Windows.Forms.ToolStripComboBox();
            this.graphicsDeviceControl1 = new View.GraphicsDeviceControl();
            this.toolStrip.SuspendLayout();
            this.toolStripContainer1.BottomToolStripPanel.SuspendLayout();
            this.toolStripContainer1.ContentPanel.SuspendLayout();
            this.toolStripContainer1.TopToolStripPanel.SuspendLayout();
            this.toolStripContainer1.SuspendLayout();
            this.physicsWorldToolStrip.SuspendLayout();
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
            this.toolStrip.Dock = System.Windows.Forms.DockStyle.None;
            this.toolStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.selectModeToolStripButton,
            this.translateModeToolStripButton,
            this.rotateModeToolStripButton,
            this.scaleModeToolStripButton,
            this.toolStripSeparator1,
            this.terrainIncreaseToolStripButton,
            this.terrainDecreaseToolStripButton,
            this.toolStripSeparator2,
            this.addRoadStripButton,
            this.toolStripSeparator3,
            this.orthogonalStripButton});
            this.toolStrip.Location = new System.Drawing.Point(3, 0);
            this.toolStrip.Name = "toolStrip";
            this.toolStrip.Size = new System.Drawing.Size(185, 25);
            this.toolStrip.TabIndex = 1;
            this.toolStrip.Text = "toolStrip1";
            // 
            // selectModeToolStripButton
            // 
            this.selectModeToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.selectModeToolStripButton.Image = ((System.Drawing.Image)(resources.GetObject("selectModeToolStripButton.Image")));
            this.selectModeToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.selectModeToolStripButton.Name = "selectModeToolStripButton";
            this.selectModeToolStripButton.Size = new System.Drawing.Size(23, 22);
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
            this.translateModeToolStripButton.Size = new System.Drawing.Size(23, 22);
            this.translateModeToolStripButton.Text = "translateModeToolStripButton";
            this.translateModeToolStripButton.Click += new System.EventHandler(this.translateModeToolStripButton_Click);
            // 
            // rotateModeToolStripButton
            // 
            this.rotateModeToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.rotateModeToolStripButton.Image = ((System.Drawing.Image)(resources.GetObject("rotateModeToolStripButton.Image")));
            this.rotateModeToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.rotateModeToolStripButton.Name = "rotateModeToolStripButton";
            this.rotateModeToolStripButton.Size = new System.Drawing.Size(23, 22);
            this.rotateModeToolStripButton.Text = "rotateModeToolStripButton";
            this.rotateModeToolStripButton.Click += new System.EventHandler(this.rotateModeToolStripButton_Click);
            // 
            // scaleModeToolStripButton
            // 
            this.scaleModeToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.scaleModeToolStripButton.Image = ((System.Drawing.Image)(resources.GetObject("scaleModeToolStripButton.Image")));
            this.scaleModeToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.scaleModeToolStripButton.Name = "scaleModeToolStripButton";
            this.scaleModeToolStripButton.Size = new System.Drawing.Size(23, 22);
            this.scaleModeToolStripButton.Text = "Scale";
            this.scaleModeToolStripButton.Click += new System.EventHandler(this.scaleModeToolStripButton_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(6, 25);
            // 
            // terrainIncreaseToolStripButton
            // 
            this.terrainIncreaseToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.terrainIncreaseToolStripButton.Image = ((System.Drawing.Image)(resources.GetObject("terrainIncreaseToolStripButton.Image")));
            this.terrainIncreaseToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.terrainIncreaseToolStripButton.Name = "terrainIncreaseToolStripButton";
            this.terrainIncreaseToolStripButton.Size = new System.Drawing.Size(23, 22);
            this.terrainIncreaseToolStripButton.Text = "toolStripButton4";
            this.terrainIncreaseToolStripButton.Click += new System.EventHandler(this.terrainIncreaseToolStripButton_Click);
            // 
            // terrainDecreaseToolStripButton
            // 
            this.terrainDecreaseToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.terrainDecreaseToolStripButton.Image = ((System.Drawing.Image)(resources.GetObject("terrainDecreaseToolStripButton.Image")));
            this.terrainDecreaseToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.terrainDecreaseToolStripButton.Name = "terrainDecreaseToolStripButton";
            this.terrainDecreaseToolStripButton.Size = new System.Drawing.Size(23, 22);
            this.terrainDecreaseToolStripButton.Text = "toolStripButton5";
            this.terrainDecreaseToolStripButton.Click += new System.EventHandler(this.terrainDecreaseToolStripButton_Click);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(6, 25);
            // 
            // addRoadStripButton
            // 
            this.addRoadStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.addRoadStripButton.Image = ((System.Drawing.Image)(resources.GetObject("addRoadStripButton.Image")));
            this.addRoadStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.addRoadStripButton.Name = "addRoadStripButton";
            this.addRoadStripButton.Size = new System.Drawing.Size(23, 22);
            this.addRoadStripButton.Text = "toolStripButton1";
            this.addRoadStripButton.Click += new System.EventHandler(this.gridStripButton_Click);
            // 
            // toolStripSeparator3
            // 
            this.toolStripSeparator3.Name = "toolStripSeparator3";
            this.toolStripSeparator3.Size = new System.Drawing.Size(6, 25);
            this.toolStripSeparator3.Visible = false;
            // 
            // orthogonalStripButton
            // 
            this.orthogonalStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.orthogonalStripButton.Image = ((System.Drawing.Image)(resources.GetObject("orthogonalStripButton.Image")));
            this.orthogonalStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.orthogonalStripButton.Name = "orthogonalStripButton";
            this.orthogonalStripButton.Size = new System.Drawing.Size(23, 22);
            this.orthogonalStripButton.Text = "orthogonalStripButton";
            this.orthogonalStripButton.Visible = false;
            this.orthogonalStripButton.Click += new System.EventHandler(this.orthogonalStripButton_Click);
            // 
            // miniToolStrip
            // 
            this.miniToolStrip.AutoSize = false;
            this.miniToolStrip.CanOverflow = false;
            this.miniToolStrip.Dock = System.Windows.Forms.DockStyle.None;
            this.miniToolStrip.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.miniToolStrip.Location = new System.Drawing.Point(9, 3);
            this.miniToolStrip.Name = "miniToolStrip";
            this.miniToolStrip.Size = new System.Drawing.Size(355, 25);
            this.miniToolStrip.TabIndex = 0;
            // 
            // toolStrip1
            // 
            this.toolStrip1.Location = new System.Drawing.Point(0, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(355, 25);
            this.toolStrip1.TabIndex = 0;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // toolStripContainer1
            // 
            // 
            // toolStripContainer1.BottomToolStripPanel
            // 
            this.toolStripContainer1.BottomToolStripPanel.Controls.Add(this.toolStrip);
            // 
            // toolStripContainer1.ContentPanel
            // 
            this.toolStripContainer1.ContentPanel.Controls.Add(this.graphicsDeviceControl1);
            this.toolStripContainer1.ContentPanel.Size = new System.Drawing.Size(525, 292);
            this.toolStripContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.toolStripContainer1.Location = new System.Drawing.Point(0, 0);
            this.toolStripContainer1.Name = "toolStripContainer1";
            this.toolStripContainer1.Size = new System.Drawing.Size(525, 342);
            this.toolStripContainer1.TabIndex = 2;
            this.toolStripContainer1.Text = "toolStripContainer1";
            // 
            // toolStripContainer1.TopToolStripPanel
            // 
            this.toolStripContainer1.TopToolStripPanel.Controls.Add(this.physicsWorldToolStrip);
            // 
            // physicsWorldToolStrip
            // 
            this.physicsWorldToolStrip.Dock = System.Windows.Forms.DockStyle.None;
            this.physicsWorldToolStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.gravityStripLabel,
            this.gravityToolStripTextBox,
            this.materialCoefficientMixingtoolStripLabel,
            this.materialCoefficientMixingToolStripComboBox});
            this.physicsWorldToolStrip.Location = new System.Drawing.Point(3, 0);
            this.physicsWorldToolStrip.Name = "physicsWorldToolStrip";
            this.physicsWorldToolStrip.Size = new System.Drawing.Size(380, 25);
            this.physicsWorldToolStrip.TabIndex = 2;
            // 
            // gravityStripLabel
            // 
            this.gravityStripLabel.Name = "gravityStripLabel";
            this.gravityStripLabel.Size = new System.Drawing.Size(44, 22);
            this.gravityStripLabel.Text = "Gravity";
            // 
            // gravityToolStripTextBox
            // 
            this.gravityToolStripTextBox.Name = "gravityToolStripTextBox";
            this.gravityToolStripTextBox.Size = new System.Drawing.Size(40, 25);
            this.gravityToolStripTextBox.Text = "-30";
            this.gravityToolStripTextBox.TextBoxTextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.gravityToolStripTextBox.TextChanged += new System.EventHandler(this.gravityToolStripTextBox_TextChanged);
            // 
            // materialCoefficientMixingtoolStripLabel
            // 
            this.materialCoefficientMixingtoolStripLabel.Name = "materialCoefficientMixingtoolStripLabel";
            this.materialCoefficientMixingtoolStripLabel.Size = new System.Drawing.Size(144, 22);
            this.materialCoefficientMixingtoolStripLabel.Text = "MaterialCoefficientMixing";
            // 
            // materialCoefficientMixingToolStripComboBox
            // 
            this.materialCoefficientMixingToolStripComboBox.Items.AddRange(new object[] {
            "TakeMaximum",
            "TakeMinimum",
            "UseAverage"});
            this.materialCoefficientMixingToolStripComboBox.Name = "materialCoefficientMixingToolStripComboBox";
            this.materialCoefficientMixingToolStripComboBox.Size = new System.Drawing.Size(105, 25);
            this.materialCoefficientMixingToolStripComboBox.Text = "TakeMaximum";
            this.materialCoefficientMixingToolStripComboBox.SelectedIndexChanged += new System.EventHandler(this.materialCoefficientMixingToolStripComboBox_SelectedIndexChanged);
            // 
            // graphicsDeviceControl1
            // 
            this.graphicsDeviceControl1.AllowDrop = true;
            this.graphicsDeviceControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.graphicsDeviceControl1.Location = new System.Drawing.Point(0, 0);
            this.graphicsDeviceControl1.Name = "graphicsDeviceControl1";
            this.graphicsDeviceControl1.Size = new System.Drawing.Size(525, 292);
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
            this.Controls.Add(this.toolStripContainer1);
            this.Name = "Editor";
            this.Size = new System.Drawing.Size(525, 342);
            this.Load += new System.EventHandler(this.Editor_Load);
            this.toolStrip.ResumeLayout(false);
            this.toolStrip.PerformLayout();
            this.toolStripContainer1.BottomToolStripPanel.ResumeLayout(false);
            this.toolStripContainer1.BottomToolStripPanel.PerformLayout();
            this.toolStripContainer1.ContentPanel.ResumeLayout(false);
            this.toolStripContainer1.TopToolStripPanel.ResumeLayout(false);
            this.toolStripContainer1.TopToolStripPanel.PerformLayout();
            this.toolStripContainer1.ResumeLayout(false);
            this.toolStripContainer1.PerformLayout();
            this.physicsWorldToolStrip.ResumeLayout(false);
            this.physicsWorldToolStrip.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

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
        private System.Windows.Forms.ToolStripButton orthogonalStripButton;
        private System.Windows.Forms.ToolStripButton addRoadStripButton;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
        private System.Windows.Forms.ToolStripButton scaleModeToolStripButton;
        private System.Windows.Forms.ToolStrip miniToolStrip;
        private GraphicsDeviceControl graphicsDeviceControl1;
        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripContainer toolStripContainer1;
        private System.Windows.Forms.ToolStrip physicsWorldToolStrip;
        private System.Windows.Forms.ToolStripLabel gravityStripLabel;
        private System.Windows.Forms.ToolStripTextBox gravityToolStripTextBox;
        private System.Windows.Forms.ToolStripLabel materialCoefficientMixingtoolStripLabel;
        private System.Windows.Forms.ToolStripComboBox materialCoefficientMixingToolStripComboBox;
    }
}
