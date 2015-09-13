using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ExplOCR
{
    public partial class FrmDirectoriesDlg : Form
    {
        public FrmDirectoriesDlg()
        {
            InitializeComponent();

            textArchive.Text = PathHelpers.BuildScreenDirectory();
            textDB.Text = PathHelpers.BuildUserSaveDirectory();
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            base.OnFormClosing(e);

            if (DialogResult == System.Windows.Forms.DialogResult.Cancel)
            {
                return;
            }

            errorProvider.SetError(textArchive, null);
            errorProvider.SetError(textDB, null);

            if (!Directory.Exists(textArchive.Text))
            {
                errorProvider.SetError(textArchive, "Directory doesn't exist.");
                e.Cancel = true;
            }
            if (!Directory.Exists(textDB.Text))
            {
                errorProvider.SetError(textDB, "Directory doesn't exist.");
                e.Cancel = true;
            }
        }

        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            base.OnFormClosed(e);

            PathHelpers.ConfigureScreenDirectory(textArchive.Text);
            PathHelpers.ConfigureUserSaveDirectory(textDB.Text);
        }

        private void buttonBrowseDB_Click(object sender, EventArgs e)
        {
            using (FolderBrowserDialog dlg = new FolderBrowserDialog())
            {
                if (Directory.Exists(textDB.Text))
                {
                    dlg.SelectedPath = textDB.Text;
                }
                if (dlg.ShowDialog() != System.Windows.Forms.DialogResult.OK)
                {
                    return;
                }
                textDB.Text = dlg.SelectedPath;
            }
        }

        private void buttonBrowseArchive_Click(object sender, EventArgs e)
        {
            using (FolderBrowserDialog dlg = new FolderBrowserDialog())
            {
                if (Directory.Exists(textArchive.Text))
                {
                    dlg.SelectedPath = textArchive.Text;
                }
                if (dlg.ShowDialog() != System.Windows.Forms.DialogResult.OK)
                {
                    return;
                }
                textArchive.Text = dlg.SelectedPath;
            }
        }
    }
}
