namespace ExplOCR
{
    partial class EditAstroBody
    {
        /// <summary> 
        /// Erforderliche Designervariable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Verwendete Ressourcen bereinigen.
        /// </summary>
        /// <param name="disposing">True, wenn verwaltete Ressourcen gelöscht werden sollen; andernfalls False.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Vom Komponenten-Designer generierter Code

        /// <summary> 
        /// Erforderliche Methode für die Designerunterstützung. 
        /// Der Inhalt der Methode darf nicht mit dem Code-Editor geändert werden.
        /// </summary>
        private void InitializeComponent()
        {
            this.label1 = new System.Windows.Forms.Label();
            this.textDescription = new System.Windows.Forms.TextBox();
            this.dataGridEdit = new System.Windows.Forms.DataGridView();
            this.columnEditName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.columnEditIndex = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.columnEditValue = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.textSystem = new System.Windows.Forms.TextBox();
            this.comboType = new System.Windows.Forms.ComboBox();
            this.label2 = new System.Windows.Forms.Label();
            this.labelType = new System.Windows.Forms.Label();
            this.textBody = new System.Windows.Forms.TextBox();
            this.textCategories = new System.Windows.Forms.TextBox();
            this.labelDescription = new System.Windows.Forms.Label();
            this.labelCategories = new System.Windows.Forms.Label();
            this.panelContainer = new System.Windows.Forms.Panel();
            this.splitContainer = new System.Windows.Forms.SplitContainer();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridEdit)).BeginInit();
            this.panelContainer.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer)).BeginInit();
            this.splitContainer.Panel1.SuspendLayout();
            this.splitContainer.Panel2.SuspendLayout();
            this.splitContainer.SuspendLayout();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(0, 6);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(44, 13);
            this.label1.TabIndex = 19;
            this.label1.Text = "System:";
            // 
            // textDescription
            // 
            this.textDescription.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textDescription.Location = new System.Drawing.Point(0, 177);
            this.textDescription.Multiline = true;
            this.textDescription.Name = "textDescription";
            this.textDescription.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.textDescription.Size = new System.Drawing.Size(288, 143);
            this.textDescription.TabIndex = 18;
            this.textDescription.TextChanged += new System.EventHandler(this.EditField_TextChanged);
            // 
            // dataGridEdit
            // 
            this.dataGridEdit.AllowUserToResizeRows = false;
            this.dataGridEdit.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridEdit.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.columnEditName,
            this.columnEditIndex,
            this.columnEditValue});
            this.dataGridEdit.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataGridEdit.Location = new System.Drawing.Point(0, 0);
            this.dataGridEdit.Name = "dataGridEdit";
            this.dataGridEdit.RowHeadersVisible = false;
            this.dataGridEdit.Size = new System.Drawing.Size(305, 320);
            this.dataGridEdit.TabIndex = 28;
            this.dataGridEdit.CellValueChanged += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridEdit_CellValueChanged);
            this.dataGridEdit.RowsAdded += new System.Windows.Forms.DataGridViewRowsAddedEventHandler(this.dataGridEdit_RowsAdded);
            this.dataGridEdit.RowsRemoved += new System.Windows.Forms.DataGridViewRowsRemovedEventHandler(this.dataGridEdit_RowsRemoved);
            // 
            // columnEditName
            // 
            this.columnEditName.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.columnEditName.HeaderText = "Property";
            this.columnEditName.Name = "columnEditName";
            this.columnEditName.Width = 71;
            // 
            // columnEditIndex
            // 
            this.columnEditIndex.HeaderText = "Index";
            this.columnEditIndex.Name = "columnEditIndex";
            this.columnEditIndex.ReadOnly = true;
            this.columnEditIndex.Visible = false;
            // 
            // columnEditValue
            // 
            this.columnEditValue.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.columnEditValue.HeaderText = "Value";
            this.columnEditValue.Name = "columnEditValue";
            // 
            // textSystem
            // 
            this.textSystem.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textSystem.Location = new System.Drawing.Point(58, 3);
            this.textSystem.Name = "textSystem";
            this.textSystem.Size = new System.Drawing.Size(230, 20);
            this.textSystem.TabIndex = 20;
            this.textSystem.Text = "Blah bla bla sector AB-C T7-1234";
            this.textSystem.TextChanged += new System.EventHandler(this.EditField_TextChanged);
            // 
            // comboType
            // 
            this.comboType.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.comboType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboType.FormattingEnabled = true;
            this.comboType.Location = new System.Drawing.Point(58, 53);
            this.comboType.Name = "comboType";
            this.comboType.Size = new System.Drawing.Size(230, 21);
            this.comboType.TabIndex = 27;
            this.comboType.SelectedIndexChanged += new System.EventHandler(this.comboType_SelectedIndexChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(0, 30);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(34, 13);
            this.label2.TabIndex = 21;
            this.label2.Text = "Body:";
            // 
            // labelType
            // 
            this.labelType.AutoSize = true;
            this.labelType.Location = new System.Drawing.Point(0, 56);
            this.labelType.Name = "labelType";
            this.labelType.Size = new System.Drawing.Size(34, 13);
            this.labelType.TabIndex = 26;
            this.labelType.Text = "Type:";
            // 
            // textBody
            // 
            this.textBody.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBody.Location = new System.Drawing.Point(58, 27);
            this.textBody.Name = "textBody";
            this.textBody.Size = new System.Drawing.Size(230, 20);
            this.textBody.TabIndex = 22;
            this.textBody.TextChanged += new System.EventHandler(this.EditField_TextChanged);
            // 
            // textCategories
            // 
            this.textCategories.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textCategories.Location = new System.Drawing.Point(104, 90);
            this.textCategories.Multiline = true;
            this.textCategories.Name = "textCategories";
            this.textCategories.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.textCategories.Size = new System.Drawing.Size(184, 62);
            this.textCategories.TabIndex = 25;
            this.textCategories.TextChanged += new System.EventHandler(this.EditField_TextChanged);
            // 
            // labelDescription
            // 
            this.labelDescription.AutoSize = true;
            this.labelDescription.Location = new System.Drawing.Point(0, 161);
            this.labelDescription.Name = "labelDescription";
            this.labelDescription.Size = new System.Drawing.Size(101, 13);
            this.labelDescription.TabIndex = 23;
            this.labelDescription.Text = "Custom Description:";
            // 
            // labelCategories
            // 
            this.labelCategories.AutoSize = true;
            this.labelCategories.Location = new System.Drawing.Point(0, 93);
            this.labelCategories.Name = "labelCategories";
            this.labelCategories.Size = new System.Drawing.Size(98, 13);
            this.labelCategories.TabIndex = 24;
            this.labelCategories.Text = "Custom Categories:";
            // 
            // panelContainer
            // 
            this.panelContainer.Controls.Add(this.label1);
            this.panelContainer.Controls.Add(this.textSystem);
            this.panelContainer.Controls.Add(this.textDescription);
            this.panelContainer.Controls.Add(this.labelCategories);
            this.panelContainer.Controls.Add(this.labelDescription);
            this.panelContainer.Controls.Add(this.textCategories);
            this.panelContainer.Controls.Add(this.comboType);
            this.panelContainer.Controls.Add(this.textBody);
            this.panelContainer.Controls.Add(this.label2);
            this.panelContainer.Controls.Add(this.labelType);
            this.panelContainer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelContainer.Location = new System.Drawing.Point(0, 0);
            this.panelContainer.Name = "panelContainer";
            this.panelContainer.Size = new System.Drawing.Size(291, 320);
            this.panelContainer.TabIndex = 29;
            // 
            // splitContainer
            // 
            this.splitContainer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer.Location = new System.Drawing.Point(0, 0);
            this.splitContainer.Name = "splitContainer";
            // 
            // splitContainer.Panel1
            // 
            this.splitContainer.Panel1.Controls.Add(this.panelContainer);
            // 
            // splitContainer.Panel2
            // 
            this.splitContainer.Panel2.Controls.Add(this.dataGridEdit);
            this.splitContainer.Size = new System.Drawing.Size(600, 320);
            this.splitContainer.SplitterDistance = 291;
            this.splitContainer.TabIndex = 30;
            // 
            // EditAstroBody
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.splitContainer);
            this.Name = "EditAstroBody";
            this.Size = new System.Drawing.Size(600, 320);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridEdit)).EndInit();
            this.panelContainer.ResumeLayout(false);
            this.panelContainer.PerformLayout();
            this.splitContainer.Panel1.ResumeLayout(false);
            this.splitContainer.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer)).EndInit();
            this.splitContainer.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox textDescription;
        private System.Windows.Forms.DataGridView dataGridEdit;
        private System.Windows.Forms.TextBox textSystem;
        private System.Windows.Forms.ComboBox comboType;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label labelType;
        private System.Windows.Forms.TextBox textBody;
        private System.Windows.Forms.TextBox textCategories;
        private System.Windows.Forms.Label labelDescription;
        private System.Windows.Forms.Label labelCategories;
        private System.Windows.Forms.Panel panelContainer;
        private System.Windows.Forms.SplitContainer splitContainer;
        private System.Windows.Forms.DataGridViewTextBoxColumn columnEditName;
        private System.Windows.Forms.DataGridViewTextBoxColumn columnEditIndex;
        private System.Windows.Forms.DataGridViewTextBoxColumn columnEditValue;
    }
}
