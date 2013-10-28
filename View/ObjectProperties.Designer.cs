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
            this.propertyGrid1 = new System.Windows.Forms.PropertyGrid();
            this.viewBodyCheckBox = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // propertyGrid1
            // 
            this.propertyGrid1.AllowDrop = true;
            this.propertyGrid1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.propertyGrid1.CommandsForeColor = System.Drawing.Color.Cornsilk;
            this.propertyGrid1.Location = new System.Drawing.Point(0, 0);
            this.propertyGrid1.Name = "propertyGrid1";
            this.propertyGrid1.Size = new System.Drawing.Size(337, 180);
            this.propertyGrid1.TabIndex = 24;
            this.propertyGrid1.PropertyValueChanged += new System.Windows.Forms.PropertyValueChangedEventHandler(this.propertyGrid_PropertyValueChanged);
            this.propertyGrid1.DragDrop += new System.Windows.Forms.DragEventHandler(this.propertyGrid1_DragDrop);
            this.propertyGrid1.DragEnter += new System.Windows.Forms.DragEventHandler(this.propertyGrid1_DragEnter);
            // 
            // viewBodyCheckBox
            // 
            this.viewBodyCheckBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.viewBodyCheckBox.AutoSize = true;
            this.viewBodyCheckBox.Location = new System.Drawing.Point(261, 3);
            this.viewBodyCheckBox.Name = "viewBodyCheckBox";
            this.viewBodyCheckBox.Size = new System.Drawing.Size(76, 17);
            this.viewBodyCheckBox.TabIndex = 25;
            this.viewBodyCheckBox.Text = "View Body";
            this.viewBodyCheckBox.UseVisualStyleBackColor = true;
            this.viewBodyCheckBox.CheckedChanged += new System.EventHandler(this.viewBodyCheckBox_CheckedChanged);
            // 
            // ObjectProperties
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.viewBodyCheckBox);
            this.Controls.Add(this.propertyGrid1);
            this.Name = "ObjectProperties";
            this.Size = new System.Drawing.Size(340, 180);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PropertyGrid propertyGrid1;
        private System.Windows.Forms.CheckBox viewBodyCheckBox;
    }
}
