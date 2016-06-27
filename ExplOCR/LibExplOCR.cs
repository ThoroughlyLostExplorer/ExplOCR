﻿// Copyright 2015 by the person represented as ThoroughlyLostExplorer on GitHub
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

        public static void ProcessImageFile(OcrReader ocrReader, string file)
        {
            using (Bitmap bmp = ImageFiles.LoadImageFile(file))
            {
                Bitmap grayscale, binary, binarySplit;
                PageSections pageSections = PrepareBitmaps(bmp, out grayscale, out binary, out binarySplit);
                ocrReader.ReadPage(new Bytemap(grayscale), new Bytemap(binary), new Bytemap(binarySplit), pageSections);
                grayscale.Dispose();
                binary.Dispose();
                binarySplit.Dispose();
            }
        }

        public static void ProcessImageFile(OcrReader ocrReader, string file, out Bitmap displayA, out Bitmap displayB)
        {
            using (Bitmap bmp = ImageFiles.LoadImageFile(file))
            {
                Bitmap grayscale, binary, binarySplit;
                PageSections pageSections = PrepareBitmaps(bmp, out grayscale, out binary, out binarySplit);

                displayA = new Bitmap(binary);
                displayB = new Bitmap(grayscale);
                AnnotatePageStructure(displayA, pageSections);
                AnnotatePageHeatmap(displayB, ocrReader.QualityData);
            }
        }

        public static bool ProcessImage(OcrReader ocrReader, Bitmap bmp, out Bitmap displayA, out Bitmap displayB)
        {
            Bitmap grayscale, binary, binarySplit;
            bmp = bmp.Clone() as Bitmap;
            PageSections pageSections = PrepareBitmaps(bmp, out grayscale, out binary, out binarySplit);
            if (pageSections == null)
            {
                displayA = new Bitmap(binary);
                displayB = new Bitmap(grayscale);
                return false;
            }
            ocrReader.ReadPage(new Bytemap(grayscale), new Bytemap(binary), new Bytemap(binarySplit), pageSections);

            displayA = new Bitmap(binary);
            displayB = new Bitmap(grayscale);
            AnnotatePageStructure(displayA, pageSections);
            AnnotatePageHeatmap(displayB, ocrReader.QualityData);
            return true;
        }

        internal static PageSections PrepareBitmaps(Bitmap bmp, out Bitmap grayscale, out Bitmap binary, out Bitmap binarySplit)
        {
            Bitmap splittish, extraSplittish, glueish;
            PreprocessImages(bmp, out grayscale, out splittish, out extraSplittish, out glueish);

            PageSections pageSections = ContextAnalysis.PartitionScreen(splittish);
            if (pageSections == null)
            {
                binary = splittish;
                binarySplit = extraSplittish;
                return null;
            }

            // To prevent lower-case letters in the descriptive text area from being split in two,
            // overwrite the splittish descriptive text area with the glueish version. 
            using (Graphics g = Graphics.FromImage(splittish))
            {
                if (pageSections.DescriptiveText != null)
                {
                    if (pageSections.DescriptiveText.Bounds.Width > 0 && pageSections.DescriptiveText.Bounds.Height > 0)
                    {
                        g.DrawImage(glueish, pageSections.DescriptiveText.Bounds, pageSections.DescriptiveText.Bounds, GraphicsUnit.Pixel);
                    }
                }
                foreach (HeadlineSection hl in pageSections.Headlines)
                {
                    g.DrawImage(glueish, hl.Line.Bounds, hl.Line.Bounds, GraphicsUnit.Pixel);
                }
            }

            // Partition with the improved descriptive text letters.
            // Apply improved letter / kerning detection.
            pageSections = ContextAnalysis.PartitionScreen(splittish);
            glueish.Dispose();
            binary = splittish;
            binarySplit = extraSplittish;
            return pageSections;
        }

        private static void PreprocessImages(Bitmap bmp, out Bitmap raw, out Bitmap splittish, out Bitmap extraSplittish, out Bitmap glueish)
        {
            raw = new Bitmap(bmp);
            ImageProcessing.GrayscaleImage(raw);

            ImageProcessing.HighPassImage(bmp, 3, 5);
            splittish = new Bitmap(bmp);
            extraSplittish = new Bitmap(bmp);
            glueish = new Bitmap(bmp);
            ImageProcessing.BinarizeImage(splittish, 1.2);
            ImageProcessing.BinarizeImage(extraSplittish, 1.5);
            ImageProcessing.BinarizeImage(glueish, 0.8);
        }

        internal static void AnnotatePageStructure(Bitmap bmp, PageSections sections)
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
                foreach (ExcludeSection exclude in sections.Excluded)
                {
                    g.FillRectangle(brushHatch, exclude.Bounds);
                    g.DrawRectangle(Pens.Purple, exclude.Bounds);
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
                foreach (HeadlineSection hl in sections.Headlines)
                {
                    g.DrawRectangle(Pens.Khaki, hl.Line.Bounds);
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

        private static void AnnotatePageHeatmap(Bitmap bmp, IEnumerable<QualityData> quality)
        {
            using (Graphics g = Graphics.FromImage(bmp))
            {
                foreach(QualityData qd in quality)
                {
                    Color c = GetQualityScaleColor(qd.Quality);
                    using (Pen p = new Pen(c, 2))
                    {
                        g.DrawLine(p, qd.Frame.Left, qd.Frame.Bottom, qd.Frame.Right, qd.Frame.Bottom);
                        g.DrawLine(p, qd.Frame.Right, qd.Frame.Bottom, qd.Frame.Right, qd.Frame.Bottom-5);
                    }
                }
            }
        }

        private static Color GetQualityScaleColor(double q)
        {
            if (q < 0.5)
            {
                q = 2 * q;
                return Color.FromArgb(240, (int)(240 * q), 0);
            }
            else
            {
                q = 2 * (q - 0.5);
                return Color.FromArgb((int)(240 * (1 - q)), 240, 0);
            }
        }

        public static TransferItem[] BuildInfoArray(string xml, string system, string body, string custom, string category, string coords, List<string> archiveNames)
        {
            XmlSerializer ser = new XmlSerializer(typeof(TransferItem[]));
            StringReader reader = new StringReader(xml);
            TransferItem[] items = ser.Deserialize(reader) as TransferItem[];
            SystemCoordinates sc = ParseCoordinateValues(coords);

            foreach (TransferItem item in items)
            {
                if (item.Name == WellKnownItems.Headline && item.Values.Count > 0)
                {
                    item.Values[0].Text = system + " " + body;
                    item.Values[0].Value = double.NaN;
                    break;
                }
            }

            List<TransferItem> info = new List<TransferItem>();
            TransferItem ti = new TransferItem();
            ti.Name = WellKnownItems.System;
            ti.Values = new List<TransferItemValue>(new TransferItemValue[] { new TransferItemValue() });
            ti.Values[0].Text = system;
            ti.Values[0].Value = double.NaN;
            info.Add(ti);
            ti = new TransferItem();
            ti.Name = WellKnownItems.BodyCode;
            ti.Values = new List<TransferItemValue>(new TransferItemValue[] { new TransferItemValue() });
            ti.Values[0].Text = body;
            ti.Values[0].Value = double.NaN;
            info.Add(ti);

            ti = new TransferItem();
            ti.Name = WellKnownItems.GalCoordX;
            ti.Values = new List<TransferItemValue>(new TransferItemValue[] { new TransferItemValue() });
            ti.Values[0].Text = "";
            ti.Values[0].Value = sc.X;
            info.Add(ti);
            ti = new TransferItem();
            ti.Name = WellKnownItems.GalCoordY;
            ti.Values = new List<TransferItemValue>(new TransferItemValue[] { new TransferItemValue() });
            ti.Values[0].Text = "";
            ti.Values[0].Value = sc.Y;
            info.Add(ti);
            ti = new TransferItem();
            ti.Name = WellKnownItems.GalCoordZ;
            ti.Values = new List<TransferItemValue>(new TransferItemValue[] { new TransferItemValue() });
            ti.Values[0].Text = "";
            ti.Values[0].Value = sc.Z;
            info.Add(ti);
            
            ti = new TransferItem();
            ti.Name = WellKnownItems.CustomCategory;
            ti.Values = new List<TransferItemValue>(new TransferItemValue[] { new TransferItemValue() });
            ti.Values[0].Text = category.Replace(Environment.NewLine, ";");
            ti.Values[0].Value = double.NaN;
            info.Add(ti);
            ti = new TransferItem();
            ti.Name = WellKnownItems.CustomDescription;
            ti.Values = new List<TransferItemValue>(new TransferItemValue[] { new TransferItemValue() });
            ti.Values[0].Text = custom;
            ti.Values[0].Value = double.NaN;
            info.Add(ti);
            if (archiveNames.Count > 0)
            {
                ti = new TransferItem();
                ti.Name = WellKnownItems.ArchiveName;
                ti.Values = new List<TransferItemValue>();
                foreach (string name in archiveNames)
                {
                    TransferItemValue tiv = new TransferItemValue();
                    tiv.Text = name;
                    tiv.Value = double.NaN;
                    ti.Values.Add(tiv);
                }
                info.Add(ti);
            }
            foreach (TransferItem item in items)
            {
                if (item == null)
                {
                    continue;
                }
                if (item.Name != "DELIMITER")
                {
                    info.Add(item);
                }
            }
            return info.ToArray();
        }

        public static SystemCoordinates ParseCoordinateValues(string coords)
        {
            // Remove '(' and ')'
            if (coords.StartsWith("("))
            {
                coords = coords.Substring(1);
            }
            if (coords.EndsWith(")"))
            {
                coords = coords.Substring(0,coords.Length-1);
            }

            string[] split = coords.Split(new char[] { ',' });
            if (split.Length != 3)
            {
                return new SystemCoordinates();
            }
            SystemCoordinates sc = new SystemCoordinates();
            if (!double.TryParse(split[0], out sc.X) ||
                 !double.TryParse(split[1], out sc.Y) ||
                 !double.TryParse(split[2], out sc.Z))
            {
                return new SystemCoordinates();
            }
            return sc;
        }

        public static void SaveInfo(string xml, string system, string body, string custom, string category, string coords, List<string> archiveNames)
        {
            if (!Directory.Exists(PathHelpers.BuildUserSaveDirectory()))
            {
                Directory.CreateDirectory(PathHelpers.BuildUserSaveDirectory());
            }

            TransferItem[] items = BuildInfoArray(xml, system, body, custom, category, coords, archiveNames);



            TransferItem[][] array;
            XmlSerializer ser = new XmlSerializer(typeof(TransferItem[][]));
            try
            {
                using (FileStream fs = File.OpenRead(PathHelpers.BuildUserSaveFilename()))
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
            using (FileStream fs = new FileStream(PathHelpers.BuildUserSaveFilename(), FileMode.Create, FileAccess.Write))
            {
                ser.Serialize(fs, list.ToArray());
            }
        }

        public static string GetCurrentSystemName(string logDir, out string coords)
        {
            coords = "";
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
                            int idxA = log.IndexOf('\"');
                            int idxB = log.IndexOf('\"', idxA+1);
                            if (idxA >= 0 && idxB >= 0)
                            {
                                coords = ParseCoordinates(log);
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

        private static string ParseCoordinates(string log)
        {
            int idx = log.IndexOf(")ly");
            if (idx < 0)
            {
                return "";
            }
            log = log.Substring(0, idx);
            idx = log.IndexOf("StarPos:(");
            if (idx < 0)
            {
                return "";
            }
            log = log.Substring(idx + "StarPos:(".Length);
            return "("+log+")";
        }
    }
}
