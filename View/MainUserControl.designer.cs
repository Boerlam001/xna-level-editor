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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainUserControl));
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.splitter2 = new System.Windows.Forms.Splitter();
            this.txt_rotX = new System.Windows.Forms.TextBox();
            this.txt_rotY = new System.Windows.Forms.TextBox();
            this.txt_rotZ = new System.Windows.Forms.TextBox();
            this.txt_qW = new System.Windows.Forms.TextBox();
            this.txt_qX = new System.Windows.Forms.TextBox();
            this.txt_qY = new System.Windows.Forms.TextBox();
            this.txt_qZ = new System.Windows.Forms.TextBox();
            this.graphicsDeviceControl1 = new View.GraphicsDeviceControl();
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.toolStripButton1 = new System.Windows.Forms.ToolStripButton();
            this.toolStripButton2 = new System.Windows.Forms.ToolStripButton();
            this.toolStripButton3 = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripButton4 = new System.Windows.Forms.ToolStripButton();
            this.toolStripButton5 = new System.Windows.Forms.ToolStripButton();
            this.toolStripButton6 = new System.Windows.Forms.ToolStripButton();
            this.txt_rotXFromQ = new System.Windows.Forms.TextBox();
            this.txt_rotYFromQ = new System.Windows.Forms.TextBox();
            this.txt_rotZFromQ = new System.Windows.Forms.TextBox();
            this.graphicsDeviceControl1.SuspendLayout();
            this.toolStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // timer1
            // 
            this.timer1.Enabled = true;
            this.timer1.Interval = 30;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // openFileDialog1
            // 
            this.openFileDialog1.FileName = "openFileDialog1";
            // 
            // splitter2
            // 
            this.splitter2.Dock = System.Windows.Forms.DockStyle.Top;
            this.splitter2.Location = new System.Drawing.Point(0, 0);
            this.splitter2.Name = "splitter2";
            this.splitter2.Size = new System.Drawing.Size(852, 3);
            this.splitter2.TabIndex = 6;
            this.splitter2.TabStop = false;
            // 
            // txt_rotX
            // 
            this.txt_rotX.Location = new System.Drawing.Point(749, 9);
            this.txt_rotX.Name = "txt_rotX";
            this.txt_rotX.Size = new System.Drawing.Size(100, 20);
            this.txt_rotX.TabIndex = 7;
            this.txt_rotX.PreviewKeyDown += new System.Windows.Forms.PreviewKeyDownEventHandler(this.txt_rot_PreviewKeyDown);
            // 
            // txt_rotY
            // 
            this.txt_rotY.Location = new System.Drawing.Point(749, 35);
            this.txt_rotY.Name = "txt_rotY";
            this.txt_rotY.Size = new System.Drawing.Size(100, 20);
            this.txt_rotY.TabIndex = 8;
            this.txt_rotY.PreviewKeyDown += new System.Windows.Forms.PreviewKeyDownEventHandler(this.txt_rot_PreviewKeyDown);
            // 
            // txt_rotZ
            // 
            this.txt_rotZ.Location = new System.Drawing.Point(749, 61);
            this.txt_rotZ.Name = "txt_rotZ";
            this.txt_rotZ.Size = new System.Drawing.Size(100, 20);
            this.txt_rotZ.TabIndex = 9;
            this.txt_rotZ.PreviewKeyDown += new System.Windows.Forms.PreviewKeyDownEventHandler(this.txt_rot_PreviewKeyDown);
            // 
            // txt_qW
            // 
            this.txt_qW.Location = new System.Drawing.Point(749, 124);
            this.txt_qW.Name = "txt_qW";
            this.txt_qW.Size = new System.Drawing.Size(100, 20);
            this.txt_qW.TabIndex = 10;
            this.txt_qW.PreviewKeyDown += new System.Windows.Forms.PreviewKeyDownEventHandler(this.txt_q_PreviewKeyDown);
            // 
            // txt_qX
            // 
            this.txt_qX.Location = new System.Drawing.Point(749, 151);
            this.txt_qX.Name = "txt_qX";
            this.txt_qX.Size = new System.Drawing.Size(100, 20);
            this.txt_qX.TabIndex = 11;
            this.txt_qX.PreviewKeyDown += new System.Windows.Forms.PreviewKeyDownEventHandler(this.txt_q_PreviewKeyDown);
            // 
            // txt_qY
            // 
            this.txt_qY.Location = new System.Drawing.Point(750, 178);
            this.txt_qY.Name = "txt_qY";
            this.txt_qY.Size = new System.Drawing.Size(100, 20);
            this.txt_qY.TabIndex = 12;
            this.txt_qY.PreviewKeyDown += new System.Windows.Forms.PreviewKeyDownEventHandler(this.txt_q_PreviewKeyDown);
            // 
            // txt_qZ
            // 
            this.txt_qZ.Location = new System.Drawing.Point(750, 205);
            this.txt_qZ.Name = "txt_qZ";
            this.txt_qZ.Size = new System.Drawing.Size(100, 20);
            this.txt_qZ.TabIndex = 13;
            this.txt_qZ.PreviewKeyDown += new System.Windows.Forms.PreviewKeyDownEventHandler(this.txt_q_PreviewKeyDown);
            // 
            // graphicsDeviceControl1
            // 
            this.graphicsDeviceControl1.Controls.Add(this.toolStrip1);
            this.graphicsDeviceControl1.Location = new System.Drawing.Point(3, 3);
            this.graphicsDeviceControl1.Name = "graphicsDeviceControl1";
            this.graphicsDeviceControl1.Size = new System.Drawing.Size(740, 450);
            this.graphicsDeviceControl1.TabIndex = 4;
            this.graphicsDeviceControl1.TabStop = true;
            this.graphicsDeviceControl1.Paint += new System.Windows.Forms.PaintEventHandler(this.graphicsDeviceControl1_Paint);
            this.graphicsDeviceControl1.MouseDown += new System.Windows.Forms.MouseEventHandler(this.graphicsDeviceControl1_MouseDown);
            this.graphicsDeviceControl1.MouseMove += new System.Windows.Forms.MouseEventHandler(this.graphicsDeviceControl1_MouseMove);
            this.graphicsDeviceControl1.MouseUp += new System.Windows.Forms.MouseEventHandler(this.graphicsDeviceControl1_MouseUp);
            this.graphicsDeviceControl1.PreviewKeyDown += new System.Windows.Forms.PreviewKeyDownEventHandler(this.graphicsDeviceControl1_KeyDown);
            // 
            // toolStrip1
            // 
            this.toolStrip1.Dock = System.Windows.Forms.DockStyle.Left;
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripButton1,
            this.toolStripButton2,
            this.toolStripButton3,
            this.toolStripSeparator1,
            this.toolStripButton4,
            this.toolStripButton5,
            this.toolStripButton6});
            this.toolStrip1.Location = new System.Drawing.Point(0, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(24, 450);
            this.toolStrip1.TabIndex = 0;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // toolStripButton1
            // 
            this.toolStripButton1.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButton1.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButton1.Image")));
            this.toolStripButton1.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButton1.Name = "toolStripButton1";
            this.toolStripButton1.Size = new System.Drawing.Size(21, 20);
            this.toolStripButton1.Text = "toolStripButton1";
            // 
            // toolStripButton2
            // 
            this.toolStripButton2.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButton2.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButton2.Image")));
            this.toolStripButton2.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButton2.Name = "toolStripButton2";
            this.toolStripButton2.Size = new System.Drawing.Size(21, 20);
            this.toolStripButton2.Text = "toolStripButton2";
            // 
            // toolStripButton3
            // 
            this.toolStripButton3.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButton3.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButton3.Image")));
            this.toolStripButton3.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButton3.Name = "toolStripButton3";
            this.toolStripButton3.Size = new System.Drawing.Size(21, 20);
            this.toolStripButton3.Text = "toolStripButton3";
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(21, 6);
            // 
            // toolStripButton4
            // 
            this.toolStripButton4.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButton4.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButton4.Image")));
            this.toolStripButton4.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButton4.Name = "toolStripButton4";
            this.toolStripButton4.Size = new System.Drawing.Size(21, 20);
            this.toolStripButton4.Text = "toolStripButton4";
            // 
            // toolStripButton5
            // 
            this.toolStripButton5.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButton5.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButton5.Image")));
            this.toolStripButton5.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButton5.Name = "toolStripButton5";
            this.toolStripButton5.Size = new System.Drawing.Size(21, 20);
            this.toolStripButton5.Text = "toolStripButton5";
            // 
            // toolStripButton6
            // 
            this.toolStripButton6.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButton6.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButton6.Image")));
            this.toolStripButton6.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButton6.Name = "toolStripButton6";
            this.toolStripButton6.Size = new System.Drawing.Size(21, 20);
            this.toolStripButton6.Text = "toolStripButton6";
            this.toolStripButton6.Click += new System.EventHandler(this.toolStripButton6_Click);
            // 
            // txt_rotXFromQ
            // 
            this.txt_rotXFromQ.Location = new System.Drawing.Point(750, 257);
            this.txt_rotXFromQ.Name = "txt_rotXFromQ";
            this.txt_rotXFromQ.Size = new System.Drawing.Size(100, 20);
            this.txt_rotXFromQ.TabIndex = 14;
            // 
            // txt_rotYFromQ
            // 
            this.txt_rotYFromQ.Location = new System.Drawing.Point(750, 284);
            this.txt_rotYFromQ.Name = "txt_rotYFromQ";
            this.txt_rotYFromQ.Size = new System.Drawing.Size(100, 20);
            this.txt_rotYFromQ.TabIndex = 15;
            // 
            // txt_rotZFromQ
            // 
            this.txt_rotZFromQ.Location = new System.Drawing.Point(750, 311);
            this.txt_rotZFromQ.Name = "txt_rotZFromQ";
            this.txt_rotZFromQ.Size = new System.Drawing.Size(100, 20);
            this.txt_rotZFromQ.TabIndex = 16;
            // 
            // MainUserControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.txt_rotZFromQ);
            this.Controls.Add(this.txt_rotYFromQ);
            this.Controls.Add(this.txt_rotXFromQ);
            this.Controls.Add(this.txt_qZ);
            this.Controls.Add(this.txt_qY);
            this.Controls.Add(this.txt_qX);
            this.Controls.Add(this.txt_qW);
            this.Controls.Add(this.txt_rotZ);
            this.Controls.Add(this.txt_rotY);
            this.Controls.Add(this.txt_rotX);
            this.Controls.Add(this.splitter2);
            this.Controls.Add(this.graphicsDeviceControl1);
            this.Name = "MainUserControl";
            this.Size = new System.Drawing.Size(852, 456);
            this.Load += new System.EventHandler(this.UserControl1_Load);
            this.graphicsDeviceControl1.ResumeLayout(false);
            this.graphicsDeviceControl1.PerformLayout();
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.OpenFileDialog openFileDialog1;
        private GraphicsDeviceControl graphicsDeviceControl1;
        private System.Windows.Forms.Splitter splitter2;
        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripButton toolStripButton1;
        private System.Windows.Forms.ToolStripButton toolStripButton2;
        private System.Windows.Forms.ToolStripButton toolStripButton3;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripButton toolStripButton4;
        private System.Windows.Forms.ToolStripButton toolStripButton5;
        private System.Windows.Forms.ToolStripButton toolStripButton6;
        private System.Windows.Forms.TextBox txt_rotX;
        private System.Windows.Forms.TextBox txt_rotY;
        private System.Windows.Forms.TextBox txt_rotZ;
        private System.Windows.Forms.TextBox txt_qW;
        private System.Windows.Forms.TextBox txt_qX;
        private System.Windows.Forms.TextBox txt_qY;
        private System.Windows.Forms.TextBox txt_qZ;
        private System.Windows.Forms.TextBox txt_rotXFromQ;
        private System.Windows.Forms.TextBox txt_rotYFromQ;
        private System.Windows.Forms.TextBox txt_rotZFromQ;
    }
}
