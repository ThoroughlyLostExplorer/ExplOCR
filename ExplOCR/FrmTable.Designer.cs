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
            this.dataGrid = new System.Windows.Forms.DataGridView();
            this.buttonApply = new System.Windows.Forms.Button();
            this.textRowFilter = new System.Windows.Forms.TextBox();
            this.checkRowFilter = new System.Windows.Forms.CheckBox();
            this.buttonColumns = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.dataGrid)).BeginInit();
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
            this.dataGrid.Location = new System.Drawing.Point(12, 70);
            this.dataGrid.Name = "dataGrid";
            this.dataGrid.RowHeadersVisible = false;
            this.dataGrid.Size = new System.Drawing.Size(690, 390);
            this.dataGrid.TabIndex = 0;
            // 
            // buttonApply
            // 
            this.buttonApply.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonApply.Location = new System.Drawing.Point(627, 12);
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
            this.textRowFilter.Location = new System.Drawing.Point(94, 14);
            this.textRowFilter.Name = "textRowFilter";
            this.textRowFilter.Size = new System.Drawing.Size(527, 20);
            this.textRowFilter.TabIndex = 2;
            // 
            // checkRowFilter
            // 
            this.checkRowFilter.AutoSize = true;
            this.checkRowFilter.Location = new System.Drawing.Point(12, 16);
            this.checkRowFilter.Name = "checkRowFilter";
            this.checkRowFilter.Size = new System.Drawing.Size(76, 17);
            this.checkRowFilter.TabIndex = 3;
            this.checkRowFilter.Text = "Row Filter:";
            this.checkRowFilter.UseVisualStyleBackColor = true;
            this.checkRowFilter.CheckedChanged += new System.EventHandler(this.checkRowFilter_CheckedChanged);
            // 
            // buttonColumns
            // 
            this.buttonColumns.Location = new System.Drawing.Point(12, 39);
            this.buttonColumns.Name = "buttonColumns";
            this.buttonColumns.Size = new System.Drawing.Size(75, 23);
            this.buttonColumns.TabIndex = 4;
            this.buttonColumns.Text = "Columns...";
            this.buttonColumns.UseVisualStyleBackColor = true;
            this.buttonColumns.Click += new System.EventHandler(this.buttonColumns_Click);
            // 
            // FrmTable
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(714, 472);
            this.Controls.Add(this.buttonColumns);
            this.Controls.Add(this.checkRowFilter);
            this.Controls.Add(this.textRowFilter);
            this.Controls.Add(this.buttonApply);
            this.Controls.Add(this.dataGrid);
            this.Name = "FrmTable";
            this.Text = "FrmGrid";
            ((System.ComponentModel.ISupportInitialize)(this.dataGrid)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.DataGridView dataGrid;
        private System.Windows.Forms.Button buttonApply;
        private System.Windows.Forms.TextBox textRowFilter;
        private System.Windows.Forms.CheckBox checkRowFilter;
        private System.Windows.Forms.Button buttonColumns;
    }
}