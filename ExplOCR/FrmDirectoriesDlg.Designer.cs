namespace ExplOCR
{
    partial class FrmDirectoriesDlg
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
            this.components = new System.ComponentModel.Container();
            this.buttonOK = new System.Windows.Forms.Button();
            this.buttonCancel = new System.Windows.Forms.Button();
            this.textDB = new System.Windows.Forms.TextBox();
            this.textArchive = new System.Windows.Forms.TextBox();
            this.buttonBrowseDB = new System.Windows.Forms.Button();
            this.buttonBrowseArchive = new System.Windows.Forms.Button();
            this.labelDB = new System.Windows.Forms.Label();
            this.labelArchive = new System.Windows.Forms.Label();
            this.errorProvider = new System.Windows.Forms.ErrorProvider(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).BeginInit();
            this.SuspendLayout();
            // 
            // buttonOK
            // 
            this.buttonOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.buttonOK.Location = new System.Drawing.Point(366, 91);
            this.buttonOK.Name = "buttonOK";
            this.buttonOK.Size = new System.Drawing.Size(75, 23);
            this.buttonOK.TabIndex = 0;
            this.buttonOK.Text = "OK";
            this.buttonOK.UseVisualStyleBackColor = true;
            // 
            // buttonCancel
            // 
            this.buttonCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.buttonCancel.Location = new System.Drawing.Point(447, 91);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.Size = new System.Drawing.Size(75, 23);
            this.buttonCancel.TabIndex = 1;
            this.buttonCancel.Text = "Cancel";
            this.buttonCancel.UseVisualStyleBackColor = true;
            // 
            // textDB
            // 
            this.textDB.Location = new System.Drawing.Point(183, 14);
            this.textDB.Name = "textDB";
            this.textDB.Size = new System.Drawing.Size(259, 20);
            this.textDB.TabIndex = 2;
            // 
            // textArchive
            // 
            this.textArchive.Location = new System.Drawing.Point(183, 43);
            this.textArchive.Name = "textArchive";
            this.textArchive.Size = new System.Drawing.Size(259, 20);
            this.textArchive.TabIndex = 3;
            // 
            // buttonBrowseDB
            // 
            this.buttonBrowseDB.Location = new System.Drawing.Point(448, 12);
            this.buttonBrowseDB.Name = "buttonBrowseDB";
            this.buttonBrowseDB.Size = new System.Drawing.Size(75, 23);
            this.buttonBrowseDB.TabIndex = 4;
            this.buttonBrowseDB.Text = "Browse...";
            this.buttonBrowseDB.UseVisualStyleBackColor = true;
            this.buttonBrowseDB.Click += new System.EventHandler(this.buttonBrowseDB_Click);
            // 
            // buttonBrowseArchive
            // 
            this.buttonBrowseArchive.Location = new System.Drawing.Point(448, 41);
            this.buttonBrowseArchive.Name = "buttonBrowseArchive";
            this.buttonBrowseArchive.Size = new System.Drawing.Size(75, 23);
            this.buttonBrowseArchive.TabIndex = 5;
            this.buttonBrowseArchive.Text = "Browse...";
            this.buttonBrowseArchive.UseVisualStyleBackColor = true;
            this.buttonBrowseArchive.Click += new System.EventHandler(this.buttonBrowseArchive_Click);
            // 
            // labelDB
            // 
            this.labelDB.AutoSize = true;
            this.labelDB.Location = new System.Drawing.Point(12, 17);
            this.labelDB.Name = "labelDB";
            this.labelDB.Size = new System.Drawing.Size(101, 13);
            this.labelDB.TabIndex = 6;
            this.labelDB.Text = "Database Directory:";
            // 
            // labelArchive
            // 
            this.labelArchive.AutoSize = true;
            this.labelArchive.Location = new System.Drawing.Point(12, 46);
            this.labelArchive.Name = "labelArchive";
            this.labelArchive.Size = new System.Drawing.Size(148, 13);
            this.labelArchive.TabIndex = 7;
            this.labelArchive.Text = "Screenshot Archive Directory:";
            // 
            // errorProvider
            // 
            this.errorProvider.ContainerControl = this;
            // 
            // FrmDirectoriesDlg
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(534, 126);
            this.Controls.Add(this.labelArchive);
            this.Controls.Add(this.labelDB);
            this.Controls.Add(this.buttonBrowseArchive);
            this.Controls.Add(this.buttonBrowseDB);
            this.Controls.Add(this.textArchive);
            this.Controls.Add(this.textDB);
            this.Controls.Add(this.buttonCancel);
            this.Controls.Add(this.buttonOK);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.MaximumSize = new System.Drawing.Size(550, 160);
            this.MinimumSize = new System.Drawing.Size(550, 160);
            this.Name = "FrmDirectoriesDlg";
            this.Text = "Configure Directories";
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button buttonOK;
        private System.Windows.Forms.Button buttonCancel;
        private System.Windows.Forms.TextBox textDB;
        private System.Windows.Forms.TextBox textArchive;
        private System.Windows.Forms.Button buttonBrowseDB;
        private System.Windows.Forms.Button buttonBrowseArchive;
        private System.Windows.Forms.Label labelDB;
        private System.Windows.Forms.Label labelArchive;
        private System.Windows.Forms.ErrorProvider errorProvider;
    }
}