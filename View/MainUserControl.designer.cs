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
            this.menuStrip = new System.Windows.Forms.MenuStrip();
            this.createFileStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveHeightmapToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.objectTreeView = new System.Windows.Forms.TreeView();
            this.assetTreeView = new System.Windows.Forms.TreeView();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.objectExplorerBox = new System.Windows.Forms.GroupBox();
            this.splitContainer2 = new System.Windows.Forms.SplitContainer();
            this.assetExplorerBox = new System.Windows.Forms.GroupBox();
            this.objectProperties1 = new View.ObjectProperties();
            this.splitContainer3 = new System.Windows.Forms.SplitContainer();
            this.editor1 = new View.Editor();
            this.menuStrip.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.objectExplorerBox.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).BeginInit();
            this.splitContainer2.Panel1.SuspendLayout();
            this.splitContainer2.Panel2.SuspendLayout();
            this.splitContainer2.SuspendLayout();
            this.assetExplorerBox.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer3)).BeginInit();
            this.splitContainer3.Panel1.SuspendLayout();
            this.splitContainer3.Panel2.SuspendLayout();
            this.splitContainer3.SuspendLayout();
            this.SuspendLayout();
            // 
            // menuStrip
            // 
            this.menuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.createFileStripMenuItem,
            this.saveHeightmapToolStripMenuItem});
            this.menuStrip.Location = new System.Drawing.Point(0, 0);
            this.menuStrip.Name = "menuStrip";
            this.menuStrip.Size = new System.Drawing.Size(668, 24);
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
            // saveHeightmapToolStripMenuItem
            // 
            this.saveHeightmapToolStripMenuItem.Name = "saveHeightmapToolStripMenuItem";
            this.saveHeightmapToolStripMenuItem.Size = new System.Drawing.Size(106, 20);
            this.saveHeightmapToolStripMenuItem.Text = "Save Heightmap";
            this.saveHeightmapToolStripMenuItem.Click += new System.EventHandler(this.saveHeightmapToolStripMenuItem_Click);
            // 
            // objectTreeView
            // 
            this.objectTreeView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.objectTreeView.Location = new System.Drawing.Point(3, 16);
            this.objectTreeView.Name = "objectTreeView";
            this.objectTreeView.Size = new System.Drawing.Size(228, 126);
            this.objectTreeView.TabIndex = 4;
            this.objectTreeView.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.objectTreeView_AfterSelect);
            // 
            // assetTreeView
            // 
            this.assetTreeView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.assetTreeView.Location = new System.Drawing.Point(3, 16);
            this.assetTreeView.Name = "assetTreeView";
            this.assetTreeView.Size = new System.Drawing.Size(224, 126);
            this.assetTreeView.TabIndex = 5;
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(0, 0);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.objectExplorerBox);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.splitContainer2);
            this.splitContainer1.Size = new System.Drawing.Size(668, 145);
            this.splitContainer1.SplitterDistance = 234;
            this.splitContainer1.TabIndex = 6;
            // 
            // objectExplorerBox
            // 
            this.objectExplorerBox.Controls.Add(this.objectTreeView);
            this.objectExplorerBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.objectExplorerBox.Location = new System.Drawing.Point(0, 0);
            this.objectExplorerBox.Name = "objectExplorerBox";
            this.objectExplorerBox.Size = new System.Drawing.Size(234, 145);
            this.objectExplorerBox.TabIndex = 5;
            this.objectExplorerBox.TabStop = false;
            this.objectExplorerBox.Text = "Objects";
            // 
            // splitContainer2
            // 
            this.splitContainer2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer2.Location = new System.Drawing.Point(0, 0);
            this.splitContainer2.Name = "splitContainer2";
            // 
            // splitContainer2.Panel1
            // 
            this.splitContainer2.Panel1.Controls.Add(this.assetExplorerBox);
            // 
            // splitContainer2.Panel2
            // 
            this.splitContainer2.Panel2.Controls.Add(this.objectProperties1);
            this.splitContainer2.Size = new System.Drawing.Size(430, 145);
            this.splitContainer2.SplitterDistance = 230;
            this.splitContainer2.TabIndex = 0;
            // 
            // assetExplorerBox
            // 
            this.assetExplorerBox.Controls.Add(this.assetTreeView);
            this.assetExplorerBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.assetExplorerBox.Location = new System.Drawing.Point(0, 0);
            this.assetExplorerBox.Name = "assetExplorerBox";
            this.assetExplorerBox.Size = new System.Drawing.Size(230, 145);
            this.assetExplorerBox.TabIndex = 6;
            this.assetExplorerBox.TabStop = false;
            this.assetExplorerBox.Text = "Assets";
            // 
            // objectProperties1
            // 
            this.objectProperties1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.objectProperties1.Location = new System.Drawing.Point(0, 0);
            this.objectProperties1.MainUserControl = null;
            this.objectProperties1.Model = null;
            this.objectProperties1.Name = "objectProperties1";
            this.objectProperties1.Size = new System.Drawing.Size(196, 145);
            this.objectProperties1.TabIndex = 1;
            // 
            // splitContainer3
            // 
            this.splitContainer3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer3.Location = new System.Drawing.Point(0, 24);
            this.splitContainer3.Name = "splitContainer3";
            this.splitContainer3.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer3.Panel1
            // 
            this.splitContainer3.Panel1.Controls.Add(this.editor1);
            // 
            // splitContainer3.Panel2
            // 
            this.splitContainer3.Panel2.Controls.Add(this.splitContainer1);
            this.splitContainer3.Size = new System.Drawing.Size(668, 502);
            this.splitContainer3.SplitterDistance = 353;
            this.splitContainer3.TabIndex = 7;
            // 
            // editor1
            // 
            this.editor1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.editor1.Location = new System.Drawing.Point(0, 0);
            this.editor1.MainUserControl = null;
            this.editor1.MapModel = null;
            this.editor1.Name = "editor1";
            this.editor1.Selected = null;
            this.editor1.Size = new System.Drawing.Size(668, 353);
            this.editor1.TabIndex = 0;
            this.editor1.TerrainBrush = null;
            this.editor1.Text1 = "";
            // 
            // MainUserControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.splitContainer3);
            this.Controls.Add(this.menuStrip);
            this.Name = "MainUserControl";
            this.Size = new System.Drawing.Size(668, 526);
            this.Load += new System.EventHandler(this.MainUserControl_Load);
            this.menuStrip.ResumeLayout(false);
            this.menuStrip.PerformLayout();
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.objectExplorerBox.ResumeLayout(false);
            this.splitContainer2.Panel1.ResumeLayout(false);
            this.splitContainer2.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).EndInit();
            this.splitContainer2.ResumeLayout(false);
            this.assetExplorerBox.ResumeLayout(false);
            this.splitContainer3.Panel1.ResumeLayout(false);
            this.splitContainer3.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer3)).EndInit();
            this.splitContainer3.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private Editor editor1;
        private ObjectProperties objectProperties1;
        private System.Windows.Forms.MenuStrip menuStrip;
        private System.Windows.Forms.ToolStripMenuItem createFileStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem saveHeightmapToolStripMenuItem;
        private System.Windows.Forms.TreeView objectTreeView;
        private System.Windows.Forms.TreeView assetTreeView;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.SplitContainer splitContainer2;
        private System.Windows.Forms.SplitContainer splitContainer3;
        private System.Windows.Forms.GroupBox objectExplorerBox;
        private System.Windows.Forms.GroupBox assetExplorerBox;
    }
}
