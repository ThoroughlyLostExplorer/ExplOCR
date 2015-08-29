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

namespace ExplOCR
{
    public partial class FrmDebug : Form
    {
        public FrmDebug()
        {
            InitializeComponent();

            if (File.Exists(Path.Combine(PathHelpers.BuildAutoTestDirectory(), "debug.log")))
            {
                review = File.ReadAllLines(Path.Combine(PathHelpers.BuildAutoTestDirectory(), "debug.log"));
            }
        }

        private void buttonB_Click(object sender, EventArgs e)
        {
            if(currentLetter>0) currentLetter--;
            DisplayItem();
        }

        private void buttonF_Click(object sender, EventArgs e)
        {
            if (currentLetter < review.Length-1) currentLetter++;
            DisplayItem();
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
            lastMousePos = new Point(info.X, info.Y);
            displayChar = info.Char;

            panel1.Invalidate();
            panel1.Update();
        }

        string[] review = new string[0];
        int currentLetter = 0;
        private Point lastMousePos;
        private char displayChar;
        private byte[] selectedBytes;

        private void panel1_Paint(object sender, PaintEventArgs e)
        {
            if (selectedBytes == null) return;

            // Debugging only!
            if (selectedBytes == null) return;

            byte[] clone = new byte[selectedBytes.Length];
            Array.Copy(selectedBytes, clone, selectedBytes.Length);
            int max = 0;
            int min = 0;
            for (int i = 0; i < clone.Length; i++)
            {
                max = Math.Max(clone[i], max);
                min = Math.Min(clone[i], min);
            }
            if (max != min)
            {
                for (int i = 0; i < clone.Length; i++)
                {
                    clone[i] = (byte)((clone[i] - min) * 255 / (max - min));
                }
            }

            for (int i = 0; i < Properties.Settings.Default.DimensionY; i++)
            {
                for (int j = 0; j < Properties.Settings.Default.DimensionY; j++)
                {
                    int col = 255 - clone[i * Properties.Settings.Default.DimensionX + j];
                    Brush b = new SolidBrush(Color.FromArgb(255, col, col, col));
                    e.Graphics.FillRectangle(b, 10 + j * 2, 10 + i * 2, 2, 2);
                    b.Dispose();
                }
            }

            int y = 5;
            if (!char.IsNumber(displayChar)) y -= 20;

            Font use = new System.Drawing.Font(FontFamily.GenericSansSerif, 22);
            if (displayChar == '#') displayChar = ',';
            e.Graphics.DrawString("" + displayChar, use, Brushes.Black, new Point(30, y));
        }

    }
}
