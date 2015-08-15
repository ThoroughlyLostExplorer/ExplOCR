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
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace ExplOCR
{
    public static class LibExplOCR
    {
        public static OcrReader CreateOcrReader()
        {
            OcrReader ocrReader;
            TrainingConfig.TrainNN(out ocrReader);
            return ocrReader;
        }

        public static void ProcessImageFile(OcrReader ocrReader, string file, bool raw)
        {
            using (Bitmap bmp = ImageFiles.LoadImageFile(file))
            {
                Bitmap grayscale, binary;
                PageSections pageSections = PrepareBitmaps(bmp, out grayscale, out binary);
                ocrReader.ReadPage(new Bytemap(grayscale), new Bytemap(binary), pageSections, raw);
            }
        }

        public static void ProcessImageFile(OcrReader ocrReader, string file, bool raw, out Bitmap display)
        {
            using (Bitmap bmp = ImageFiles.LoadImageFile(file))
            {
                Bitmap grayscale, binary;
                PageSections pageSections = PrepareBitmaps(bmp, out grayscale, out binary);
                display = new Bitmap(binary);
                AnnotateDrawBitmap(display, pageSections);
                ocrReader.ReadPage(new Bytemap(grayscale), new Bytemap(binary), pageSections, raw);
            }
        }

        public static void ProcessImage(OcrReader ocrReader, Bitmap bmp, bool raw, out Bitmap display)
        {
            Bitmap grayscale, binary;
            bmp = bmp.Clone() as Bitmap;
            PageSections pageSections = PrepareBitmaps(bmp, out grayscale, out binary);
            display = new Bitmap(binary);
            AnnotateDrawBitmap(display, pageSections);
            ocrReader.ReadPage(new Bytemap(grayscale), new Bytemap(binary), pageSections, raw);
        }

        internal static PageSections PrepareBitmaps(Bitmap bmp, out Bitmap grayscale, out Bitmap binary)
        {
            Bitmap splittish, glueish;
            PreprocessImages(bmp, out grayscale, out splittish, out glueish);

            PageSections pageSections = ContextAnalysis.PartitionScreen(splittish);

            // To prevent lower-case letters in the descriptive text area, from being split in two,
            // overwrite the splittish descriptive text area with the glueish version. 
            if (pageSections.DescriptiveText != null)
            {
                if (pageSections.DescriptiveText.Bounds.Width > 0 && pageSections.DescriptiveText.Bounds.Height > 0)
                {
                    using (Graphics g = Graphics.FromImage(splittish))
                    {
                        g.DrawImage(glueish, pageSections.DescriptiveText.Bounds, pageSections.DescriptiveText.Bounds, GraphicsUnit.Pixel);
                    }
                }
            }
            // Partition with the improved descriptive text letters.
            // Apply improved letter / kerning detection.
            pageSections = ContextAnalysis.PartitionScreen(splittish);
            glueish.Dispose();
            binary = splittish;
            return pageSections;
        }

        private static void PreprocessImages(Bitmap bmp, out Bitmap raw, out Bitmap splittish, out Bitmap glueish)
        {
            raw = new Bitmap(bmp);
            ImageProcessing.GrayscaleImage(raw);

            ImageProcessing.HighPassImage(bmp, 3, 5);
            splittish = new Bitmap(bmp);
            glueish = new Bitmap(bmp);
            ImageProcessing.BinarizeImage(splittish, 1.1);
            ImageProcessing.BinarizeImage(glueish, 0.8);
        }

        internal static void AnnotateDrawBitmap(Bitmap bmp, PageSections sections)
        {
            if (sections == null)
            {
                return;
            }

            using (Graphics g = Graphics.FromImage(bmp))
            using (Pen greenpen = new Pen(Color.FromArgb(120, Color.Green)))
            using (Brush brushHatch = new HatchBrush(HatchStyle.BackwardDiagonal, Color.Purple))
            using (Brush b = new SolidBrush(Color.FromArgb(120, Color.Blue)))
            {
                foreach (Rectangle exclude in sections.Excluded)
                {
                    g.FillRectangle(brushHatch, exclude);
                    g.DrawRectangle(Pens.Purple, exclude);
                }

                foreach (Line line in sections.AllLines)
                {
                    g.FillRectangle(b, line.Bounds);
                    foreach (Rectangle letter in line)
                    {
                        g.DrawRectangle(greenpen, letter);
                    }
                }

                if (sections.DescriptiveText != null)
                {
                    Rectangle r = sections.DescriptiveText.Bounds;
                    r.Inflate(-3, 0);
                    g.DrawRectangle(Pens.Orange, r);
                }
                foreach (TextLineSection tl in sections.TextLines)
                {
                    g.DrawRectangle(Pens.Purple, tl.Line.Bounds);
                }

                foreach (TableSection table in sections.Tables)
                {
                    g.DrawRectangle(Pens.Red, table.Bounds);
                    g.DrawRectangle(Pens.Red, table.Gap);

                    int previousItem = 0;
                    for (int i = 1 /*sic!*/; i < table.Count; i++)
                    {
                        if (table.GetLineItem(i) > previousItem)
                        {
                            g.DrawLine(Pens.Red, table.Bounds.Left, table[i].Bounds.Top - 6, table.Gap.Left, table[i].Bounds.Top - 6);
                            g.DrawLine(Pens.Red, table.Bounds.Right, table[i].Bounds.Top - 6, table.Gap.Right, table[i].Bounds.Top - 6);
                            previousItem = table.GetLineItem(i);
                        }
                    }
                }

                /* Word display for debugging.
                int wordStart = 0;
                for (int i = 0; i < sections.AllLetters.Count; i++)
                {
                    bool small = sections.DescriptiveText.Bounds.Contains(sections.AllLetters[i]);
                    if (ImageLetters.IsNewWord(sections.AllLetters,i, small))
                    {
                        r = sections.AllLetters[wordStart];
                        for (int j = wordStart; j < i; j++)
                        {
                            r = Rectangle.Union(r, sections.AllLetters[j]);
                        }
                        g.FillRectangle(Brushes.Yellow, r);
                        wordStart = i;
                    }
                }*/
            }
        }

        public static void SaveInfo(string xml, string system, string body, string archiveName)
        {
            if (!Directory.Exists(PathHelpers.BuildSaveDirectory()))
            {
                Directory.CreateDirectory(PathHelpers.BuildSaveDirectory());
            }

            XmlSerializer ser = new XmlSerializer(typeof(TransferItem[]));
            StringReader reader = new StringReader(xml);
            TransferItem[] items = ser.Deserialize(reader) as TransferItem[];

            List<TransferItem> info = new List<TransferItem>();
            TransferItem ti = new TransferItem();
            ti.Name = "SYSTEM";
            ti.Values = new List<TransferItemValue>(new TransferItemValue[] { new TransferItemValue() });
            ti.Values[0].Text = system;
            ti.Values[0].Value = float.NaN;
            info.Add(ti);
            ti = new TransferItem();
            ti.Name = "BODY";
            ti.Values = new List<TransferItemValue>(new TransferItemValue[] { new TransferItemValue() });
            ti.Values[0].Text = body;
            ti.Values[0].Value = float.NaN;
            info.Add(ti);
            info.AddRange(items);
            ti = new TransferItem();
            ti.Name = "ARCHIVE_NAME";
            ti.Values = new List<TransferItemValue>(new TransferItemValue[] { new TransferItemValue() });
            ti.Values[0].Text = archiveName;
            ti.Values[0].Value = float.NaN;
            info.Add(ti);
            info.AddRange(items);
            items = info.ToArray();

            TransferItem[][] array;
            ser = new XmlSerializer(typeof(TransferItem[][]));
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

            List<TransferItem[]> list = new List<TransferItem[]>(array);
            list.Add(items);
            using (FileStream fs = File.OpenWrite(PathHelpers.BuildSaveFilename()))
            {
                ser.Serialize(fs, list.ToArray());
            }
        }

        public static string GetCurrentSystemName(string logDir)
        {
            try
            {
                if (string.IsNullOrEmpty(logDir))
                {
                    return "";
                }
                if (!Directory.Exists(logDir))
                {
                    return "";
                }
                // Find locations at https://support.frontier.co.uk/kb/faq.php?id=108                
                string[] files = Directory.GetFiles(logDir, "netlog*.log");
                if (files.Length > 0)
                {
                    using (FileStream fs = new FileStream(files[files.Length - 1], FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                    using (StreamReader sr = new StreamReader(fs))
                    {
                        string log = sr.ReadToEnd();
                        int idx = log.LastIndexOf("System:");
                        if (idx >= 0)
                        {
                            log = log.Substring(idx);
                            int idxA = log.IndexOf('(');
                            int idxB = log.IndexOf(')');
                            if (idxA >= 0 && idxB >= 0)
                            {
                                return log.Substring(idxA + 1, idxB - idxA - 1);
                            }
                        }
                    }
                }
            }
            catch
            {
            }
            return "";
        }
    }
}
