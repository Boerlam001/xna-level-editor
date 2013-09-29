namespace View
{
    partial class ObjectProperties
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
            this.txt_qZ = new System.Windows.Forms.TextBox();
            this.txt_qY = new System.Windows.Forms.TextBox();
            this.txt_qX = new System.Windows.Forms.TextBox();
            this.txt_qW = new System.Windows.Forms.TextBox();
            this.txt_rotZ = new System.Windows.Forms.TextBox();
            this.txt_rotY = new System.Windows.Forms.TextBox();
            this.txt_rotX = new System.Windows.Forms.TextBox();
            this.quaternionPanel = new System.Windows.Forms.Panel();
            this.positionPanel = new System.Windows.Forms.Panel();
            this.txt_posX = new System.Windows.Forms.TextBox();
            this.txt_posY = new System.Windows.Forms.TextBox();
            this.txt_posZ = new System.Windows.Forms.TextBox();
            this.txt_name = new System.Windows.Forms.TextBox();
            this.eulerPanel = new System.Windows.Forms.Panel();
            this.propertyGrid1 = new System.Windows.Forms.PropertyGrid();
            this.quaternionPanel.SuspendLayout();
            this.positionPanel.SuspendLayout();
            this.eulerPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // txt_qZ
            // 
            this.txt_qZ.Location = new System.Drawing.Point(3, 3);
            this.txt_qZ.Name = "txt_qZ";
            this.txt_qZ.Size = new System.Drawing.Size(100, 20);
            this.txt_qZ.TabIndex = 20;
            // 
            // txt_qY
            // 
            this.txt_qY.Location = new System.Drawing.Point(215, 3);
            this.txt_qY.Name = "txt_qY";
            this.txt_qY.Size = new System.Drawing.Size(100, 20);
            this.txt_qY.TabIndex = 19;
            this.txt_qY.PreviewKeyDown += new System.Windows.Forms.PreviewKeyDownEventHandler(this.txt_q_PreviewKeyDown);
            // 
            // txt_qX
            // 
            this.txt_qX.Location = new System.Drawing.Point(109, 3);
            this.txt_qX.Name = "txt_qX";
            this.txt_qX.Size = new System.Drawing.Size(100, 20);
            this.txt_qX.TabIndex = 18;
            this.txt_qX.PreviewKeyDown += new System.Windows.Forms.PreviewKeyDownEventHandler(this.txt_q_PreviewKeyDown);
            // 
            // txt_qW
            // 
            this.txt_qW.Location = new System.Drawing.Point(321, 3);
            this.txt_qW.Name = "txt_qW";
            this.txt_qW.Size = new System.Drawing.Size(100, 20);
            this.txt_qW.TabIndex = 17;
            this.txt_qW.PreviewKeyDown += new System.Windows.Forms.PreviewKeyDownEventHandler(this.txt_q_PreviewKeyDown);
            // 
            // txt_rotZ
            // 
            this.txt_rotZ.Location = new System.Drawing.Point(215, 3);
            this.txt_rotZ.Name = "txt_rotZ";
            this.txt_rotZ.Size = new System.Drawing.Size(100, 20);
            this.txt_rotZ.TabIndex = 16;
            this.txt_rotZ.PreviewKeyDown += new System.Windows.Forms.PreviewKeyDownEventHandler(this.txt_rot_PreviewKeyDown);
            // 
            // txt_rotY
            // 
            this.txt_rotY.Location = new System.Drawing.Point(109, 3);
            this.txt_rotY.Name = "txt_rotY";
            this.txt_rotY.Size = new System.Drawing.Size(100, 20);
            this.txt_rotY.TabIndex = 15;
            this.txt_rotY.PreviewKeyDown += new System.Windows.Forms.PreviewKeyDownEventHandler(this.txt_rot_PreviewKeyDown);
            // 
            // txt_rotX
            // 
            this.txt_rotX.Location = new System.Drawing.Point(3, 3);
            this.txt_rotX.Name = "txt_rotX";
            this.txt_rotX.Size = new System.Drawing.Size(100, 20);
            this.txt_rotX.TabIndex = 14;
            this.txt_rotX.PreviewKeyDown += new System.Windows.Forms.PreviewKeyDownEventHandler(this.txt_rot_PreviewKeyDown);
            // 
            // quaternionPanel
            // 
            this.quaternionPanel.AutoScroll = true;
            this.quaternionPanel.AutoScrollMinSize = new System.Drawing.Size(320, 0);
            this.quaternionPanel.Controls.Add(this.txt_qW);
            this.quaternionPanel.Controls.Add(this.txt_qX);
            this.quaternionPanel.Controls.Add(this.txt_qZ);
            this.quaternionPanel.Controls.Add(this.txt_qY);
            this.quaternionPanel.Location = new System.Drawing.Point(0, 65);
            this.quaternionPanel.Name = "quaternionPanel";
            this.quaternionPanel.Size = new System.Drawing.Size(320, 44);
            this.quaternionPanel.TabIndex = 22;
            this.quaternionPanel.Visible = false;
            // 
            // positionPanel
            // 
            this.positionPanel.Controls.Add(this.txt_posX);
            this.positionPanel.Controls.Add(this.txt_posY);
            this.positionPanel.Controls.Add(this.txt_posZ);
            this.positionPanel.Location = new System.Drawing.Point(0, 115);
            this.positionPanel.Name = "positionPanel";
            this.positionPanel.Size = new System.Drawing.Size(320, 29);
            this.positionPanel.TabIndex = 22;
            this.positionPanel.Visible = false;
            // 
            // txt_posX
            // 
            this.txt_posX.Location = new System.Drawing.Point(3, 3);
            this.txt_posX.Name = "txt_posX";
            this.txt_posX.Size = new System.Drawing.Size(100, 20);
            this.txt_posX.TabIndex = 14;
            this.txt_posX.PreviewKeyDown += new System.Windows.Forms.PreviewKeyDownEventHandler(this.txt_pos_PreviewKeyDown);
            // 
            // txt_posY
            // 
            this.txt_posY.Location = new System.Drawing.Point(109, 3);
            this.txt_posY.Name = "txt_posY";
            this.txt_posY.Size = new System.Drawing.Size(100, 20);
            this.txt_posY.TabIndex = 15;
            this.txt_posY.PreviewKeyDown += new System.Windows.Forms.PreviewKeyDownEventHandler(this.txt_pos_PreviewKeyDown);
            // 
            // txt_posZ
            // 
            this.txt_posZ.Location = new System.Drawing.Point(215, 3);
            this.txt_posZ.Name = "txt_posZ";
            this.txt_posZ.Size = new System.Drawing.Size(100, 20);
            this.txt_posZ.TabIndex = 16;
            this.txt_posZ.PreviewKeyDown += new System.Windows.Forms.PreviewKeyDownEventHandler(this.txt_pos_PreviewKeyDown);
            // 
            // txt_name
            // 
            this.txt_name.Location = new System.Drawing.Point(3, 4);
            this.txt_name.Name = "txt_name";
            this.txt_name.Size = new System.Drawing.Size(312, 20);
            this.txt_name.TabIndex = 23;
            this.txt_name.Visible = false;
            // 
            // eulerPanel
            // 
            this.eulerPanel.Controls.Add(this.txt_rotX);
            this.eulerPanel.Controls.Add(this.txt_rotY);
            this.eulerPanel.Controls.Add(this.txt_rotZ);
            this.eulerPanel.Location = new System.Drawing.Point(0, 30);
            this.eulerPanel.Name = "eulerPanel";
            this.eulerPanel.Size = new System.Drawing.Size(320, 29);
            this.eulerPanel.TabIndex = 21;
            this.eulerPanel.Visible = false;
            // 
            // propertyGrid1
            // 
            this.propertyGrid1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.propertyGrid1.Location = new System.Drawing.Point(0, 4);
            this.propertyGrid1.Name = "propertyGrid1";
            this.propertyGrid1.Size = new System.Drawing.Size(340, 157);
            this.propertyGrid1.TabIndex = 24;
            this.propertyGrid1.PropertyValueChanged += new System.Windows.Forms.PropertyValueChangedEventHandler(this.propertyGrid1_PropertyValueChanged);
            // 
            // ObjectProperties
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.propertyGrid1);
            this.Controls.Add(this.txt_name);
            this.Controls.Add(this.positionPanel);
            this.Controls.Add(this.quaternionPanel);
            this.Controls.Add(this.eulerPanel);
            this.Name = "ObjectProperties";
            this.Size = new System.Drawing.Size(343, 164);
            this.quaternionPanel.ResumeLayout(false);
            this.quaternionPanel.PerformLayout();
            this.positionPanel.ResumeLayout(false);
            this.positionPanel.PerformLayout();
            this.eulerPanel.ResumeLayout(false);
            this.eulerPanel.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox txt_qZ;
        private System.Windows.Forms.TextBox txt_qY;
        private System.Windows.Forms.TextBox txt_qX;
        private System.Windows.Forms.TextBox txt_qW;
        private System.Windows.Forms.TextBox txt_rotZ;
        private System.Windows.Forms.TextBox txt_rotY;
        private System.Windows.Forms.TextBox txt_rotX;
        private System.Windows.Forms.Panel quaternionPanel;
        private System.Windows.Forms.Panel positionPanel;
        private System.Windows.Forms.TextBox txt_posX;
        private System.Windows.Forms.TextBox txt_posY;
        private System.Windows.Forms.TextBox txt_posZ;
        private System.Windows.Forms.TextBox txt_name;
        private System.Windows.Forms.Panel eulerPanel;
        private System.Windows.Forms.PropertyGrid propertyGrid1;
    }
}
