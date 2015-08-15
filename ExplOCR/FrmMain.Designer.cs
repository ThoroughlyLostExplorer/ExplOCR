namespace ExplOCR
{
    partial class FrmMain
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
            if (drawBmpRaw != null)
            {
                drawBmpRaw.Dispose();
            }
            if (drawBmp != null)
            {
                drawBmp.Dispose();
            }
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Vom Windows Form-Designer generierter Code

        /// <summary>
        /// Erforderliche Methode für die Designerunterstützung.
        /// Der Inhalt der Methode darf nicht mit dem Code-Editor geändert werden.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.splitText = new System.Windows.Forms.SplitContainer();
            this.textCoordinates = new System.Windows.Forms.TextBox();
            this.panelMagnifier = new System.Windows.Forms.Panel();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.panelLetter = new System.Windows.Forms.Panel();
            this.textLetter = new System.Windows.Forms.TextBox();
            this.panelWordSwitch = new System.Windows.Forms.Panel();
            this.checkRaw = new System.Windows.Forms.CheckBox();
            this.radioWord = new System.Windows.Forms.RadioButton();
            this.radioLetter = new System.Windows.Forms.RadioButton();
            this.timerKeys = new System.Windows.Forms.Timer(this.components);
            this.splitMain = new System.Windows.Forms.SplitContainer();
            this.panelScreenC = new System.Windows.Forms.Panel();
            this.buttonRead = new System.Windows.Forms.Button();
            this.buttonAdd = new System.Windows.Forms.Button();
            this.buttonTest = new System.Windows.Forms.Button();
            this.buttonReview = new System.Windows.Forms.Button();
            this.buttonScreenF = new System.Windows.Forms.Button();
            this.buttonScreenB = new System.Windows.Forms.Button();
            this.panelScreen = new System.Windows.Forms.Panel();
            this.textAll = new System.Windows.Forms.TextBox();
            this.panelScreenContainer = new System.Windows.Forms.Panel();
            ((System.ComponentModel.ISupportInitialize)(this.splitText)).BeginInit();
            this.splitText.Panel1.SuspendLayout();
            this.splitText.Panel2.SuspendLayout();
            this.splitText.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.panelWordSwitch.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitMain)).BeginInit();
            this.splitMain.Panel1.SuspendLayout();
            this.splitMain.Panel2.SuspendLayout();
            this.splitMain.SuspendLayout();
            this.panelScreenC.SuspendLayout();
            this.panelScreenContainer.SuspendLayout();
            this.SuspendLayout();
            // 
            // splitText
            // 
            this.splitText.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitText.Location = new System.Drawing.Point(0, 0);
            this.splitText.Name = "splitText";
            this.splitText.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitText.Panel1
            // 
            this.splitText.Panel1.Controls.Add(this.textCoordinates);
            this.splitText.Panel1.Controls.Add(this.panelMagnifier);
            // 
            // splitText.Panel2
            // 
            this.splitText.Panel2.Controls.Add(this.splitContainer1);
            this.splitText.Size = new System.Drawing.Size(555, 582);
            this.splitText.SplitterDistance = 200;
            this.splitText.TabIndex = 1;
            // 
            // textCoordinates
            // 
            this.textCoordinates.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.textCoordinates.Location = new System.Drawing.Point(0, 180);
            this.textCoordinates.Name = "textCoordinates";
            this.textCoordinates.ReadOnly = true;
            this.textCoordinates.Size = new System.Drawing.Size(555, 20);
            this.textCoordinates.TabIndex = 1;
            // 
            // panelMagnifier
            // 
            this.panelMagnifier.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelMagnifier.Location = new System.Drawing.Point(0, 0);
            this.panelMagnifier.Name = "panelMagnifier";
            this.panelMagnifier.Size = new System.Drawing.Size(555, 200);
            this.panelMagnifier.TabIndex = 0;
            this.panelMagnifier.Paint += new System.Windows.Forms.PaintEventHandler(this.panelMagnifier_Paint);
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(0, 0);
            this.splitContainer1.Name = "splitContainer1";
            this.splitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.panelLetter);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.textLetter);
            this.splitContainer1.Panel2.Controls.Add(this.panelWordSwitch);
            this.splitContainer1.Size = new System.Drawing.Size(555, 378);
            this.splitContainer1.SplitterDistance = 189;
            this.splitContainer1.TabIndex = 3;
            // 
            // panelLetter
            // 
            this.panelLetter.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelLetter.Location = new System.Drawing.Point(0, 0);
            this.panelLetter.Name = "panelLetter";
            this.panelLetter.Size = new System.Drawing.Size(555, 189);
            this.panelLetter.TabIndex = 2;
            this.panelLetter.Paint += new System.Windows.Forms.PaintEventHandler(this.panelLetter_Paint);
            // 
            // textLetter
            // 
            this.textLetter.Dock = System.Windows.Forms.DockStyle.Fill;
            this.textLetter.Location = new System.Drawing.Point(0, 0);
            this.textLetter.Multiline = true;
            this.textLetter.Name = "textLetter";
            this.textLetter.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.textLetter.Size = new System.Drawing.Size(555, 133);
            this.textLetter.TabIndex = 1;
            this.textLetter.TextChanged += new System.EventHandler(this.textLetter_TextChanged);
            // 
            // panelWordSwitch
            // 
            this.panelWordSwitch.Controls.Add(this.checkRaw);
            this.panelWordSwitch.Controls.Add(this.radioWord);
            this.panelWordSwitch.Controls.Add(this.radioLetter);
            this.panelWordSwitch.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panelWordSwitch.Location = new System.Drawing.Point(0, 133);
            this.panelWordSwitch.Name = "panelWordSwitch";
            this.panelWordSwitch.Size = new System.Drawing.Size(555, 52);
            this.panelWordSwitch.TabIndex = 4;
            // 
            // checkRaw
            // 
            this.checkRaw.AutoSize = true;
            this.checkRaw.Location = new System.Drawing.Point(36, 30);
            this.checkRaw.Name = "checkRaw";
            this.checkRaw.Size = new System.Drawing.Size(135, 17);
            this.checkRaw.TabIndex = 4;
            this.checkRaw.Text = "Read Box Display Raw";
            this.checkRaw.UseVisualStyleBackColor = true;
            // 
            // radioWord
            // 
            this.radioWord.AutoSize = true;
            this.radioWord.Location = new System.Drawing.Point(192, 7);
            this.radioWord.Name = "radioWord";
            this.radioWord.Size = new System.Drawing.Size(81, 17);
            this.radioWord.TabIndex = 3;
            this.radioWord.TabStop = true;
            this.radioWord.Text = "Word Mode";
            this.radioWord.UseVisualStyleBackColor = true;
            this.radioWord.CheckedChanged += new System.EventHandler(this.radioWord_CheckedChanged);
            // 
            // radioLetter
            // 
            this.radioLetter.AutoSize = true;
            this.radioLetter.Location = new System.Drawing.Point(36, 7);
            this.radioLetter.Name = "radioLetter";
            this.radioLetter.Size = new System.Drawing.Size(101, 17);
            this.radioLetter.TabIndex = 2;
            this.radioLetter.TabStop = true;
            this.radioLetter.Text = "Character Mode";
            this.radioLetter.UseVisualStyleBackColor = true;
            this.radioLetter.CheckedChanged += new System.EventHandler(this.radioLetter_CheckedChanged);
            // 
            // timerKeys
            // 
            this.timerKeys.Enabled = true;
            this.timerKeys.Tick += new System.EventHandler(this.timerKeys_Tick);
            // 
            // splitMain
            // 
            this.splitMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitMain.Location = new System.Drawing.Point(0, 0);
            this.splitMain.Name = "splitMain";
            // 
            // splitMain.Panel1
            // 
            this.splitMain.Panel1.Controls.Add(this.panelScreenContainer);
            this.splitMain.Panel1.Controls.Add(this.panelScreenC);
            // 
            // splitMain.Panel2
            // 
            this.splitMain.Panel2.Controls.Add(this.splitText);
            this.splitMain.Panel2.Controls.Add(this.textAll);
            this.splitMain.Size = new System.Drawing.Size(1063, 582);
            this.splitMain.SplitterDistance = 504;
            this.splitMain.TabIndex = 2;
            // 
            // panelScreenC
            // 
            this.panelScreenC.Controls.Add(this.buttonRead);
            this.panelScreenC.Controls.Add(this.buttonAdd);
            this.panelScreenC.Controls.Add(this.buttonTest);
            this.panelScreenC.Controls.Add(this.buttonReview);
            this.panelScreenC.Controls.Add(this.buttonScreenF);
            this.panelScreenC.Controls.Add(this.buttonScreenB);
            this.panelScreenC.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panelScreenC.Location = new System.Drawing.Point(0, 543);
            this.panelScreenC.Name = "panelScreenC";
            this.panelScreenC.Size = new System.Drawing.Size(504, 39);
            this.panelScreenC.TabIndex = 1;
            // 
            // buttonRead
            // 
            this.buttonRead.Location = new System.Drawing.Point(408, 4);
            this.buttonRead.Name = "buttonRead";
            this.buttonRead.Size = new System.Drawing.Size(75, 23);
            this.buttonRead.TabIndex = 2;
            this.buttonRead.Text = "Read";
            this.buttonRead.UseVisualStyleBackColor = true;
            this.buttonRead.Click += new System.EventHandler(this.buttonRead_Click);
            // 
            // buttonAdd
            // 
            this.buttonAdd.Location = new System.Drawing.Point(165, 4);
            this.buttonAdd.Name = "buttonAdd";
            this.buttonAdd.Size = new System.Drawing.Size(75, 23);
            this.buttonAdd.TabIndex = 0;
            this.buttonAdd.Text = "Add";
            this.buttonAdd.UseVisualStyleBackColor = true;
            this.buttonAdd.Click += new System.EventHandler(this.buttonAdd_Click);
            // 
            // buttonTest
            // 
            this.buttonTest.Location = new System.Drawing.Point(327, 4);
            this.buttonTest.Name = "buttonTest";
            this.buttonTest.Size = new System.Drawing.Size(75, 23);
            this.buttonTest.TabIndex = 1;
            this.buttonTest.Text = "Test";
            this.buttonTest.UseVisualStyleBackColor = true;
            this.buttonTest.Click += new System.EventHandler(this.buttonTest_Click);
            // 
            // buttonReview
            // 
            this.buttonReview.Location = new System.Drawing.Point(246, 4);
            this.buttonReview.Name = "buttonReview";
            this.buttonReview.Size = new System.Drawing.Size(75, 23);
            this.buttonReview.TabIndex = 0;
            this.buttonReview.Text = "Review";
            this.buttonReview.UseVisualStyleBackColor = true;
            this.buttonReview.Click += new System.EventHandler(this.buttonReview_Click);
            // 
            // buttonScreenF
            // 
            this.buttonScreenF.Location = new System.Drawing.Point(84, 4);
            this.buttonScreenF.Name = "buttonScreenF";
            this.buttonScreenF.Size = new System.Drawing.Size(75, 23);
            this.buttonScreenF.TabIndex = 0;
            this.buttonScreenF.Text = "-->";
            this.buttonScreenF.UseVisualStyleBackColor = true;
            this.buttonScreenF.Click += new System.EventHandler(this.buttonScreenF_Click);
            // 
            // buttonScreenB
            // 
            this.buttonScreenB.Location = new System.Drawing.Point(3, 4);
            this.buttonScreenB.Name = "buttonScreenB";
            this.buttonScreenB.Size = new System.Drawing.Size(75, 23);
            this.buttonScreenB.TabIndex = 0;
            this.buttonScreenB.Text = "<--";
            this.buttonScreenB.UseVisualStyleBackColor = true;
            this.buttonScreenB.Click += new System.EventHandler(this.buttonScreenB_Click);
            // 
            // panelScreen
            // 
            this.panelScreen.Location = new System.Drawing.Point(0, 0);
            this.panelScreen.Name = "panelScreen";
            this.panelScreen.Size = new System.Drawing.Size(450, 1200);
            this.panelScreen.TabIndex = 0;
            this.panelScreen.Paint += new System.Windows.Forms.PaintEventHandler(this.panelScreen_Paint);
            this.panelScreen.MouseClick += new System.Windows.Forms.MouseEventHandler(this.panelScreen_MouseClick);
            this.panelScreen.MouseMove += new System.Windows.Forms.MouseEventHandler(this.panelScreen_MouseMove);
            // 
            // textAll
            // 
            this.textAll.Dock = System.Windows.Forms.DockStyle.Fill;
            this.textAll.Location = new System.Drawing.Point(0, 0);
            this.textAll.Multiline = true;
            this.textAll.Name = "textAll";
            this.textAll.Size = new System.Drawing.Size(555, 582);
            this.textAll.TabIndex = 0;
            // 
            // panelScreenContainer
            // 
            this.panelScreenContainer.AutoScroll = true;
            this.panelScreenContainer.Controls.Add(this.panelScreen);
            this.panelScreenContainer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelScreenContainer.Location = new System.Drawing.Point(0, 0);
            this.panelScreenContainer.Name = "panelScreenContainer";
            this.panelScreenContainer.Size = new System.Drawing.Size(504, 543);
            this.panelScreenContainer.TabIndex = 2;
            // 
            // FrmMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1063, 582);
            this.Controls.Add(this.splitMain);
            this.Name = "FrmMain";
            this.Text = "Form1";
            this.splitText.Panel1.ResumeLayout(false);
            this.splitText.Panel1.PerformLayout();
            this.splitText.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitText)).EndInit();
            this.splitText.ResumeLayout(false);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            this.splitContainer1.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.panelWordSwitch.ResumeLayout(false);
            this.panelWordSwitch.PerformLayout();
            this.splitMain.Panel1.ResumeLayout(false);
            this.splitMain.Panel2.ResumeLayout(false);
            this.splitMain.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitMain)).EndInit();
            this.splitMain.ResumeLayout(false);
            this.panelScreenC.ResumeLayout(false);
            this.panelScreenContainer.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.SplitContainer splitText;
        private System.Windows.Forms.Timer timerKeys;
        private System.Windows.Forms.SplitContainer splitMain;
        private System.Windows.Forms.Panel panelScreen;
        private System.Windows.Forms.Panel panelMagnifier;
        private System.Windows.Forms.TextBox textCoordinates;
        private System.Windows.Forms.Panel panelScreenC;
        private System.Windows.Forms.Button buttonScreenF;
        private System.Windows.Forms.Button buttonScreenB;
        private System.Windows.Forms.TextBox textLetter;
        private System.Windows.Forms.Panel panelLetter;
        private System.Windows.Forms.Button buttonReview;
        private System.Windows.Forms.Button buttonAdd;
        private System.Windows.Forms.Button buttonTest;
        private System.Windows.Forms.Button buttonRead;
        private System.Windows.Forms.TextBox textAll;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.Panel panelWordSwitch;
        private System.Windows.Forms.RadioButton radioWord;
        private System.Windows.Forms.RadioButton radioLetter;
        private System.Windows.Forms.CheckBox checkRaw;
        private System.Windows.Forms.Panel panelScreenContainer;
    }
}

