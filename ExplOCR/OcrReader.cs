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
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace ExplOCR
{
    public class OcrReader : IDisposable
    {
        internal OcrReader(NeuralNet descriptions, NeuralNet tables, NeuralNet numbers, NeuralNet headlines, NeuralNet delimiters)
        {
            nnDescriptions = descriptions;
            nnTables = tables;
            nnNumbers = numbers;
            nnHeadlines = headlines;
            nnDelimiters = delimiters;

            tableConfig = TableItem.Load(PathHelpers.BuildConfigFilename("TableItems"));
            descriptionConfig = DescriptionItem.Load(PathHelpers.BuildConfigFilename("Descriptions"));
            itemConfig = ItemValues.Load(PathHelpers.BuildConfigFilename("ItemValues"));

            wordList = File.ReadAllLines(PathHelpers.BuildWordFilename("words"));

            Dictionary<string, int> lookup = new Dictionary<string, int>();
            foreach (DescriptionItem di in descriptionConfig)
            {
                string[] split = di.Description.Split(new char[] { ' ' });
                foreach (string s in split)
                {
                    lookup[s] = 1;
                }
            }
            List<string> tmp = new List<string>(lookup.Keys);
            wordListDescription = tmp.ToArray();
        }

        internal void ReadPage(Bytemap imageGray, Bytemap imageBinary, Bytemap imageBinarySplit, PageSections sections)
        {
            int descriptionLimit = -1;
            qualityData.Clear();
            List<TransferItem> output = new List<TransferItem>();

            // Get rid of those pesky powerplay tables.
            foreach (IPageSection section in sections.AllSections)
            {
                if (section is TextSection)
                {
                    descriptionLimit = section.Bounds.Bottom;
                }
            }

            foreach (IPageSection section in sections.AllSections)
            {
                if (section is HeadlineSection)
                {
                    TransferItem ti = ReadHeadline(section as HeadlineSection, imageGray, imageBinary);
                    if (ti != null)
                    {
                        output.Add(ti);
                    }
                }
                if (section is TextSection)
                {
                    output.Add(ReadDescription(section as TextSection, imageGray, imageBinary));
                }
                if (section is TableSection)
                {
                    if (descriptionLimit > section.Bounds.Top)
                    {
                        continue;
                    }
                    output.AddRange(ReadTableSection(section as TableSection, sections, imageGray, imageBinary, imageBinarySplit));
                }
                if (section is TextLineSection)
                {
                    TransferItem ti;
                    TextLineSection tsl = section as TextLineSection;
                    ti = ReadTerraformingLine(tsl, sections, imageGray, imageBinary);
                    if (ti != null)
                    {
                        output.Add(ti);
                    }

                    ti = ReadMiningReservesLine(tsl, sections, imageGray, imageBinary);
                    if (ti != null)
                    {
                        output.Add(ti);
                    }
                }
            }
            CustomItemProcessing(output);
            if (StitchPrevious)
            {
                output = MergeItems(baseItems, output);
            }
            baseItems = output.ToArray();
            AppendMetaInformation(output);
            currentItems = output.ToArray();
        }

        internal void ReadPageClassic(Bytemap imageGray, Bytemap imageBinary, Bytemap imageBinarySplit, PageSections sections)
        {
            qualityData.Clear();
            List<TransferItem> output = new List<TransferItem>();
            if (sections.DescriptiveText != null)
            {
                output.Add(ReadDescription(sections.DescriptiveText, imageGray, imageBinary));
            }

            foreach (HeadlineSection hl in sections.Headlines)
            {
                TransferItem ti = ReadHeadline(hl, imageGray, imageBinary);
                if (ti != null)
                {
                    output.Add(ti);
                }
            }

            foreach (TableSection table in sections.Tables)
            {
                output.AddRange(ReadTableSection(table, sections, imageGray, imageBinary, imageBinarySplit));
            }

            foreach (TextLineSection tsl in sections.TextLines)
            {
                TransferItem ti;
                ti = ReadTerraformingLine(tsl, sections, imageGray, imageBinary);
                if (ti != null)
                {
                    output.Add(ti);
                }

                ti = ReadMiningReservesLine(tsl, sections, imageGray, imageBinary);
                if (ti != null)
                {
                    output.Add(ti);
                }
            }

            CustomItemProcessing(output);
            if (StitchPrevious)
            {
                output = MergeItems(baseItems, output);
            }
            baseItems = output.ToArray();
            AppendMetaInformation(output);
            currentItems = output.ToArray();
        }

        private void AppendMetaInformation(List<TransferItem> output)
        {
            TransferItem ti = new TransferItem(WellKnownItems.ScanDate);
            ti.Values.Add(new TransferItemValue(DateTime.UtcNow.ToString("s")));
            output.Add(ti);
        }

        private TransferItem ReadHeadline(HeadlineSection hl, Bytemap imageGray, Bytemap imageBinary)
        {
            if (hl.Line.Count < 0) return null;
            if (hl.Line.Bounds.Height < 18) return null;
            if (hl.Line[0].Height < 20 && hl.Line[0].Width < 20) return null;

            string content = "";
            List<Rectangle> rs = new List<Rectangle>(hl.Line);
            for (int i = 1 /* sic! */; i < hl.Line.Count; i++)
            {
                if (i > 1 /* sic! */ && ImageLetters.IsNewHeadlineWord(rs, i))
                {
                    content += " ";
                }
                content += PredictAsLetterH(imageGray, imageBinary, rs[i]);
            }
            TransferItem ti = new TransferItem(WellKnownItems.Headline);
            ti.Values.Add(new TransferItemValue());
            ti.Values[0].Text = content;
            ti.Values[0].Value = double.NaN;
            return ti;
        }

        private IEnumerable<TransferItem> ReadTableSection(TableSection table, PageSections sections, Bytemap imageGray, Bytemap imageBinary, Bytemap imageBinarySplit)
        {
            List<TransferItem> tis = new List<TransferItem>();
            // TODO: For now, do not allow table items that are above the
            // description. This would be configurable in the future.
            if (sections.DescriptiveText != null && table.Bounds.Bottom < sections.DescriptiveText.Bounds.Top)
            {
                return tis;
            }
            if (!HasLeftText(table) || !HasRightText(table))
            {
                return tis;
            }

            tis.Add(new TransferItem("DELIMITER"));
            for (int i = 0; i < table.Count; i++)
            {
                List<Line> left;
                List<Line> right;
                GetTableItem(table, i, out left, out right);
                tis.Add(ReadTableItem(imageGray, imageBinary, imageBinarySplit, left, right));
                i += (left.Count - 1);
            }
            return tis;
        }

        private bool HasRightText(TableSection table)
        {
            foreach (Line line in table)
            {
                foreach (Rectangle r in line)
                {
                    if (r.Left >= table.Gap.Right) return true;
                }
            }
            return false;
        }

        private bool HasLeftText(TableSection table)
        {
            foreach (Line line in table)
            {
                foreach (Rectangle r in line)
                {
                    if (r.Right <= table.Gap.Left) return true;
                }
            }
            return false;
        }

        private TransferItem ReadMiningReservesLine(TextLineSection tsl, PageSections sections, Bytemap imageGray, Bytemap imageBinary)
        {
            bool afterTable = false;
            bool afterDescription = false;
            // Mining reserves are stated between first table and a headline OR immediately after the description..
            if (sections.Tables.Count < 1) return null;
            afterTable = tsl.Line.Bounds.Top > sections.Tables[0].Bounds.Bottom;
            afterDescription = tsl.Line.Bounds.Bottom < sections.Tables[0].Bounds.Top
                && tsl.Line.Bounds.Bottom > sections.Tables[0].Bounds.Top - 50;
            if (!afterTable && !afterDescription) return null;
            int index = sections.AllSections.IndexOf(tsl);
            if (index <= 0 || index + 1 >= sections.AllSections.Count) return null;
            if (!afterDescription && !(sections.AllSections[index + 1] is HeadlineSection) && !(sections.AllSections[index - 1] is TextSection)) return null;

            string mining = "";
            List<Rectangle> rs = new List<Rectangle>(tsl.Line);
            for (int i = 0; i < tsl.Line.Count; i++)
            {
                if (i > 0 && ImageLetters.IsNewWord(rs, i, true))
                {
                    mining += " ";
                }
                mining += PredictAsLetterD(imageGray, imageBinary, rs[i]);
            }
            return GuessMiningReserves(mining);
        }

        private TransferItem ReadTerraformingLine(TextLineSection tsl, PageSections sections, Bytemap imageGray, Bytemap imageBinary)
        {
            // Terraforming description is above the first table.
            if (sections.Tables.Count < 1) return null;
            if (tsl.Line.Bounds.Bottom >= sections.Tables[0].Bounds.Top) return null;

            string terraforming = "";
            List<Rectangle> rs = new List<Rectangle>(tsl.Line);
            for (int i = 0; i < tsl.Line.Count; i++)
            {
                if (i > 0 && ImageLetters.IsNewWord(rs, i, true))
                {
                    terraforming += " ";
                }
                terraforming += PredictAsLetterD(imageGray, imageBinary, rs[i]);
            }
            return GuessTerraforming(terraforming);

        }

        // Gets the table item starting at line i
        private static void GetTableItem(TableSection table, int i, out List<Line> left, out List<Line> right)
        {
            left = new List<Line>();
            right = new List<Line>();

            // Only line in item is always a gap line, regardless of wether gap has minimum width.
            if (table.Count <= i + 1 || table.GetLineItem(i + 1) > table.GetLineItem(i))
            {
                left.Add(table.GetLineLeft(i));
                right.Add(table.GetLineRight(i));
                return;
            }

            for (int j=0; i + j < table.Count; j++)
            {
                if (table.GetLineItem(i + j) > table.GetLineItem(i))
                {
                    break;
                }
                if (table.LineHasGap(i + j))
                {
                    // No two lft-and-right lines in same item
                    if (j > 0) break;
                    // Left-and-right line with table gap
                    left.Add(table.GetLineLeft(i + j));
                    right.Add(table.GetLineRight(i + j));
                }
                else if (table.GetLineLeft(i + j).Count == 0)
                {
                    // No left part of line, i.e. right-only line.
                    left.Add(table.GetLineLeft(i + j));
                    right.Add(table.GetLineRight(i + j));
                }
                else
                {
                    // Left-only line that may extend beyond the gap.
                    left.Add(table[i + j]);
                    right.Add(new Line(table.Bounds, new Rectangle[0]));
                }
            }
        }

        private TransferItem ReadDescription(TextSection desc, Bytemap imageGray, Bytemap imageBinary)
        {
            string description = "";
            List<Rectangle> all = new List<Rectangle>();
            for (int i = 0; i < desc.Count; i++)
            {
                all.AddRange(desc[i]);
            }

            for (int i = 0; i < all.Count; i++)
            {
                if (i > 0 && ImageLetters.IsNewWord(all, i, true))
                {
                    description += " ";
                }
                description += PredictAsLetterD(imageGray, imageBinary, all[i]);
            }
            TransferItem ti = new TransferItem(WellKnownItems.Description);
            TransferItemValue tv = new TransferItemValue();
            if (!RawMode)
            {
                tv.Text = GuessDescription(description);
            }
            else
            {
                tv.Text = description;
            }
            tv.Value = float.NaN;
            ti.Values.Add(tv);
            return ti;
        }

        private TransferItem GuessTerraforming(string terraforming)
        {
            TransferItem ti = new TransferItem("TERRAFORMING");

            for (int i = 0; i < itemConfig.Terraforming.Length; i++)
            {
                if (SimilarityMatch.WordsSimilar(terraforming, itemConfig.Terraforming[i]))
                {
                    TransferItemValue tv = new TransferItemValue();
                    tv.Text = itemConfig.Terraforming[i];
                    tv.Value = float.NaN;
                    ti.Values.Add(tv);
                    return ti;
                }
            }

            string[] tf = terraforming.Split(new char[] { ' ' });
            if (SimilarityMatch.WordsSimilar(tf[tf.Length - 1], "terraforming"))
            {
                TransferItemValue tv = new TransferItemValue();
                tv.Text = itemConfig.Terraforming[0];
                tv.Value = float.NaN;
                ti.Values.Add(tv);
                return ti;
            }
            if (SimilarityMatch.WordsSimilar(tf[tf.Length - 1], "terraformed"))
            {
                TransferItemValue tv = new TransferItemValue();
                tv.Text = itemConfig.Terraforming[0];
                tv.Value = float.NaN;
                ti.Values.Add(tv);
                return ti;
            }
            return null;
        }

        private TransferItem GuessMiningReserves(string mining)
        {
            TransferItem ti = new TransferItem("MINING_RESERVES");

            mining = mining.Replace("y", "j");
            mining = mining.Replace("reseaes", "reserves");
            mining = mining.Replace("reseles", "reserves");
            for (int i = 0; i < itemConfig.MiningReserves.Length; i++)
            {
                if (SimilarityMatch.WordsSimilar(mining, itemConfig.MiningReserves[i]))
                {
                    TransferItemValue tv = new TransferItemValue();
                    tv.Text = itemConfig.MiningReserves[i];
                    tv.Value = float.NaN;
                    ti.Values.Add(tv);                 
                    return ti;
                }
            }
            return null;
        }

        private TransferItem ReadTableItem(Bytemap imageGray, Bytemap imageBinary, Bytemap imageBinarySplit, List<Line> left, List<Line> right)
        {
            List<Rectangle> allLeft = new List<Rectangle>();
            int lettersRight = 0;
            foreach (Line line in right)
            {
                lettersRight += line.Count;
            }

            if (lettersRight > 0 || left.Count <= 1)
            {
                foreach (Line line in left)
                {
                    allLeft.AddRange(line);
                }
            }
            else
            {
                allLeft.AddRange(left[0]);
                for (int i = left.Count - 1; i >= 1; i--)
                {
                    right.Insert(0, left[i]);
                }
            }

            string leftText = "";
                for (int i = 0; i < allLeft.Count; i++)
                {
                    if (i > 0 && ImageLetters.IsNewWord(allLeft, i, false))
                    {
                        leftText += " ";
                    }
                    leftText += PredictAsLetter(imageGray, imageBinary, allLeft[i]);
                }

            TableItem item = GetTableItem(leftText);
            if (item == null)
            {
                return null;
            }
            if (RawMode)
            {
                item.Name = leftText;
            }

            TransferItem ti = new TransferItem(item.Name);
            for (int i = 0; i < right.Count; i++)
            {
                TransferItemValue tv = new TransferItemValue();
                List<Rectangle> rightLine = new List<Rectangle>(right[i]);
                if (rightLine.Count == 0) continue;

                if (item.InitialSkip > 0)
                {
                    rightLine.RemoveRange(0, Math.Min(item.InitialSkip, rightLine.Count));
                }

                int numberLength = GetNumberLength(rightLine, item);
                string accumulateText = "";
                string accumulateNumber = "";
                for (int j = 0; j < rightLine.Count; j++)
                {
                    if (accumulateText != "" && ImageLetters.IsNewWord(rightLine, j, false))
                    {
                        accumulateText += " ";
                    }
                    // Test if in range for numerical or for text portion.
                    if (j < numberLength)
                    {
                        if (j == 0 && rightLine[j].Width > 10)
                        {
                            // Extra splittish to get any "-" separated from the digit.
                            accumulateNumber += PredictAsNumber(imageGray, imageBinarySplit, rightLine[j]);
                        }
                        else
                        {
                            accumulateNumber += PredictAsNumber(imageGray, imageBinary, rightLine[j]);
                        }
                    }
                    else
                    {
                        // Reading part of text information: Read as letter.
                        accumulateText += PredictAsLetter(imageGray, imageBinary, rightLine[j]);
                    }
                }

                // TODO: Eliminate this hack. Should be a configurable flag in TableItem class.
                if (item.Name == "AGE" || item.Name == "RADIUS")
                {
                    // Never has a decimal point, so the decimal point is most likely a ','
                    int indexDec = accumulateNumber.IndexOf('.');
                    if (indexDec >= 0 && accumulateNumber.IndexOf(',') < 0)
                    {
                        accumulateNumber = accumulateNumber.Substring(0, indexDec) + accumulateNumber.Substring(indexDec + 1);
                    }
                }
                if (item.Name == "ORBIT_PERIAPSIS")
                {
                    if (Math.Abs(GetNumericalValue(accumulateNumber, item.Percentage)) > 360)
                    {
                        accumulateNumber =  accumulateNumber.Replace(',', '.');
                    }
                }

                if (accumulateNumber != "")
                {
                    tv.Value = GetNumericalValue(accumulateNumber, item.Percentage);
                }
                else
                {
                    tv.Value = float.NaN;
                }
                if (accumulateText != "")
                {
                    tv.Text = SimilarityMatch.GuessWords(accumulateText, wordList);
                }
                if (RawMode)
                {
                    tv.Text = accumulateNumber + accumulateText;
                }
                tv.Unit = item.Unit;
                if (item.NoText)
                {
                    tv.Text = "";
                }
                // Special case of '<' in age table item. Don't know how to handle this systematically yet.
                if (item.Name == "AGE" && accumulateText.Split(new char[] { ' ' }).Length == 3)
                {
                    tv.Value = 0;
                }
                ti.Values.Add(tv);
            }

            return ti;
        }

        private string PredictAsNumber(Bytemap imageGray, Bytemap imageBinary, Rectangle letter)
        {
            double quality;
            string text = "";
            Bytemap letterMask = ImageLetters.CopyRectangle(imageBinary, letter);
            List<Rectangle> tmp = ImageLetters.CleanupKerning(letterMask, true);
            for (int i = 0; i < tmp.Count; i++)
            {
                // Reading part of number information: Read as digit.
                Bytemap letterBytes = ImageLetters.CopyLetter(imageGray, tmp[i], nnNumbers.InputSize);
                ApplyLetterMask(i, letterBytes, letterMask);
                char c;
                if (IsDelimiter(tmp[i]))
                {
                    c = nnDelimiters.Predict(letterBytes.Bytes, false, out quality);
                }
                else
                {
                    c = nnNumbers.Predict(letterBytes.Bytes, true, out quality);
                }
                if (c == '*')
                {
                    // The current neural net is vulnurable against x/y offsets. Compensate.
                    // TODO: Handle in a better way by adjusting the neural net.
                    // TODO: Fix radius of planets e.g. 129, 223
                    Rectangle r2 = tmp[i];
                    r2.X += 1;
                    letterBytes = ImageLetters.CopyLetter(imageGray, r2, nnNumbers.InputSize);
                    ApplyLetterMask(i, letterBytes, letterMask);
                    if (IsDelimiter(r2))
                    {
                        c = nnDelimiters.Predict(ImageLetters.CopyLetter(imageGray, r2, nnNumbers.InputSize).Bytes, true, out quality);
                    }
                    else
                    {
                        c = nnNumbers.Predict(ImageLetters.CopyLetter(imageGray, r2, nnNumbers.InputSize).Bytes, true, out quality);
                    }
                    quality = Math.Max(0, quality - 0.25);
                }
                AddQualityData(quality, tmp[i]);
                FrmMain.DebugLog(c, tmp[i], letterBytes.Bytes);
                if (c == '*')
                {
                    c = '.';
                }
                text += c;
            }
            return text;
        }

        private bool IsDelimiter(Rectangle frame)
        {
            if (frame.Height <= 5)
            {
                return true;
            }
            if (frame.Height <= 9 && frame.Width <= 5)
            {
                return true;
            }
            return false;
        }

        private string PredictAsLetter(Bytemap imageGray, Bytemap imageBinary, Rectangle letter)
        {
            double quality;
            string text = "";
            Bytemap letterMask = ImageLetters.CopyRectangle(imageBinary, letter);
            List<Rectangle> tmp = ImageLetters.CleanupKerning(letterMask, false);
            for (int i = 0; i < tmp.Count; i++)
            {
                Bytemap letterBytes = ImageLetters.CopyLetter(imageGray, tmp[i], nnTables.InputSize);
                ApplyLetterMask(i, letterBytes, letterMask);
                text += nnTables.Predict(letterBytes.Bytes, false, out quality);
                AddQualityData(quality, tmp[i]);
            }
            return text;
        }

        private string PredictAsLetterD(Bytemap imageGray, Bytemap imageBinary, Rectangle letter)
        {
            double quality;
            string text = "";
            Bytemap letterMask = ImageLetters.CopyRectangle(imageBinary, letter);
            List<Rectangle> tmp = ImageLetters.CleanupKerning(letterMask, false);
            for(int i=0; i < tmp.Count; i++)
            {
                Bytemap letterBytes = ImageLetters.CopyLetter(imageGray, tmp[i], nnDescriptions.InputSize);
                ApplyLetterMask(i, letterBytes, letterMask);
                text += nnDescriptions.Predict(letterBytes.Bytes, false, out quality);
                AddQualityData(quality, tmp[i]);
            }
            return text;
        }

        private string PredictAsLetterH(Bytemap imageGray, Bytemap imageBinary, Rectangle letter)
        {
            double quality;
            string text = "";
            Bytemap letterMask = ImageLetters.CopyRectangle(imageBinary, letter);
            List<Rectangle> tmp = ImageLetters.CleanupKerning(letterMask, false);
            for (int i = 0; i < tmp.Count; i++)
            {
                Bytemap letterBytes = ImageLetters.CopyLetter(imageGray, tmp[i], nnHeadlines.InputSize);
                ApplyLetterMask(i, letterBytes, letterMask);
                text += nnHeadlines.Predict(letterBytes.Bytes, false, out quality);
                AddQualityData(quality, tmp[i]);
            }
            return text;
        }

        private void ApplyLetterMask(int n, Bytemap letter, Bytemap mask)
        {
            Rectangle letterFrame = letter.Frame;
            byte[] maskBytes = mask.Bytes;
            int DimensionX = letter.Frame.Width;
            n += 2;
            byte[] matchMask = new byte[letter.Bytes.Length];
            // Relative position of letter / letterBytes in mask / letterMask.
            int diffX = letter.Frame.X-mask.Frame.X;
            int diffY = letter.Frame.Y - mask.Frame.Y;
            int untilY = Math.Min(mask.Frame.Height - diffY, letter.Frame.Height);
            int untilX = Math.Min(mask.Frame.Width - diffX, letter.Frame.Width);
            int letterW = letter.Frame.Width;
            int maskW = mask.Frame.Width;

            // Set match mask to pixels with correct value of n.
            // i, j index the matchMask, a,b index the letterMask
            for (int i = 0, a = diffY; i < untilY; i++, a++)
            {
                for (int j = 0, b = diffX; j < untilX; j++, b++)
                {
                    matchMask[i * letterW + j] = (maskBytes[a * maskW + b] == n) ? (byte)1 : (byte)0;
                }
            }

            // Grow the blob of 1's in the match mask to include neighbouring pixels.
            for (int i = 0; i < untilY; i++)
            {
                for (int j = 0; j < untilX; j++)
                {
                    if (matchMask[i * letterW + j] != 1)
                    {
                        continue;
                    }
                    for (int a = -2; a <= 2; a++)
                    {
                        for (int b = -2; b <= 2; b++)
                        {
                            if (a + i < 0 || a + i >= untilY) continue;
                            if (b + j < 0 || b + j >= untilX) continue;
                            int index = (i + a) * letterW + (j + b);
                            if (index >= 0 && index < matchMask.Length)
                            {
                                if (matchMask[index] == 1) continue;
                                matchMask[index] = 2;
                            }
                        }
                    }
                }
            }

            for (int i = 0; i < untilY; i++)
            {
                for (int j = 0; j < untilX; j++)
                {
                    if (matchMask[i * letterW + j] == 0)
                    {
                        letter.Bytes[i * DimensionX + j] = 0;
                    }
                }
            }
        }

        internal static string DumpBytesToString(byte[] bytes, int w, int h)
        {
            string s="";
            for (int a = 0; a < h; a++)
            {
                for (int b = 0; b < w; b++)
                {
                    s += bytes[w * a + b].ToString("X2");
                }
                s += Environment.NewLine;
            }
            return s;
        }

        private string GuessDescription(string textOCR)
        {
            List<DescriptionItem> possible = new List<DescriptionItem>();
            textOCR = NormalizeHyphens(textOCR);
            List<string> words = new List<string>(textOCR.Split(new char[] { ' ' }));
            foreach (DescriptionItem di in descriptionConfig)
            {
                string desc = NormalizeHyphens(di.Description);
                int wordCount = desc.Length - desc.Replace(" ", "").Length;
                if (Math.Abs(words.Count - wordCount) < 4)
                {
                    possible.Add(di);
                }
            }

            if (possible.Count == 0)
            {
                return "";
            }
            if (possible.Count == 1)
            {
                return possible[0].Name;
            }

            for (int i = 0; i < words.Count; i++)
            {
                words[i] = SimilarityMatch.GuessWord(words[i], wordListDescription);
            }
            string textGuess = string.Join(" ", words);

            int best = 0;
            int bestScore = 10000;
            for (int i = 0; i < possible.Count; i++)
            {
                string[] split = NormalizeHyphens(possible[i].Description).Split(new char[] { ' ' });
                int distance = SimilarityMatch.SentenceDistance(split, words.ToArray());
                if (distance < bestScore)
                {
                    best = i;
                    bestScore = distance;
                }
            }
            return possible[best].Name;
        }

        private string NormalizeHyphens(string desc)
        {
            desc = desc.Replace('-', ' ');
            desc = desc.Replace("  ", " ");
            desc = desc.Replace("  ", " ");
            return desc;
        }


        private int GetNumberLength(List<Rectangle> rightLine, TableItem item)
        {
            int numberLength;
            for (numberLength = 1; numberLength < rightLine.Count; numberLength++)
            {
                if (ImageLetters.IsNewWord(rightLine, numberLength, false))
                {
                    break;
                }
            }
            if (item.Percentage && numberLength == rightLine.Count)
            {
                // Percentage values use "short" value because the space separating the 
                // component name is sometimes not visible.
                // TODO: Find a way to remove the '%', this would create enough space.
                for (numberLength = rightLine.Count - 1; numberLength >= 0; numberLength--)
                {
                    if (ImageLetters.IsNewWord(rightLine, numberLength, true))
                    {
                        break;
                    }
                }
            }

            if (item.AllText)
            {
                numberLength = 0;
            }
            else
            {
                int unit = item.ExcludeUnit;
                if (item.Unit == "KM" && unit == 2 && rightLine.Count > 2)
                {
                    // Counter letter splits within the KM
                    if (rightLine[rightLine.Count - 1].Width < 5 || rightLine[rightLine.Count - 2].Width < 5)
                    {
                        unit += 1;
                    }
                }
                numberLength = Math.Max(numberLength - unit, 0);
            }
            return numberLength;
        }

        private static double GetNumericalValue(string numberText, bool percentage)
        {
            // Handle a rather obcure case. (???)
            int index = numberText.IndexOf('-');
            if (index == 0) index = numberText.IndexOf('-', index + 1);

            if (index > 0 && !numberText.Contains('.'))
            {
                numberText = numberText.Substring(0, index) + '.' + numberText.Substring(index + 1);
            }

            if (numberText.Length > 1 && numberText[0] == '.')
            {
                numberText = "-" + numberText.Substring(1);
            }

            // Unprotect the commas.
            numberText = numberText.Replace('#', ',');

            // If two '.' exist, then the rightmost one is likely real
            // and the leftmost one is likely a misread ','
            if (numberText.LastIndexOf('.') != numberText.IndexOf('.'))
            {
                int idx = numberText.LastIndexOf('.');
                if (idx >= 0 && idx + 1 < numberText.Length)
                {
                    numberText = numberText.Replace('.', ',');
                    numberText = numberText.Substring(0, idx) + "." + numberText.Substring(idx + 1);
                }
            }
            // No ',' to the right of '.'
            if (numberText.LastIndexOf(',') > numberText.IndexOf('.'))
            {
                numberText = numberText.Replace('.', ',');
            }
            // Make sure all ',' are genuine and not misread '.'
            int decSep = numberText.IndexOf('.');
            if (decSep < 0) decSep = numberText.Length;
            for (int k = numberText.Length - 1; k >= 0; k--)
            {
                if (numberText[k] == ',' && (decSep - k) % 4 != 0)
                {
                    numberText = numberText.Substring(0, k) + "." + numberText.Substring(k + 1);
                    decSep = k;
                }
            }

            if (percentage && numberText.IndexOf('.') + 1 < numberText.Length)
            {
                numberText = numberText.Substring(0, numberText.IndexOf('.') + 2);
            }

            double number;
            if (!double.TryParse(numberText, out number))
            {
                return 0;
            }
            return number;
        }

        private TableItem GetTableItem(string leftText)
        {
            foreach (TableItem item in tableConfig)
            {
                if (leftText == item.Description)
                {
                    return item;
                }
                if (item.MinimalMatch != "" && leftText.Contains(item.MinimalMatch))
                {
                    return item;
                }
            }
            foreach (TableItem item in tableConfig)
            {
                if (SimilarityMatch.HasSimilarWord(leftText, item.MinimalMatch))
                {
                    return item;
                }
            }
            foreach (TableItem item in tableConfig)
            {
                if (SimilarityMatch.SentencesSimilarPerWord(leftText, item.Description))
                {
                    return item;
                }
            }

            int bestDistance = int.MaxValue;
            int bestItem = -1;
            for (int i=0; i < tableConfig.Length; i++)
            {
                int distance = SimilarityMatch.SentenceWordDistance(leftText, tableConfig[i].Description);
                if (distance < bestDistance)
                {
                    bestItem = i;
                    bestDistance = distance;
                }
            }
            if (bestItem >= 0)
            {
                return tableConfig[bestItem];
            }
            return null;
        }

        private void CustomItemProcessing(List<TransferItem> output)
        {
            for (int i = 0; i < output.Count; i++)
            {
                if (output[i] == null) continue;
                if (output[i].Name == "ROTATION_PERIOD" && output[i].Values.Count > 0)
                {
                    string locked = "NO";
                    if (SimilarityMatch.HasSimilarWord(output[i].Values[0].Text, "LOCKED"))
                    {
                        locked = "YES";
                    }
                    TransferItem ti = new TransferItem("ROTATION_LOCKED");
                    ti.Values.Add(new TransferItemValue());
                    ti.Values[0].Text = locked;
                    ti.Values[0].Value = double.NaN;
                    ti.Values[0].Unit = GetDataUnit("ROTATION_LOCKED");
                    output.Insert(i + 1, ti);
                    output[i].Values[0].Text = "";
                }
            }

            foreach (TransferItem ti in output)
            {
                if (ti == null) continue;
                if (ti.Name == "VOLCANISM_TYPE")
                {
                    if (!itemConfig.VolcanismTypes.Contains<string>(ti.Values[0].Text.Trim()))
                    {
                        ti.Values[0].Text = ForceItemList(ti.Values[0].Text.Trim(), itemConfig.VolcanismTypes);
                    }
                }
                if (ti.Name == "ATMOSPHERE_TYPE")
                {
                    // Multi-line atmosphere types -- example is "suitable for water-based\\life"
                    if (ti.Values.Count > 1 && ti.Values[0].Unit == "LITERAL")
                    {
                        string complete = "";
                        // Loop backwards over all items except ti.Values[0]
                        for (int i = ti.Values.Count - 1; i > 0; i--)
                        {
                            if (ti.Values[i].Unit != "LITERAL")
                            {
                                // Skip anything irregular.
                                continue;
                            }
                            // Gather up into "complete"
                            complete = ti.Values[i].Text.Trim() + " " + complete;
                            // Remove item no longer needed.
                            ti.Values.RemoveAt(i);
                        }
                        ti.Values[0].Text += " "+complete;
                    }

                    ti.Values[0].Text = ti.Values[0].Text.Trim();
                    if (ti.Values[0].Text.StartsWith("NO ")) ti.Values[0].Text = "NO ATMOSPHERE";
                    if (!itemConfig.AtmosphereTypes.Contains<string>(ti.Values[0].Text.Trim()))
                    {
                        ti.Values[0].Text = ForceItemList(ti.Values[0].Text.Trim(), itemConfig.AtmosphereTypes);
                    }
                }
                if (ti.Name == "ATMOSPHERE")
                {
                    foreach (TransferItemValue v in ti.Values)
                    {
                        v.Text = v.Text.Trim();
                        int word = v.Text.IndexOf(' ');
                        if (word >= 0 && word < 3) v.Text = v.Text.Substring(word + 1);
                        if (!itemConfig.AtmosphereComponents.Contains<string>(v.Text.Trim()))
                        {
                            v.Text = ForceItemList(v.Text.Trim(), itemConfig.AtmosphereComponents);
                        }
                    }
                }
                if (ti.Name == "COMPOSITION")
                {
                    double sum = 0;
                    foreach (TransferItemValue v in ti.Values)
                    {
                        v.Text = v.Text.Trim();
                        int word = v.Text.IndexOf(' ');
                        if (word >= 0 && word < 3) v.Text = v.Text.Substring(word+1);
                        if (v.Text.StartsWith("ROC")) v.Text = "ROCK";
                        if (!itemConfig.SolidComponents.Contains<string>(v.Text.Trim()))
                        {
                            v.Text = ForceItemList(v.Text.Trim(), itemConfig.SolidComponents);
                        }
                        sum += v.Value;
                    }
                    if (Math.Abs(sum - 100) > 0.25)
                    {
                        sum = 0;
                    }
                }
                if (ti.Name == "RING_TYPE" && ti.Values.Count > 0)
                {
                    if (ti.Values[0].Text.StartsWith("ROC")) ti.Values[0].Text = "ROCKY";
                    if (ti.Values[0].Text == "ICE") ti.Values[0].Text = "ICY";
                    ti.Values[0].Text = ti.Values[0].Text.Trim();
                    if (!itemConfig.RingTypes.Contains<string>(ti.Values[0].Text.Trim()))
                    {
                        ti.Values[0].Text = ForceItemList(ti.Values[0].Text.Trim(), itemConfig.RingTypes);
                    }
                }
            }
        }

        private string ForceItemList(string item, string[] list)
        {
            item = item.Trim();
            int bestDistance = int.MaxValue;
            int bestItem = -1;
            for (int i = 0; i < list.Length; i++)
            {
                int distance = SimilarityMatch.SentenceWordDistance(item, list[i]);
                if (distance < bestDistance)
                {
                    bestItem = i;
                    bestDistance = distance;
                }
            }
            if (bestItem >= 0)
            {
                return list[bestItem];
            }
            return item;            
        }

		// TODO: Merging needs work.
        private List<TransferItem> MergeItems(IEnumerable<TransferItem> previousItems, IEnumerable<TransferItem> newItems)
        {
            List<string> multipleAllowed = new List<string>(new string[] { "RING_TYPE", "RING_INNER", "RING_OUTER", "RING_MASS" });
            bool overlapFound = false;
            List<TransferItem> merged = new List<TransferItem>(previousItems);
            List<int> replace = new List<int>();
            if (merged.Count > 0)
            {
                // The last item of the previous list is likely broken and should not be used.
                if (!multipleAllowed.Contains(merged[merged.Count - 1].Name))
                {
                    replace.Add(merged.Count - 1);
                }
            }
            if (merged.Count > 1)
            {
                // The this item may actually come from same broken text line
                // (ROTATION_PERIOD/ROTATION_LOCKED)
                if (!multipleAllowed.Contains(merged[merged.Count - 2].Name))
                {
                    replace.Add(merged.Count - 2);
                }
            }
            bool first = true;
            foreach (TransferItem item in newItems)
            {
                if (first)
                {
                    // Don't use the first item.
                    first = false;
                    continue;
                }
                if (ItemExistsIn(item, previousItems))
                {
                    overlapFound = true;
                }
                if (overlapFound && !ItemExistsIn(item, previousItems))
                {
                    int replaceIndex = -1;
                    foreach (int i in replace)
                    {
                        if (merged[i].Name == item.Name)
                        {
                            replaceIndex = i;
                        }
                    }
                    if (replaceIndex >= 0 && merged[replaceIndex].Name == item.Name)
                    {
                        // The last item of the previous list is likely broken and should not be used.
                        merged[replaceIndex] = item;
                        replace.Remove(replaceIndex);
                        replaceIndex = -1;
                    }
                    else
                    {
                        merged.Add(item);
                    }
                }
            }
            return merged;
        }

        private bool ItemExistsIn(TransferItem item, IEnumerable<TransferItem> previousItems)
        {
            if (item == null)
            {
                return previousItems.Contains(null);
            }
            foreach (TransferItem previous in previousItems)
            {
                bool match = true;
                if (previous == null) continue;
                if (item.Name != previous.Name) continue;
                if (item.Values == null) continue;
                if (previous.Values == null) continue;
                if (item.Values.Count != previous.Values.Count) continue;
                for (int i = 0; i < item.Values.Count; i++)
                {
                    if (item.Values[i].Text != previous.Values[i].Text) match = false;
                    if (item.Values[i].Unit != previous.Values[i].Unit) match = false;
                    if (double.IsNaN(item.Values[i].Value))
                    {
                        if (!double.IsNaN(previous.Values[i].Value)) match = false;
                    }
                    else
                    {
                        if (item.Values[i].Value != previous.Values[i].Value) match = false;
                    }
                }
                if (match) return true;
            }
            return false;
        }

        private void AddQualityData(double quality, Rectangle rectangle)
        {
            qualityData.Add(new QualityData(quality, rectangle));
        }

        public IEnumerable<QualityData> QualityData
        {
            get { return qualityData; }
        }

        public bool StitchPrevious
        {
            get { return stitchMode; }
            set { stitchMode = value; }
        }

        public void Dispose()
        {
            if (nnDescriptions != null)
            {
                nnDescriptions.Dispose();
            }
            if (nnNumbers != null)
            {
                nnNumbers.Dispose();
            }
            if (nnTables != null)
            {
                nnTables.Dispose();
            }
        }

        private string GetDataUnit(string name)
        {
            foreach (TableItem ti in tableConfig)
            {
                if (ti.Name == name) return ti.Unit;
            }
            return "";
        }

        public bool RawMode
        {
            get { return rawMode; }
            set { rawMode = value; }
        }

        public TransferItem[] Items
        {
            get { return currentItems; }
        }

        bool stitchMode = false;

        string[] wordList;
        string[] wordListDescription;

        TableItem[] tableConfig;
        DescriptionItem[] descriptionConfig;
        ItemValues itemConfig;
        NeuralNet nnDescriptions;
        NeuralNet nnNumbers;
        NeuralNet nnHeadlines;
        NeuralNet nnTables;
        NeuralNet nnDelimiters;
        TransferItem[] currentItems = new TransferItem[0];
        TransferItem[] baseItems = new TransferItem[0];
        List<QualityData> qualityData = new List<QualityData>();
        bool rawMode;
    }
}
