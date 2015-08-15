// Copyright 2015 by the person represented as ThoroughlyLostExplorer on GitHub
//
//   Licensed under the Apache License, Version 2.0 (the "License");
//   you may not use this file except in compliance with the License.
//   You may obtain a copy of the License at
//
//       http://www.apache.org/licenses/LICENSE-2.0
//
//   Unless required by applicable law or agreed to in writing, software
//   distributed under the License is distributed on an "AS IS" BASIS,
//   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//   See the License for the specific language governing permissions and
//   limitations under the License.
////////////////////////////////////////////////////////////////////////////////

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
using System.Xml.Serialization;

namespace ExplOCR
{
    public partial class FrmUser : Form
    {
        public FrmUser()
        {
            ocrReader = LibExplOCR.CreateOcrReader();
            InitializeComponent();
        }
        
        private Bitmap MakeScreenshot()
        {
            //Bitmap b = new Bitmap(SystemInformation.VirtualScreen.Width, SystemInformation.VirtualScreen.Height);
            Bitmap b = new Bitmap(ExplOCR.Properties.Settings.Default.ScreenshotW, ExplOCR.Properties.Settings.Default.ScreenshotH);
            Graphics g = Graphics.FromImage(b);
            g.CopyFromScreen(ExplOCR.Properties.Settings.Default.ScreenshotX, ExplOCR.Properties.Settings.Default.ScreenshotY, 0, 0, b.Size);
            g.Dispose();
            return b;
        }

        private void ProcessScreenshot(Bitmap bmp)
        {
            textSystem.Text = LibExplOCR.GetCurrentSystemName(Properties.Settings.Default.NetLogDir);
            if (string.IsNullOrEmpty(textSystem.Text))
            {
                errorProvider.SetError(textSystem, "Possibly you need to configure your NetLog directory in menu Configuration / NetLog Directory");
            }
            else
            {
                errorProvider.SetError(textSystem, "");
            }

            LibExplOCR.ProcessImage(ocrReader, bmp, false, out bmpDisplay);
            textXML.Text = ocrReader.GetDataXML();
            textBox2.Text = ocrReader.GetDataText();
            Invalidate();
            Update();
            Refresh();
            bmpArchive = bmp.Clone() as Bitmap;

            WindowState = FormWindowState.Normal;
            textBody.SelectAll();
            textBody.Focus();
            Activate();
        }

        private void SaveToArchive(Bitmap archive)
        {
            string archiveName = "";
            Directory.CreateDirectory(PathHelpers.BuildScreenDirectory());
            for (int i = 0; ; i++)
            {
                if (File.Exists(PathHelpers.BuildScreenFilename(i)))
                {
                    continue;
                }
                if (archive != null)
                {
                    archive.Save(PathHelpers.BuildScreenFilename(i));
                    archiveName = Path.GetFileName(PathHelpers.BuildScreenFilename(i));
                }
                break;
            }

            LibExplOCR.SaveInfo(textXML.Text, textSystem.Text, textBody.Text, archiveName);
            WindowState = FormWindowState.Minimized;
        }

        private void OpenFile()
        {
            Bitmap bitmap;
            using (OpenFileDialog fd = new OpenFileDialog())
            {
                if (fd.ShowDialog() != System.Windows.Forms.DialogResult.OK)
                {
                    return;
                }
                try
                {
                    bitmap = new Bitmap(fd.FileName);
                }
                catch
                {
                    MessageBox.Show("Couldn't load");
                }
                try
                {
                    LibExplOCR.ProcessImageFile(ocrReader, fd.FileName, false, out bmpDisplay);
                    textXML.Text = ocrReader.GetDataXML();
                    textBox2.Text = ocrReader.GetDataText();
                    Invalidate();
                    Update();
                    Refresh();
                }
                catch
                {
                    MessageBox.Show("Couldn't process");
                }
            }
        }

        private void miFileOpen_Click(object sender, EventArgs e)
        {
            OpenFile();
        }

        private void miFileSave_Click(object sender, EventArgs e)
        {
            SaveToArchive(bmpArchive);
        }

        private void miFileExit_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void miTableDisplay_Click(object sender, EventArgs e)
        {
            TransferItem[][] array;
            XmlSerializer ser = new XmlSerializer(typeof(TransferItem[][]));
            try
            {
                using (FileStream fs = File.OpenRead(PathHelpers.BuildSaveFilename()))
                {
                    array = ser.Deserialize(fs) as TransferItem[][];
                }
            }
            catch
            {
                array = new TransferItem[0][];
            }


            using (FrmTable frm = new FrmTable())
            {
                frm.SetValues(array);
                frm.ShowDialog();
            }
        }

        private void panelScreen_Paint(object sender, PaintEventArgs e)
        {
            e.Graphics.FillRectangle(Brushes.Black, panelScreen.ClientRectangle);
            if (bmpDisplay == null)
            {
                return;
            }
            e.Graphics.DrawImage(bmpDisplay, 0, 0, bmpDisplay.Width, bmpDisplay.Height);
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (Control.ModifierKeys != (Keys.Shift | Keys.Alt | Keys.Control))
            {
                return;
            }

            using (Bitmap bmp = MakeScreenshot())
            {
                ProcessScreenshot(bmp);
            }
        }

        private void buttonSave_Click(object sender, EventArgs e)
        {
            SaveToArchive(bmpArchive);
        }

        private void EditBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == 13)
            {
                SaveToArchive(bmpArchive);
            }
        }

        OcrReader ocrReader;
        Bitmap bmpArchive;
        Bitmap bmpDisplay;

        private void miConfigNetLogDir_Click(object sender, EventArgs e)
        {
            using (Form dlg = new FrmNetLogDlg())
            {
                dlg.ShowDialog();
            }
        }
    }
}
