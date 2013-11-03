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
            this.objectTreeView = new System.Windows.Forms.TreeView();
            this.assetTreeView = new System.Windows.Forms.TreeView();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.objectExplorerBox = new System.Windows.Forms.GroupBox();
            this.splitContainer2 = new System.Windows.Forms.SplitContainer();
            this.assetExplorerBox = new System.Windows.Forms.GroupBox();
            this.objectProperties = new View.ObjectProperties();
            this.splitContainer3 = new System.Windows.Forms.SplitContainer();
            this.editor = new View.Editor();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.toolStripStatusLabel1 = new System.Windows.Forms.ToolStripStatusLabel();
            this.createFileStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.cameraPropertiesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.menuStrip = new System.Windows.Forms.MenuStrip();
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
            this.statusStrip1.SuspendLayout();
            this.menuStrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // objectTreeView
            // 
            this.objectTreeView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.objectTreeView.HideSelection = false;
            this.objectTreeView.Location = new System.Drawing.Point(3, 16);
            this.objectTreeView.Name = "objectTreeView";
            this.objectTreeView.Size = new System.Drawing.Size(222, 164);
            this.objectTreeView.TabIndex = 4;
            this.objectTreeView.NodeMouseClick += new System.Windows.Forms.TreeNodeMouseClickEventHandler(this.objectTreeView_NodeMouseClick);
            this.objectTreeView.NodeMouseDoubleClick += new System.Windows.Forms.TreeNodeMouseClickEventHandler(this.objectTreeView_NodeMouseDoubleClick);
            // 
            // assetTreeView
            // 
            this.assetTreeView.AllowDrop = true;
            this.assetTreeView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.assetTreeView.Location = new System.Drawing.Point(3, 16);
            this.assetTreeView.Name = "assetTreeView";
            this.assetTreeView.Size = new System.Drawing.Size(210, 164);
            this.assetTreeView.TabIndex = 5;
            this.assetTreeView.ItemDrag += new System.Windows.Forms.ItemDragEventHandler(this.assetTreeView_ItemDrag);
            this.assetTreeView.DragEnter += new System.Windows.Forms.DragEventHandler(this.assetTreeView_DragEnter);
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
            this.splitContainer1.Size = new System.Drawing.Size(668, 183);
            this.splitContainer1.SplitterDistance = 228;
            this.splitContainer1.TabIndex = 6;
            // 
            // objectExplorerBox
            // 
            this.objectExplorerBox.Controls.Add(this.objectTreeView);
            this.objectExplorerBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.objectExplorerBox.Location = new System.Drawing.Point(0, 0);
            this.objectExplorerBox.Name = "objectExplorerBox";
            this.objectExplorerBox.Size = new System.Drawing.Size(228, 183);
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
            this.splitContainer2.Panel2.Controls.Add(this.objectProperties);
            this.splitContainer2.Size = new System.Drawing.Size(436, 183);
            this.splitContainer2.SplitterDistance = 216;
            this.splitContainer2.TabIndex = 0;
            // 
            // assetExplorerBox
            // 
            this.assetExplorerBox.Controls.Add(this.assetTreeView);
            this.assetExplorerBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.assetExplorerBox.Location = new System.Drawing.Point(0, 0);
            this.assetExplorerBox.Name = "assetExplorerBox";
            this.assetExplorerBox.Size = new System.Drawing.Size(216, 183);
            this.assetExplorerBox.TabIndex = 6;
            this.assetExplorerBox.TabStop = false;
            this.assetExplorerBox.Text = "Assets";
            // 
            // objectProperties
            // 
            this.objectProperties.Dock = System.Windows.Forms.DockStyle.Fill;
            this.objectProperties.Location = new System.Drawing.Point(0, 0);
            this.objectProperties.MainUserControl = null;
            this.objectProperties.Model = null;
            this.objectProperties.Name = "objectProperties";
            this.objectProperties.Size = new System.Drawing.Size(216, 183);
            this.objectProperties.TabIndex = 1;
            // 
            // splitContainer3
            // 
            this.splitContainer3.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.splitContainer3.Location = new System.Drawing.Point(0, 24);
            this.splitContainer3.Name = "splitContainer3";
            this.splitContainer3.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer3.Panel1
            // 
            this.splitContainer3.Panel1.Controls.Add(this.editor);
            // 
            // splitContainer3.Panel2
            // 
            this.splitContainer3.Panel2.Controls.Add(this.splitContainer1);
            this.splitContainer3.Size = new System.Drawing.Size(668, 477);
            this.splitContainer3.SplitterDistance = 290;
            this.splitContainer3.TabIndex = 7;
            // 
            // editor
            // 
            this.editor.Dock = System.Windows.Forms.DockStyle.Fill;
            this.editor.GrassTexture = null;
            this.editor.Grid = null;
            this.editor.GridPointer = null;
            this.editor.GridPointers = null;
            this.editor.Location = new System.Drawing.Point(0, 0);
            this.editor.MainUserControl = null;
            this.editor.MapModel = null;
            this.editor.Name = "editor";
            this.editor.Selected = null;
            this.editor.Size = new System.Drawing.Size(668, 290);
            this.editor.TabIndex = 0;
            this.editor.TerrainBrush = null;
            this.editor.Text1 = "";
            this.editor.ViewBody = false;
            // 
            // statusStrip1
            // 
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripStatusLabel1});
            this.statusStrip1.Location = new System.Drawing.Point(0, 504);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(668, 22);
            this.statusStrip1.TabIndex = 8;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // toolStripStatusLabel1
            // 
            this.toolStripStatusLabel1.Name = "toolStripStatusLabel1";
            this.toolStripStatusLabel1.Size = new System.Drawing.Size(0, 17);
            // 
            // createFileStripMenuItem
            // 
            this.createFileStripMenuItem.Name = "createFileStripMenuItem";
            this.createFileStripMenuItem.Size = new System.Drawing.Size(66, 20);
            this.createFileStripMenuItem.Text = "Generate";
            this.createFileStripMenuItem.Click += new System.EventHandler(this.createFileStripMenuItem_Click);
            // 
            // cameraPropertiesToolStripMenuItem
            // 
            this.cameraPropertiesToolStripMenuItem.Name = "cameraPropertiesToolStripMenuItem";
            this.cameraPropertiesToolStripMenuItem.Size = new System.Drawing.Size(116, 20);
            this.cameraPropertiesToolStripMenuItem.Text = "Camera Properties";
            this.cameraPropertiesToolStripMenuItem.Visible = false;
            this.cameraPropertiesToolStripMenuItem.Click += new System.EventHandler(this.cameraPropertiesToolStripMenuItem_Click);
            // 
            // menuStrip
            // 
            this.menuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.createFileStripMenuItem,
            this.cameraPropertiesToolStripMenuItem});
            this.menuStrip.Location = new System.Drawing.Point(0, 0);
            this.menuStrip.Name = "menuStrip";
            this.menuStrip.Size = new System.Drawing.Size(668, 24);
            this.menuStrip.TabIndex = 3;
            this.menuStrip.Text = "menuStrip1";
            // 
            // MainUserControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.splitContainer3);
            this.Controls.Add(this.menuStrip);
            this.Name = "MainUserControl";
            this.Size = new System.Drawing.Size(668, 526);
            this.Load += new System.EventHandler(this.MainUserControl_Load);
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
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.menuStrip.ResumeLayout(false);
            this.menuStrip.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private Editor editor;
        private ObjectProperties objectProperties;
        private System.Windows.Forms.TreeView objectTreeView;
        private System.Windows.Forms.TreeView assetTreeView;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.SplitContainer splitContainer2;
        private System.Windows.Forms.SplitContainer splitContainer3;
        private System.Windows.Forms.GroupBox objectExplorerBox;
        private System.Windows.Forms.GroupBox assetExplorerBox;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel1;
        private System.Windows.Forms.ToolStripMenuItem createFileStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem cameraPropertiesToolStripMenuItem;
        private System.Windows.Forms.MenuStrip menuStrip;
    }
}
