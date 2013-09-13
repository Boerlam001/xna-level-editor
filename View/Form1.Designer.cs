namespace View
{
    partial class Form1
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

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            XleGenerator.ClassManager classManager1 = new XleGenerator.ClassManager();
            this.mainUserControl1 = new View.MainUserControl();
            this.button1 = new System.Windows.Forms.Button();
            this.button3 = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // mainUserControl1
            // 
            classManager1.ClassFile = null;
            classManager1.ContentBuilder = null;
            classManager1.ContentProject = null;
            classManager1.MapModel = null;
            classManager1.Name = "asd";
            classManager1.Output = null;
            this.mainUserControl1._ClassManager = classManager1;
            this.mainUserControl1.ApplicationObject = null;
            this.mainUserControl1.IsOpen = false;
            this.mainUserControl1.Location = new System.Drawing.Point(12, 12);
            this.mainUserControl1.Name = "mainUserControl1";
            this.mainUserControl1.ProjectItem = null;
            this.mainUserControl1.Size = new System.Drawing.Size(662, 526);
            this.mainUserControl1.TabIndex = 0;
            this.mainUserControl1.Window = null;
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(12, 544);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 1;
            this.button1.Text = "button1";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // button3
            // 
            this.button3.Location = new System.Drawing.Point(228, 554);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(75, 23);
            this.button3.TabIndex = 3;
            this.button3.Text = "button3";
            this.button3.UseVisualStyleBackColor = true;
            this.button3.Click += new System.EventHandler(this.button3_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(698, 582);
            this.Controls.Add(this.button3);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.mainUserControl1);
            this.Name = "Form1";
            this.Text = "Form1";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private MainUserControl mainUserControl1;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button button3;



    }
}

