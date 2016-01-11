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
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ExplOCR
{
    public partial class FrmMain : Form
    {
        public FrmMain()
        {
            /*string[] files = Directory.GetFiles(PathHelpers.BuildTeachDirectory());
            Directory.CreateDirectory(PathHelpers.BuildKnowledgeDirectory("headlines"));
            foreach (string file in files)
            {
                string[] lines = File.ReadAllLines(file);
                for (int i = 0; i < lines.Length; i++)
                {
                    LetterInfo info = LetterInfo.ReadLetterInfoLine(lines[i]);
                    info.X = (int)Math.Round(info.X * 15.0 / 22.0);
                    byte[] bytes15 = new byte[15 * 22];
                    byte[] bytes22 = Convert.FromBase64String(info.Base64);
                    for (int j = 0; j < 15; j++)
                    {
                        for (int k = 0; k < 15; k++)
                        {
                            bytes15[j * 15 + k] = bytes22[j * 22 + k];
                        }
                    }
                    info.Base64 = Convert.ToBase64String(bytes15);
                    lines[i] = info.Base64 + "," + info.Screen.ToString() + "," + info.X.ToString() + "," + info.Y.ToString() + "," + info.Char.ToString();
                }
                string tmp = PathHelpers.BuildKnowledgeFilename("headlines", Path.GetFileNameWithoutExtension(file));
                File.WriteAllLines(tmp, lines);
            }*/

            InitializeComponent();
            UpdateUI();
            PrepareBitmaps();

            if (ocrReader != null) ocrReader.Dispose();
            TrainingConfig.TrainNN(out ocrReader);

            //RebuildKnowledge();
            //checkRaw.Checked = true;
            //DoAutoTest();
            //File.WriteAllLines(Path.Combine(PathHelpers.BuildAutoTestDirectory(), "debug.log"), debugStrings);
            currentScreen = 0;
            usingWords = true;

            FrmDebug frm = new FrmDebug();
            frm.Show();
        }

        private void UpdateUI()
        {
            if (review == null)
            {
                buttonAdd.Enabled = false;
                buttonTest.Enabled = !doTest;
                buttonReview.Enabled = true;
                buttonScreenB.Enabled = File.Exists(PathHelpers.BuildScreenFilename(currentScreen - 1));
                string tmp = PathHelpers.BuildScreenFilename(currentScreen + 1);
                buttonScreenF.Enabled = File.Exists(PathHelpers.BuildScreenFilename(currentScreen + 1));
            }
            else
            {
                buttonTest.Enabled = true;
                buttonReview.Enabled = true;
                buttonAdd.Enabled = false;
                buttonScreenB.Enabled = currentLetter > 0;
                buttonScreenF.Enabled = currentLetter+1 < review.Length;
            }
            radioWord.Checked = usingWords;
            radioLetter.Checked = !usingWords;
            panelScreen.Invalidate();
            panelScreen.Refresh();
            textLetter.Focus();
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

        private void SetScreen(int screen)
        {
            currentScreen = screen;
            PrepareBitmaps();
            UpdateUI();
            RedrawAll();
            if (doRead)
            {
                ocrReader.RawMode = checkRaw.Checked;
                ocrReader.ReadPage(new Bytemap(baseBmp), new Bytemap(drawBmpRaw), new Bytemap(drawBmpRawSplit), pageSections);
                textAll.Text = OutputConverter.GetDataText(ocrReader.Items);
            }
        }

        private unsafe void timerKeys_Tick(object sender, EventArgs e)
        {
            if (Control.ModifierKeys != (Keys.Shift | Keys.Alt | Keys.Control))
            {
                return;
            }

            using (Bitmap bmp = MakeScreenshot())
            {
                
                Directory.CreateDirectory(PathHelpers.BuildAutoTestDirectory());
                for (int i = 0; ; i++)
                {
                    string file = PathHelpers.BuildAutoTestFilename(i);
                    if (File.Exists(file))
                    {
                        continue;
                    }
                    bmp.Save(file);
                    break;
                }

                PrepareBitmaps(bmp);
                UpdateUI();
                RedrawAll();
                if (doRead)
                {
                    ocrReader.RawMode = checkRaw.Checked;
                    ocrReader.ReadPage(new Bytemap(baseBmp), new Bytemap(drawBmpRaw), new Bytemap(drawBmpRawSplit), pageSections);
                    textAll.Text = OutputConverter.GetDataText(ocrReader.Items);
                }
                Activate();
            }            
        }

        private void buttonScreenB_Click(object sender, EventArgs e)
        {
            if (review == null)
            {
                SetScreen(currentScreen-1);
            }
            else
            {
                currentLetter--;
                DisplayItem();
            }

        }

        private void buttonScreenF_Click(object sender, EventArgs e)
        {
            if (review == null)
            {
                SetScreen(currentScreen + 1);
            }
            else
            {
                currentLetter++;
                DisplayItem();
            }
        }

        private void panelScreen_MouseMove(object sender, MouseEventArgs e)
        {
            lastMousePos = e.Location;
            textCoordinates.Text = "("+lastMousePos.X.ToString()+","+lastMousePos.Y+")";
            panelMagnifier.Invalidate();
            panelMagnifier.Update();
        }

        private void panelScreen_MouseClick(object sender, MouseEventArgs e)
        {
            if (baseBmp == null)
            {
                return;
            }

            if (usingWords)
            {
                TeachLetters.ProcessWords(baseBmp, currentScreen, textLetter.Text, e.Location, pageSections);
                return;
            }

            selected = new Rectangle();
            foreach (Rectangle letter in pageSections.AllLetters)
            {
                if (letter.Contains(e.Location))
                {
                    selected = letter;
                    break;
                }
            }

            clickPos = e.Location;

            if (!selected.IsEmpty && drawBmp != null)
            {
                using (Graphics g = Graphics.FromImage(drawBmp))
                {
                    g.FillRectangle(Brushes.Red, selected);
                }
            }

            if (selected.Width == 0 || selected.Height == 0)
            {
                return;
            }

            Bitmap reduced = baseBmp.Clone(selected, baseBmp.PixelFormat);
            Bytemap reducedBytes = ImageLetters.ExtractBytes(reduced);
            Size size = new System.Drawing.Size(Properties.Settings.Default.DimensionX, Properties.Settings.Default.DimensionY);
            selectedBytes = ImageLetters.CopyLetter(reducedBytes, new Rectangle(0,0,selected.Width, selected.Height), size).Bytes;

            panelLetter.Update();
            panelLetter.Refresh();

            panelScreen.Invalidate();
            panelScreen.Update();
        }

        #region Paint and Paint Support

        private void panelScreen_Paint(object sender, PaintEventArgs e)
        {
            e.Graphics.FillRectangle(Brushes.Black, panelScreen.ClientRectangle);
            if (drawBmp != null)
            {
                e.Graphics.DrawImage(drawBmp, new Rectangle(0, 0, drawBmp.Width, drawBmp.Height));
            }
            List<Rectangle> masks = GenerateLetterMasks(currentScreen, pageSections.AllLetters);
            foreach (Rectangle letter in masks)
            {
                e.Graphics.FillRectangle(Brushes.Red, letter);
            }
            foreach (Rectangle letter in pageSections.AllLetters)
            {
                e.Graphics.DrawRectangle(Pens.Green, letter);
            }
        }

        private void panelMagnifier_Paint(object sender, PaintEventArgs e)
        {
            Rectangle r = new Rectangle(lastMousePos.X - 20, lastMousePos.Y - 20, 40, 40);
            e.Graphics.DrawImage(baseBmp, new Rectangle(0, 0, 160, 160), r, GraphicsUnit.Pixel);
            e.Graphics.DrawImage(drawBmp, new Rectangle(180, 0, 160, 160), r, GraphicsUnit.Pixel);
            e.Graphics.DrawImage(drawBmpRaw, new Rectangle(360, 0, 160, 160), r, GraphicsUnit.Pixel);
            e.Graphics.DrawLine(Pens.Red, 80, 70, 80, 90);
            e.Graphics.DrawLine(Pens.Red, 70, 80, 90, 80);
            e.Graphics.DrawLine(Pens.Red, 180 + 80, 70, 180 + 80, 90);
            e.Graphics.DrawLine(Pens.Red, 180 + 70, 80, 180 + 90, 80);
            e.Graphics.DrawLine(Pens.Red, 360 + 80, 70, 360 + 80, 90);
            e.Graphics.DrawLine(Pens.Red, 360 + 70, 80, 360 + 90, 80);
        }

        private static List<Rectangle> GenerateLetterMasks(int screen, List<Rectangle> letters)
        {
            string[] lines;
            List<Rectangle> masks = new List<Rectangle>();
            if (!File.Exists(PathHelpers.BuildTeachBaseFilename()))
            {
                lines = new string[0];
            }
            else
            {
                lines = File.ReadAllLines(PathHelpers.BuildTeachBaseFilename());
            }
            for (int i = 0; i < lines.Length; i++)
            {
                LetterInfo letterInfo = LetterInfo.ReadLetterInfoLine(lines[i]);
                if (letterInfo.Invalid || letterInfo.Screen != screen)
                {
                    continue;
                }
                foreach (Rectangle r in letters)
                {
                    if (r.Contains(letterInfo.X, letterInfo.Y)) masks.Add(r);
                }
            }
            return masks;
        }

        #endregion

        private void PrepareBitmaps()
        {
            Bitmap bmp = LoadImageFile(currentScreen);
            if (bmp == null)
            {
                return;
            }
            PrepareBitmaps(bmp);
            bmp.Dispose();
        }

        private void PrepareBitmaps(Bitmap bmp)
        {
            DisposeOldImages();

            Bitmap binary, binarySplit;
            pageSections = LibExplOCR.PrepareBitmaps(bmp, out baseBmp, out binary, out binarySplit);

            drawBmpRaw = new Bitmap(binary);
            drawBmpRawSplit = new Bitmap(binarySplit);
            drawBmp = new Bitmap(binary);
            LibExplOCR.AnnotatePageStructure(drawBmp, pageSections);

            binary.Dispose();
        }

        private void DisposeOldImages()
        {
            if (baseBmp != null)
            {
                baseBmp.Dispose();
            }
            if (drawBmp != null)
            {
                drawBmp.Dispose();
            }
        }

        private Bitmap LoadImageFile(int screen)
        {
            string file = PathHelpers.BuildScreenFilename(screen);
            return ImageFiles.LoadImageFile(file);
        }

        void DisplayItem()
        {
            if (review == null) return;

            LetterInfo info = LetterInfo.ReadLetterInfoLine(review[currentLetter]);
            if (info.Invalid)
            {
                return;
            }
            selectedBytes = Convert.FromBase64String(info.Base64);
            currentScreen = info.Screen;
            lastMousePos = new Point(info.X, info.Y);
            displayChar = info.Char;

            PrepareBitmaps();
            UpdateUI();
            RedrawAll();
        }

        private void textLetter_TextChanged(object sender, EventArgs e)
        {
            if (usingWords) return;

            if (textLetter.Text != "" && !selected.IsEmpty)
            {
                char c = textLetter.Text[0];
                TeachLetters.ProcessLetter(baseBmp, currentScreen, c, selected);
            }
            textLetter.Text = "";
            selected = new Rectangle();

        }

        private void panelLetter_Paint(object sender, PaintEventArgs e)
        {
            if(selectedBytes==null) return;

            for (int i = 0; i < Properties.Settings.Default.DimensionY; i++)
            {
                for (int j = 0; j < Properties.Settings.Default.DimensionX; j++)
                {
                    Brush b = new SolidBrush(
                        Color.FromArgb(255, selectedBytes[i * Properties.Settings.Default.DimensionX + j], 
                                            selectedBytes[i * Properties.Settings.Default.DimensionX + j], 
                                            selectedBytes[i * Properties.Settings.Default.DimensionX + j]));
                        e.Graphics.FillRectangle(b, j * 10, i * 10, 10, 10);
                        b.Dispose();
                }
            }
            Font use = new System.Drawing.Font(FontFamily.GenericSansSerif, 24);
            e.Graphics.DrawString(""+displayChar, use, Brushes.Black, new Point(150,0));
        }

        private void DoAutoTest()
        {
            string[] test = Directory.GetFiles(PathHelpers.BuildAutoTestDirectory(), "*.png");
            string results = "test_" + DateTime.Now.ToString("s");
            results = results.Replace(":", "-");
            if (checkRaw.Checked) results += "_raw";
            results = Path.Combine(PathHelpers.BuildAutoTestDirectory(), results);
            Directory.CreateDirectory(results);
            StringBuilder total = new StringBuilder();
            for(int i=0; i < test.Length; i++)
            {
                total.AppendLine("File: " + PathHelpers.BuildAutoTestFilename(i));
                currentScreen = i;
                if (!File.Exists(PathHelpers.BuildAutoTestFilename(i))) continue;
                ocrReader.RawMode = checkRaw.Checked;
                LibExplOCR.ProcessImageFile(ocrReader, PathHelpers.BuildAutoTestFilename(i));
                string s = OutputConverter.GetDataTextClassic(ocrReader.Items);
                string output = Path.Combine(results, Path.GetFileNameWithoutExtension(PathHelpers.BuildAutoTestFilename(i)) + ".txt");
                File.WriteAllText(output, s);
                total.AppendLine(s);
            }
            File.WriteAllText(Path.Combine(results, "all.txt"), total.ToString()); ;
        }

        private void RebuildKnowledge(string type)
        {
            string[] knowledge = Directory.GetFiles(PathHelpers.BuildKnowledgeDirectory(type), "*.txt");
            string backup = "backup_" + DateTime.Now;
            backup = backup.Replace(":", "_");
            backup = backup.Replace(" ", "_");
            backup = Path.Combine(PathHelpers.BuildKnowledgeDirectory(type), backup);
            Directory.CreateDirectory(backup);
            foreach (string file in knowledge)
            {
                File.Move(file, Path.Combine(backup, Path.GetFileName(file)));
            }

            foreach (string file in knowledge)
            {
                RebuildKnowledgeFile(file, Path.Combine(backup, Path.GetFileName(file))); 
            }
        }

        private void RebuildKnowledgeFile(string outFile, string inFile)
        {
            string[] content = File.ReadAllLines(inFile);
            List<string> output = new List<string>();
            foreach (string item in content)
            {
                LetterInfo info = LetterInfo.ReadLetterInfoLine(item);
                if (info.Invalid)
                {
                    return;
                }
                SetScreen(info.Screen);

                Rectangle frame = new Rectangle();
                foreach (Rectangle letter in pageSections.AllLetters)
                {
                    if (letter.Contains(info.X,info.Y))
                    {
                        frame = letter;
                        break;
                    }
                }
                byte[] letterBytes = ImageLetters.GetLetterImage(baseBmp, frame, new Size(Properties.Settings.Default.DimensionX, Properties.Settings.Default.DimensionY));
                string s = LetterInfo.WriteLetterInfoLine(info.Char, frame, currentScreen, Convert.ToBase64String(letterBytes));
                output.Add(s);
            }
            File.WriteAllLines(outFile, output);
        }

        private void RedrawAll()
        {
            panelScreen.Invalidate();
            panelScreen.Update();
            panelMagnifier.Invalidate();
            panelMagnifier.Update();
            panelLetter.Invalidate();
            panelLetter.Update();
        }

        private void buttonAdd_Click(object sender, EventArgs e)
        {
            review = null;
            doTest = false;
            UpdateUI();
            PrepareBitmaps();
            RedrawAll();
        }

        private void buttonReview_Click(object sender, EventArgs e)
        {
            if (!File.Exists(PathHelpers.BuildTeachBaseFilename()))
            {
                review = new string[0];
            }
            else
            {
                review = File.ReadAllLines(PathHelpers.BuildTeachBaseFilename());
                currentLetter = -1;
                buttonScreenF_Click(this, e);
            }
        }

        private void buttonTest_Click(object sender, EventArgs e)
        {
            review = null;
            doTest = true;
            doRead = false;
            if (ocrReader != null) ocrReader.Dispose();
            TrainingConfig.TrainNN(out ocrReader);
            UpdateUI();
            PrepareBitmaps();
            RedrawAll();
        }

        private void buttonRead_Click(object sender, EventArgs e)
        {
            if (!doRead)
            {
                doTest = false;
                doRead = true;
                textAll.BringToFront();
            }
            else
            {
                doRead = false;
                textAll.SendToBack();
            }
            if (ocrReader != null) ocrReader.Dispose();
            TrainingConfig.TrainNN(out ocrReader);
            UpdateUI();
            PrepareBitmaps();
            RedrawAll();
            ocrReader.RawMode = checkRaw.Checked;
            ocrReader.ReadPage(new Bytemap(baseBmp), new Bytemap(drawBmpRaw),  new Bytemap(drawBmpRawSplit), pageSections);
            textAll.Text = OutputConverter.GetDataTextClassic(ocrReader.Items);
        }

        private void radioLetter_CheckedChanged(object sender, EventArgs e)
        {
            usingWords = false;
        }

        private void radioWord_CheckedChanged(object sender, EventArgs e)
        {
            usingWords = true;
        }

        public static void DebugLog(char c, Rectangle r, byte[] letterBytes)
        {
            debugStrings.Add(LetterInfo.WriteLetterInfoLine(c, r, currentScreen, Convert.ToBase64String(letterBytes)));
        }

        PageSections pageSections;
        OcrReader ocrReader;
        Point clickPos;
        Point lastMousePos;
        Bitmap baseBmp;
        Bitmap drawBmp;
        Bitmap drawBmpRaw;
        Bitmap drawBmpRawSplit;
        Rectangle selected = new Rectangle();
        byte[] selectedBytes;
        List<Rectangle> selectedLetters = new List<Rectangle>();
        static int currentScreen = 0;
        int currentLetter = 0;
        char displayChar = ' ';
        string[] review;
        bool doTest = false;
        bool doRead = false;
        bool usingWords = true;
        static List<string> debugStrings = new List<string>();

        private void textLetter_KeyDown(object sender, KeyEventArgs e)
        {
            if (buttonScreenB.Enabled &&  e.KeyCode == Keys.F1 && review == null)
            {
                SetScreen(currentScreen - 1);
            }
            if (buttonScreenF.Enabled && e.KeyCode == Keys.F2 && review == null)
            {
                SetScreen(currentScreen + 1);
            }
        }
    }
}
