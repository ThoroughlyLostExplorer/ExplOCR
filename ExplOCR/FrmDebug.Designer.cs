namespace ExplOCR
{
    partial class FrmDebug
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
            this.buttonB = new System.Windows.Forms.Button();
            this.buttonF = new System.Windows.Forms.Button();
            this.panel1 = new System.Windows.Forms.Panel();
            this.SuspendLayout();
            // 
            // buttonB
            // 
            this.buttonB.Location = new System.Drawing.Point(12, 227);
            this.buttonB.Name = "buttonB";
            this.buttonB.Size = new System.Drawing.Size(75, 23);
            this.buttonB.TabIndex = 0;
            this.buttonB.Text = "<-";
            this.buttonB.UseVisualStyleBackColor = true;
            this.buttonB.Click += new System.EventHandler(this.buttonB_Click);
            // 
            // buttonF
            // 
            this.buttonF.Location = new System.Drawing.Point(93, 227);
            this.buttonF.Name = "buttonF";
            this.buttonF.Size = new System.Drawing.Size(75, 23);
            this.buttonF.TabIndex = 1;
            this.buttonF.Text = "->";
            this.buttonF.UseVisualStyleBackColor = true;
            this.buttonF.Click += new System.EventHandler(this.buttonF_Click);
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.Color.White;
            this.panel1.Location = new System.Drawing.Point(12, 12);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(260, 209);
            this.panel1.TabIndex = 2;
            this.panel1.Paint += new System.Windows.Forms.PaintEventHandler(this.panel1_Paint);
            // 
            // FrmDebug
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(284, 262);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.buttonF);
            this.Controls.Add(this.buttonB);
            this.MaximumSize = new System.Drawing.Size(300, 300);
            this.MinimumSize = new System.Drawing.Size(300, 300);
            this.Name = "FrmDebug";
            this.Text = "FrmDebug";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button buttonB;
        private System.Windows.Forms.Button buttonF;
        private System.Windows.Forms.Panel panel1;
    }
}