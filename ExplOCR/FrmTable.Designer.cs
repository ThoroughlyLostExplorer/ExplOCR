namespace ExplOCR
{
    partial class FrmTable
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FrmTable));
            this.dataGrid = new System.Windows.Forms.DataGridView();
            this.buttonApply = new System.Windows.Forms.Button();
            this.textRowFilter = new System.Windows.Forms.TextBox();
            this.checkRowFilter = new System.Windows.Forms.CheckBox();
            this.buttonColumns = new System.Windows.Forms.Button();
            this.splitContainer = new System.Windows.Forms.SplitContainer();
            this.panelControlContainer = new System.Windows.Forms.Panel();
            this.buttonTEST = new System.Windows.Forms.Button();
            this.editAstroBody = new ExplOCR.EditAstroBody();
            this.buttonCancel = new System.Windows.Forms.Button();
            this.buttonReRead = new System.Windows.Forms.Button();
            this.buttonSave = new System.Windows.Forms.Button();
            this.buttonHide = new System.Windows.Forms.Button();
            this.checkReadOnly = new System.Windows.Forms.CheckBox();
            this.tabControl = new System.Windows.Forms.TabControl();
            this.tabPageImage = new System.Windows.Forms.TabPage();
            this.panelImageComtainer = new System.Windows.Forms.Panel();
            this.imageDisplay = new ExplOCR.ImageDisplay();
            this.tabPageText = new System.Windows.Forms.TabPage();
            this.textOverview = new System.Windows.Forms.TextBox();
            ((System.ComponentModel.ISupportInitialize)(this.dataGrid)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer)).BeginInit();
            this.splitContainer.Panel1.SuspendLayout();
            this.splitContainer.Panel2.SuspendLayout();
            this.splitContainer.SuspendLayout();
            this.panelControlContainer.SuspendLayout();
            this.tabControl.SuspendLayout();
            this.tabPageImage.SuspendLayout();
            this.panelImageComtainer.SuspendLayout();
            this.tabPageText.SuspendLayout();
            this.SuspendLayout();
            // 
            // dataGrid
            // 
            this.dataGrid.AllowUserToAddRows = false;
            this.dataGrid.AllowUserToDeleteRows = false;
            this.dataGrid.AllowUserToResizeRows = false;
            this.dataGrid.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dataGrid.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGrid.Location = new System.Drawing.Point(3, 67);
            this.dataGrid.Name = "dataGrid";
            this.dataGrid.RowHeadersVisible = false;
            this.dataGrid.Size = new System.Drawing.Size(364, 539);
            this.dataGrid.TabIndex = 0;
            this.dataGrid.SelectionChanged += new System.EventHandler(this.dataGrid_SelectionChanged);
            // 
            // buttonApply
            // 
            this.buttonApply.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonApply.Location = new System.Drawing.Point(292, 10);
            this.buttonApply.Name = "buttonApply";
            this.buttonApply.Size = new System.Drawing.Size(75, 23);
            this.buttonApply.TabIndex = 1;
            this.buttonApply.Text = "Apply";
            this.buttonApply.UseVisualStyleBackColor = true;
            this.buttonApply.Click += new System.EventHandler(this.buttonApply_Click);
            // 
            // textRowFilter
            // 
            this.textRowFilter.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textRowFilter.Location = new System.Drawing.Point(96, 12);
            this.textRowFilter.Name = "textRowFilter";
            this.textRowFilter.Size = new System.Drawing.Size(190, 20);
            this.textRowFilter.TabIndex = 2;
            // 
            // checkRowFilter
            // 
            this.checkRowFilter.AutoSize = true;
            this.checkRowFilter.Location = new System.Drawing.Point(14, 14);
            this.checkRowFilter.Name = "checkRowFilter";
            this.checkRowFilter.Size = new System.Drawing.Size(76, 17);
            this.checkRowFilter.TabIndex = 3;
            this.checkRowFilter.Text = "Row Filter:";
            this.checkRowFilter.UseVisualStyleBackColor = true;
            this.checkRowFilter.CheckedChanged += new System.EventHandler(this.checkRowFilter_CheckedChanged);
            // 
            // buttonColumns
            // 
            this.buttonColumns.Location = new System.Drawing.Point(14, 38);
            this.buttonColumns.Name = "buttonColumns";
            this.buttonColumns.Size = new System.Drawing.Size(75, 23);
            this.buttonColumns.TabIndex = 4;
            this.buttonColumns.Text = "Columns...";
            this.buttonColumns.UseVisualStyleBackColor = true;
            this.buttonColumns.Click += new System.EventHandler(this.buttonColumns_Click);
            // 
            // splitContainer
            // 
            this.splitContainer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer.Location = new System.Drawing.Point(0, 0);
            this.splitContainer.Name = "splitContainer";
            // 
            // splitContainer.Panel1
            // 
            this.splitContainer.Panel1.Controls.Add(this.dataGrid);
            this.splitContainer.Panel1.Controls.Add(this.buttonApply);
            this.splitContainer.Panel1.Controls.Add(this.buttonColumns);
            this.splitContainer.Panel1.Controls.Add(this.checkRowFilter);
            this.splitContainer.Panel1.Controls.Add(this.textRowFilter);
            this.splitContainer.Panel1MinSize = 100;
            // 
            // splitContainer.Panel2
            // 
            this.splitContainer.Panel2.Controls.Add(this.panelControlContainer);
            this.splitContainer.Panel2.Controls.Add(this.tabControl);
            this.splitContainer.Panel2MinSize = 345;
            this.splitContainer.Size = new System.Drawing.Size(1169, 609);
            this.splitContainer.SplitterDistance = 370;
            this.splitContainer.TabIndex = 5;
            // 
            // panelControlContainer
            // 
            this.panelControlContainer.Controls.Add(this.buttonTEST);
            this.panelControlContainer.Controls.Add(this.editAstroBody);
            this.panelControlContainer.Controls.Add(this.buttonCancel);
            this.panelControlContainer.Controls.Add(this.buttonReRead);
            this.panelControlContainer.Controls.Add(this.buttonSave);
            this.panelControlContainer.Controls.Add(this.buttonHide);
            this.panelControlContainer.Controls.Add(this.checkReadOnly);
            this.panelControlContainer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelControlContainer.Location = new System.Drawing.Point(0, 0);
            this.panelControlContainer.Name = "panelControlContainer";
            this.panelControlContainer.Size = new System.Drawing.Size(330, 609);
            this.panelControlContainer.TabIndex = 23;
            // 
            // buttonTEST
            // 
            this.buttonTEST.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.buttonTEST.Location = new System.Drawing.Point(85, 543);
            this.buttonTEST.Name = "buttonTEST";
            this.buttonTEST.Size = new System.Drawing.Size(75, 23);
            this.buttonTEST.TabIndex = 23;
            this.buttonTEST.Text = "API Test";
            this.buttonTEST.UseVisualStyleBackColor = true;
            this.buttonTEST.Click += new System.EventHandler(this.buttonTEST_Click);
            // 
            // editAstroBody
            // 
            this.editAstroBody.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.editAstroBody.HasChanges = false;
            this.editAstroBody.Location = new System.Drawing.Point(0, 0);
            this.editAstroBody.Name = "editAstroBody";
            this.editAstroBody.ReadOnly = false;
            this.editAstroBody.Size = new System.Drawing.Size(328, 345);
            this.editAstroBody.TabIndex = 19;
            this.editAstroBody.StateChanged += new System.EventHandler(this.editAstroBody_StateChanged);
            // 
            // buttonCancel
            // 
            this.buttonCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.buttonCancel.Location = new System.Drawing.Point(84, 574);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.Size = new System.Drawing.Size(75, 23);
            this.buttonCancel.TabIndex = 18;
            this.buttonCancel.Text = "Cancel";
            this.buttonCancel.UseVisualStyleBackColor = true;
            this.buttonCancel.Click += new System.EventHandler(this.buttonCancel_Click);
            // 
            // buttonReRead
            // 
            this.buttonReRead.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonReRead.Location = new System.Drawing.Point(249, 545);
            this.buttonReRead.Name = "buttonReRead";
            this.buttonReRead.Size = new System.Drawing.Size(75, 23);
            this.buttonReRead.TabIndex = 22;
            this.buttonReRead.Text = "Re-Read";
            this.buttonReRead.UseVisualStyleBackColor = true;
            this.buttonReRead.Click += new System.EventHandler(this.buttonReRead_Click);
            // 
            // buttonSave
            // 
            this.buttonSave.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.buttonSave.Location = new System.Drawing.Point(3, 574);
            this.buttonSave.Name = "buttonSave";
            this.buttonSave.Size = new System.Drawing.Size(75, 23);
            this.buttonSave.TabIndex = 6;
            this.buttonSave.Text = "Save";
            this.buttonSave.UseVisualStyleBackColor = true;
            this.buttonSave.Click += new System.EventHandler(this.buttonSave_Click);
            // 
            // buttonHide
            // 
            this.buttonHide.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonHide.Location = new System.Drawing.Point(249, 574);
            this.buttonHide.Name = "buttonHide";
            this.buttonHide.Size = new System.Drawing.Size(75, 23);
            this.buttonHide.TabIndex = 21;
            this.buttonHide.Text = "Hide >";
            this.buttonHide.UseVisualStyleBackColor = true;
            this.buttonHide.Click += new System.EventHandler(this.buttonHide_Click);
            // 
            // checkReadOnly
            // 
            this.checkReadOnly.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.checkReadOnly.AutoSize = true;
            this.checkReadOnly.ForeColor = System.Drawing.Color.DarkRed;
            this.checkReadOnly.Location = new System.Drawing.Point(3, 549);
            this.checkReadOnly.Name = "checkReadOnly";
            this.checkReadOnly.Size = new System.Drawing.Size(76, 17);
            this.checkReadOnly.TabIndex = 20;
            this.checkReadOnly.Text = "Read Only";
            this.checkReadOnly.UseVisualStyleBackColor = true;
            this.checkReadOnly.CheckedChanged += new System.EventHandler(this.checkReadOnly_CheckedChanged);
            // 
            // tabControl
            // 
            this.tabControl.Controls.Add(this.tabPageImage);
            this.tabControl.Controls.Add(this.tabPageText);
            this.tabControl.Dock = System.Windows.Forms.DockStyle.Right;
            this.tabControl.Location = new System.Drawing.Point(330, 0);
            this.tabControl.Name = "tabControl";
            this.tabControl.SelectedIndex = 0;
            this.tabControl.Size = new System.Drawing.Size(465, 609);
            this.tabControl.TabIndex = 21;
            // 
            // tabPageImage
            // 
            this.tabPageImage.Controls.Add(this.panelImageComtainer);
            this.tabPageImage.Location = new System.Drawing.Point(4, 22);
            this.tabPageImage.Name = "tabPageImage";
            this.tabPageImage.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageImage.Size = new System.Drawing.Size(457, 583);
            this.tabPageImage.TabIndex = 0;
            this.tabPageImage.Text = "Image";
            this.tabPageImage.UseVisualStyleBackColor = true;
            // 
            // panelImageComtainer
            // 
            this.panelImageComtainer.AutoScroll = true;
            this.panelImageComtainer.Controls.Add(this.imageDisplay);
            this.panelImageComtainer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelImageComtainer.Location = new System.Drawing.Point(3, 3);
            this.panelImageComtainer.Name = "panelImageComtainer";
            this.panelImageComtainer.Size = new System.Drawing.Size(451, 577);
            this.panelImageComtainer.TabIndex = 18;
            // 
            // imageDisplay
            // 
            this.imageDisplay.Image = ((System.Drawing.Bitmap)(resources.GetObject("imageDisplay.Image")));
            this.imageDisplay.Location = new System.Drawing.Point(0, 0);
            this.imageDisplay.Name = "imageDisplay";
            this.imageDisplay.Size = new System.Drawing.Size(150, 150);
            this.imageDisplay.TabIndex = 0;
            // 
            // tabPageText
            // 
            this.tabPageText.Controls.Add(this.textOverview);
            this.tabPageText.Location = new System.Drawing.Point(4, 22);
            this.tabPageText.Name = "tabPageText";
            this.tabPageText.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageText.Size = new System.Drawing.Size(457, 583);
            this.tabPageText.TabIndex = 1;
            this.tabPageText.Text = "Text";
            this.tabPageText.UseVisualStyleBackColor = true;
            // 
            // textOverview
            // 
            this.textOverview.BackColor = System.Drawing.SystemColors.Window;
            this.textOverview.Dock = System.Windows.Forms.DockStyle.Fill;
            this.textOverview.Font = new System.Drawing.Font("Courier New", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textOverview.Location = new System.Drawing.Point(3, 3);
            this.textOverview.Multiline = true;
            this.textOverview.Name = "textOverview";
            this.textOverview.ReadOnly = true;
            this.textOverview.Size = new System.Drawing.Size(451, 577);
            this.textOverview.TabIndex = 0;
            // 
            // FrmTable
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1169, 609);
            this.Controls.Add(this.splitContainer);
            this.MinimumSize = new System.Drawing.Size(600, 400);
            this.Name = "FrmTable";
            this.Text = "FrmGrid";
            ((System.ComponentModel.ISupportInitialize)(this.dataGrid)).EndInit();
            this.splitContainer.Panel1.ResumeLayout(false);
            this.splitContainer.Panel1.PerformLayout();
            this.splitContainer.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer)).EndInit();
            this.splitContainer.ResumeLayout(false);
            this.panelControlContainer.ResumeLayout(false);
            this.panelControlContainer.PerformLayout();
            this.tabControl.ResumeLayout(false);
            this.tabPageImage.ResumeLayout(false);
            this.panelImageComtainer.ResumeLayout(false);
            this.tabPageText.ResumeLayout(false);
            this.tabPageText.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.DataGridView dataGrid;
        private System.Windows.Forms.Button buttonApply;
        private System.Windows.Forms.TextBox textRowFilter;
        private System.Windows.Forms.CheckBox checkRowFilter;
        private System.Windows.Forms.Button buttonColumns;
        private System.Windows.Forms.SplitContainer splitContainer;
        private System.Windows.Forms.TabControl tabControl;
        private System.Windows.Forms.TabPage tabPageImage;
        private System.Windows.Forms.Panel panelImageComtainer;
        private System.Windows.Forms.TabPage tabPageText;
        private System.Windows.Forms.TextBox textOverview;
        private ImageDisplay imageDisplay;
        private System.Windows.Forms.Panel panelControlContainer;
        private EditAstroBody editAstroBody;
        private System.Windows.Forms.Button buttonCancel;
        private System.Windows.Forms.Button buttonReRead;
        private System.Windows.Forms.Button buttonSave;
        private System.Windows.Forms.Button buttonHide;
        private System.Windows.Forms.CheckBox checkReadOnly;
        private System.Windows.Forms.Button buttonTEST;
    }
}