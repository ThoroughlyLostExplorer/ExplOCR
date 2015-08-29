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
            previousState = WindowState;
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

            ocrReader.StitchPrevious = checkStitch.Checked;
            LibExplOCR.ProcessImage(ocrReader, bmp, out bmpStructure, out bmpHeatmap);
            imageDisplay.Image = bmpHeatmap;
            ocrReader.StitchPrevious = false;
            textXML = OutputConverter.GetDataXML(ocrReader.Items);
            textShort.Text = OutputConverter.GetDataText(ocrReader.Items);

            if (!checkStitch.Checked && textSystem.Text != "")
            {
                textBody.Text = OutputConverter.GetDataBodyCode(textSystem.Text, ocrReader.Items);
            }

            Invalidate();
            Update();
            Refresh();

            if (checkStitch.Checked)
            {
                checkStitch.Checked = false;
                bmpOther.Add(bmp.Clone() as Bitmap);
            }
            else
            {
                bmpArchive = bmp.Clone() as Bitmap;
                bmpOther.Clear();
            }
        }

        private void SaveToArchive()
        {
            if (checkStitch.Checked)
            {
                // If stiching several pictures together, save only on last one.
                ConditionalMinimize();
                return;
            }

            int nextFree = GetUnusedFileNumber();
            List<string> archiveNames = new List<string>();
            if (bmpArchive != null)
            {
                bmpArchive.Save(PathHelpers.BuildScreenFilename(nextFree));
                archiveNames.Add(Path.GetFileName(PathHelpers.BuildScreenFilename(nextFree)));
            }
            for (int j = 0; j < bmpOther.Count; j++)
            {
                string name = PathHelpers.BuildScreenFilename(nextFree);
                name = Path.GetFileNameWithoutExtension(name) + "_" + (j + 1).ToString() + Path.GetExtension(name);
                bmpOther[j].Save(Path.Combine(PathHelpers.BuildScreenDirectory(), name));
                archiveNames.Add(Path.GetFileName(name));
            }

            LibExplOCR.SaveInfo(textXML, textSystem.Text, textBody.Text, textDescription.Text, textCategories.Text, archiveNames);
            ConditionalMinimize();
        }

        // Get next available number, without using gaps.
        private static int GetUnusedFileNumber()
        {
            int max = -1;
            Directory.CreateDirectory(PathHelpers.BuildScreenDirectory());
            string extension = Path.GetExtension(PathHelpers.BuildScreenFilename(0));
            string[] files = Directory.GetFiles(PathHelpers.BuildScreenDirectory(), "*" + extension);
            // Get highest number in use to skip gaps.
            foreach (string file in files)
            {
                max = Math.Max(max, PathHelpers.GetFileNumber(Path.GetFileName(file)));
            }

            for (int nextFree = max+1; ; nextFree++)
            {
                if (!File.Exists(PathHelpers.BuildScreenFilename(nextFree)))
                {
                    return nextFree;
                }
            }
        }

        // Minimize, but not when reading an existing file.
        private void ConditionalMinimize()
        {
            if (isTrueScreenshot)
            {
                previousState = WindowState;
                WindowState = FormWindowState.Minimized;
                isTrueScreenshot = false;
            }
        }

        // Read an existing file.
        private void OpenFile()
        {
            bool processing = false;
            using (OpenFileDialog fd = new OpenFileDialog())
            {
                if (fd.ShowDialog() != System.Windows.Forms.DialogResult.OK)
                {
                    return;
                }
                try
                {
                    using (Bitmap bitmap = new Bitmap(fd.FileName))
                    {

                        Rectangle r = new Rectangle(ExplOCR.Properties.Settings.Default.ScreenshotX, ExplOCR.Properties.Settings.Default.ScreenshotY,
                            ExplOCR.Properties.Settings.Default.ScreenshotW, ExplOCR.Properties.Settings.Default.ScreenshotH);
                        r.Width = Math.Min(r.Width, bitmap.Width - r.X);
                        r.Height = Math.Min(r.Height, bitmap.Height - r.Y);
                        Bitmap b = new Bitmap(r.Width, r.Height);
                        using (Graphics g = Graphics.FromImage(b))
                        {
                            g.DrawImage(bitmap, new Rectangle(0, 0, r.Width, r.Height), r, GraphicsUnit.Pixel);
                        }
                        processing = true;
                        isTrueScreenshot = false;
                        ProcessScreenshot(b);
                    }
                }
                catch
                {
                    if (processing)
                    {
                        MessageBox.Show("Couldn't process");
                    }
                    else
                    {
                        MessageBox.Show("Couldn't load");
                    }
                }
            }
        }

        private void ShowTableForm()
        {
            try
            {
                timerTick.Enabled = false;
                using (FrmTable frm = new FrmTable())
                {
                    frm.ShowDialog();
                }
            }
            finally
            {
                timerTick.Enabled = true;
            }
        }


        #region Menu Items

        private void miFileOpen_Click(object sender, EventArgs e)
        {
            OpenFile();
        }

        private void miFileSave_Click(object sender, EventArgs e)
        {
            SaveToArchive();
        }

        private void miFileExit_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void miTableDisplay_Click(object sender, EventArgs e)
        {
            ShowTableForm();
        }

        private void miConfigNetLogDir_Click(object sender, EventArgs e)
        {
            using (Form dlg = new FrmNetLogDlg())
            {
                dlg.ShowDialog();
            }
        }

        #endregion

        private void buttonSave_Click(object sender, EventArgs e)
        {
            SaveToArchive();
        }

        private void buttonEdit_Click(object sender, EventArgs e)
        {
            using (Form dlg = new FrmQuickEdit())
            {
                dlg.ShowDialog();
            }
        }

        private void buttonBrowse_Click(object sender, EventArgs e)
        {
            ShowTableForm();
        }

        private void timer_Tick(object sender, EventArgs e)
        {
            if (Control.ModifierKeys != (Keys.Shift | Keys.Alt | Keys.Control))
            {
                return;
            }

            using (Bitmap bmp = MakeScreenshot())
            {
                isTrueScreenshot = true;
                ProcessScreenshot(bmp);

                WindowState = previousState;
                textBody.SelectAll();
                textBody.Focus();
                Activate();
            }
        }

        private void EditBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == 13)
            {
                SaveToArchive();
            }
        }

        #region Private Variables

        OcrReader ocrReader;
        Bitmap bmpArchive;
        List<Bitmap> bmpOther = new List<Bitmap>();
        Bitmap bmpStructure;
        Bitmap bmpHeatmap;
        string textXML;
        FormWindowState previousState;
        bool isTrueScreenshot = false;

        #endregion

    }
}
