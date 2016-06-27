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
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExplOCR
{
    class ImageLetters
    {
        public static bool IsNewWord(List<Rectangle> letters, int i, bool small)
        {
            if (i == 0)
            {
                return true;
            }
            int distance = small ? Properties.Settings.Default.WordDistanceSmall : Properties.Settings.Default.WordDistanceLarge;
            return letters[i].Left - letters[i - 1].Right > distance || letters[i].Top >= letters[i - 1].Bottom;
        }

        public static bool IsNewHeadlineWord(List<Rectangle> letters, int i)
        {
            if (i == 0)
            {
                return true;
            }
            int distance = Properties.Settings.Default.WordDistanceXLarge;
            return letters[i].Left - letters[i - 1].Right > distance || letters[i].Top >= letters[i - 1].Bottom;
        }

        // Splits the screen into (text-)lines. Lines are rectangular areas that contain non-0
        // pixels that are separated by 'blank' rows of all-0 pixels.
        public static List<Rectangle> GetPrimitiveLines(Bitmap bmp)
        {
            List<Rectangle> lines = new List<Rectangle>();
            int[] histogram = GetRowHistogram(bmp, 0, bmp.Height);


            bool inLine = false;
            Rectangle line = new Rectangle();
            line.X = 0;
            line.Width = bmp.Width;
            // First try to find lines as completely separate blocks in histogram.
            int LineThreshold = 0;
            for (int i = 0; i < histogram.Length; i++)
            {
                if (!inLine && histogram[i] > LineThreshold)
                {
                    line.Y = i;
                    inLine = true;
                }
                if (inLine && histogram[i] <= LineThreshold)
                {
                    line.Height = i - line.Y;
                    inLine = false;
                    lines.Add(line);
                }
            }
            if (inLine)
            {
                line.Height = histogram.Length - line.Y;
                lines.Add(line);
            }

            List<int> gap = new List<int>();
            for (int i = 1; i < lines.Count; i++)
            {
                gap.Add(lines[i].Top - lines[i - 1].Bottom);
            }
            return lines;
        }

        private unsafe static int[] GetRowHistogram(Bitmap bmp, int y, int height)
        {
            int[] histogram = new int[height];

            BitmapData data = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height), ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb);
            try
            {
                byte* buf = (byte*)data.Scan0;
                for (int i = y; i < y + height; i++)
                {
                    histogram[i - y] = 0;
                    for (int j = 0; j < bmp.Width; j++)
                    {
                        if (buf[4 * (i * bmp.Width + j)] != 0)
                        {
                            histogram[i - y]++;
                        }
                    }
                }
                return histogram;
            }
            finally
            {
                bmp.UnlockBits(data);
            }
        }

        // Find letters within a line. In the first pass, letters are assumed to be rectangles
        // separated by vertical lines of pixels that are 0 (i.e. no kerning).
        // In a second pass, letters that are unusually wide are analyzed to see if they
        // are actually several letters using kerning.
        public unsafe static List<Rectangle> GetPrimitiveLetters(Bitmap bmp, Rectangle line)
        {
            List<Rectangle> letters = new List<Rectangle>();
            int[] contentT = new int[bmp.Width];
            int[] contentB = new int[bmp.Width];

            BitmapData data = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height), ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb);
            try
            {
                byte* buf = (byte*)data.Scan0;
                for (int i = 0; i < contentT.Length; i++)
                {
                    contentT[i] = bmp.Height + 1;
                    contentB[i] = -1;
                    for (int j = line.Top; j < line.Bottom; j++)
                    {
                        if (buf[4 * (j * bmp.Width + i)] != 0)
                        {
                            contentB[i] = Math.Max(contentB[i], j);
                            contentT[i] = Math.Min(contentT[i], j);
                        }
                    }
                }

                bool inLetter = false;
                Rectangle letter = new Rectangle();
                int letterB = -1;
                int letterT = bmp.Height + 1;
                for (int i = 0; i < contentT.Length; i++)
                {
                    if (!inLetter && contentB[i] >= 0)
                    {
                        letter.X = i;
                        inLetter = true;
                    }
                    if (inLetter && contentB[i] < 0)
                    {
                        letter.Width = i - letter.X;
                        inLetter = false;
                        letters.Add(new Rectangle(letter.X, letterT, letter.Width, 1 + letterB - letterT));
                        letterB = -1;
                        letterT = bmp.Height + 1;
                    }
                    if (inLetter)
                    {
                        letterB = Math.Max(letterB, contentB[i]);
                        letterT = Math.Min(letterT, contentT[i]);
                    }
                }
                if (inLetter)
                {
                    letter.Width = contentT.Length - letter.X;
                    letters.Add(new Rectangle(letter.X, letter.Y, letter.Width, 1 + letter.Height - letter.Y));
                }
                return letters;
            }
            finally
            {
                bmp.UnlockBits(data);
            }
        }

        public static List<Rectangle> CleanupKerning(Bytemap sourceFrame, bool number)
        {
            byte[] letterBuf = sourceFrame.Bytes;
            Rectangle frame = sourceFrame.Frame;
            List<Rectangle> letters = new List<Rectangle>();
            letters.Add(frame);
            for (int i = 0; i < letterBuf.Length; i++)
            {
                letterBuf[i] = (letterBuf[i] != 0) ? (byte)1 : (byte)0;
            }

            for (int i = 0; i < letters.Count; i++)
            {
                int max = 0;
                byte targetValue = (byte)(i + 2);

                // For a small interval, paint all pixels with value 1 using the
                // target color. This gives the flood fill something to start with.
                int fillUntil = (letters[i].X - frame.X) + Properties.Settings.Default.KerningEliminationAutoFill;
                fillUntil = Math.Min(fillUntil, frame.Width);
                for (int a = letters[i].X - frame.X; a < fillUntil; a++)
                {
                    for (int b = 0; b < frame.Height; b++)
                    {
                        if (letterBuf[frame.Width * b + a] == 1)
                        {
                            letterBuf[frame.Width * b + a] = targetValue;
                            max = Math.Max(max, a);
                        }
                    }
                }

                // Flood fill to replace 1 with the target color.
                FloodFill(1, targetValue, frame.Size, letterBuf, number);
                // Find rightmost column painted in target color..
                for (int a = letters[i].X - frame.X; a < frame.Width; a++)
                {
                    for (int b = 0; b < frame.Height; b++)
                    {
                        if (letterBuf[frame.Width * b + a] == targetValue)
                        {
                            max = Math.Max(max, a);
                        }
                    }
                }

                if (max != 0 && frame.Width - max > 2)
                {
                    Rectangle r1 = GetSubRect(frame, letterBuf, 1);
                    Rectangle r2 = GetSubRect(frame, letterBuf, targetValue);
                    letters[i] = r2;
                    letters.Insert(i + 1, r1);
                }
            }
            return letters;
        }

        private static void FloodFill(int originalValue, byte targetValue, Size frame, byte[] letterBuf, bool number)
        {
            bool overallChange = false;
            do
            {
                overallChange = false;
                // Flood ascending columns.
                for (int a = 0; a < frame.Width; a++)
                {
                    bool change;
                    do
                    {
                        change = FloodFillColumn(originalValue, targetValue, a, frame, letterBuf, number);
                        overallChange = overallChange || change;
                    } while (change);
                }
                // Flood descending columns.
                for (int a = frame.Width - 1; a >= 0; a--)
                {
                    bool change;
                    do
                    {
                        change = FloodFillColumn(originalValue, targetValue, a, frame, letterBuf, number);
                        overallChange = overallChange || change;
                    } while (change);
                }
            }
            while (overallChange);
        }

        private static bool FloodFillColumn(int originalValue, byte targetValue, int a, Size frame, byte[] letterBuf, bool number)
        {
            bool change = false;
            int yrange = number ? 1 : 2;
            for (int b = 0; b < frame.Height; b++)
            {
                // Only consider changing pixels that have value 1
                if (letterBuf[frame.Width * b + a] != 1)
                {
                    continue;
                }

                // Change pixel value to 2 if a pixel within a strip 3 px wide and 5 px tall
                // already has value 2. The vertical limit of 5 px was chosen because 
                // binarisation will sometimes lead to pixels which are a little bit unconnected.
                // Typical letter kerning will be outside this range.
                for (int x = -1; x <= 1; x++)
                {
                    for (int y = -yrange; y <= yrange; y++)
                    {
                        if (a + x < 0 || a + x >= frame.Width) continue;
                        if (b + y < 0 || b + y >= frame.Height) continue;
                        int index = frame.Width * (b + y) + (a + x);
                        if (index < 0 || index >= letterBuf.Length - 1) continue;
                        if (letterBuf[index] == targetValue)
                        {
                            letterBuf[frame.Width * b + a] = targetValue;
                            change = true;
                        }
                    }
                }
            }
            return change;
        }

        // Get the sub-rectangle of r that contains the values in the buffer equal to value.
        private static Rectangle GetSubRect(Rectangle r, byte[] letterBuf, int value)
        {
            int maxX = 0;
            int maxY = 0;
            int minX = int.MaxValue;
            int minY = int.MaxValue;
            for (int i = 0; i < r.Width; i++)
            {
                for (int j = 0; j < r.Height; j++)
                {
                    if (letterBuf[r.Width * j + i] == value)
                    {
                        maxX = Math.Max(i, maxX);
                        maxY = Math.Max(j, maxY);
                        minX = Math.Min(i, minX);
                        minY = Math.Min(j, minY);
                    }
                }
            }
            return new Rectangle(r.X + minX, r.Y + minY, 1 + maxX - minX, 1 + maxY - minY);
        }

        public unsafe static byte[] GetLetterImage(Bitmap baseBmp, Rectangle frame, Size dimension)
        {
            Bitmap bmp = baseBmp.Clone(frame, baseBmp.PixelFormat);
            BitmapData data = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height), ImageLockMode.ReadWrite, bmp.PixelFormat);
            byte[] bytes = new byte[dimension.Width * dimension.Height];
            int min = 255;
            try
            {
                byte* d = (byte*)data.Scan0;
                for (int i = 0; i < Math.Min(data.Height, dimension.Height); i++)
                {
                    for (int j = 0; j < Math.Min(data.Width, dimension.Width); j++)
                    {
                        bytes[i * dimension.Width + j] = d[4 * (i * data.Width + j)];
                        min = Math.Min(bytes[i * dimension.Width + j], min);
                    }
                }
            }
            finally
            {
                bmp.UnlockBits(data);
            }
            for (int i = 0; i < bytes.Length; i++)
            {
                if (bytes[i] >= min)
                    bytes[i] = (byte)(bytes[i] - min);
                else
                    bytes[i] = 0;
            }

            return bytes;
        }

        // Description lines are close together and have a tendency to stick together.
        // Use minimum bracketing on the histogram because description lines are not
        // favourable for flood-fill detection of connected pixels.
        internal static TextSection ImproveDescriptionLines(Bitmap bmp, TextSection description)
        {
            List<Line> lines = new List<Line>(description);
            bool change = true;
            while (change)
            {
                lines = new List<Line>(description.Count);
                change = false;
                foreach (Line line in description)
                {
                    if (lines.Count > 100)
                    {
                        // Safety for unsuitable screenshots
                        break;
                    }
                    if (line.Bounds.Height < Properties.Settings.Default.DimensionY * 1.5)
                    {
                        lines.Add(line);
                        continue;
                    }

                    int[] histogram = GetRowHistogram(bmp, line.Bounds.Y, line.Bounds.Height);

                    int max = 0;
                    int maxIdx = 0;
                    for (int i = 0; i < histogram.Length; i++)
                    {
                        if (max < histogram[i])
                        {
                            max = histogram[i];
                            maxIdx = i;
                        }
                    }

                    // Get minimum bracketed by two maxima so that the sum
                    // of height differences is maximized.
                    // Loop over all i to bracket between indexes i and maxIdx
                    int best = 0;
                    int bestIdx = 0;
                    for (int i = 0; i < histogram.Length; i++)
                    {
                        if (Math.Abs(i - maxIdx) < 2)
                        {
                            continue;
                        }

                        // Loop to get minimum within the bracket.
                        int min = histogram[i];
                        int minIdx = maxIdx;
                        for (int j = Math.Min(maxIdx, i); j < Math.Max(maxIdx, i); j++)
                        {
                            if (histogram[j] < min)
                            {
                                min = histogram[j];
                                minIdx = j;
                            }
                        }

                        if (max - min + histogram[i] - min > best)
                        {
                            best = max - min + histogram[i] - min;
                            bestIdx = minIdx;
                        }
                    }

                    // Split the line horizontally at the minimum.
                    Rectangle top = new Rectangle(line.Bounds.X, line.Bounds.Y, line.Bounds.Width, bestIdx);
                    Rectangle bottom = new Rectangle(line.Bounds.X, line.Bounds.Y + bestIdx, line.Bounds.Width, line.Bounds.Height - bestIdx);
                    lines.Add(new Line(top, GetPrimitiveLetters(bmp, top)));
                    lines.Add(new Line(bottom, GetPrimitiveLetters(bmp, bottom)));
                    change = true;
                }
                description = new TextSection(lines);
            }
            return new TextSection(lines);
        }

        // Dump byte data from a (grayscale) bitmap to pixel array with
        // one byte per pixel.
        internal unsafe static Bytemap ExtractBytes(Bitmap source)
        {
            BitmapData data = source.LockBits(new Rectangle(0, 0, source.Width, source.Height), ImageLockMode.ReadWrite, source.PixelFormat);
            Bytemap bytemap = new Bytemap(new Rectangle(0, 0, source.Width, source.Height));
            byte[] bytes = bytemap.Bytes;
            try
            {
                byte* d = (byte*)data.Scan0;
                for (int i = 0; i < source.Height; i++)
                {
                    for (int j = 0; j < source.Width; j++)
                    {
                        bytes[i * source.Width + j] = d[4 * (i * data.Width + j)];
                    }
                }
            }
            finally
            {
                source.UnlockBits(data);
            }
            return bytemap;
        }

        /// <summary>
        /// Copy a portion of an existing Bytemap into a new Bytemap. The new bytemap
        /// will always have a size of 'size' (typically a NN letter) regardless
        /// of the size of area. Pruning and zero-padding are used to ensure this.
        /// No data outside 'area' is copied.
        /// The rectangles that describe the position of the source and the selected area 
        /// are all relative to the original page bitmap that the OCR is processing.
        /// </summary>
        internal static Bytemap CopyLetter(Bytemap source, Rectangle area, Size size)
        {
            byte[] sourceBytes = source.Bytes;
            int sourceW = source.Frame.Width;
            int dimX = size.Width;
            int dimY = size.Height;
            Bytemap bytemap = new Bytemap(new Rectangle(area.X, area.Y, dimX, dimY));
            byte[] bytes = bytemap.Bytes;
            int min = 255;

            int diffX = area.X - source.Frame.X;
            int diffY = area.Y - source.Frame.Y;

            int untilY = Math.Min(source.Frame.Height - diffY, dimY);
            untilY = Math.Min(area.Height, dimY);
            int untilX = Math.Min(source.Frame.Width - diffX, dimX);
            untilX = Math.Min(area.Width, dimX);
            for (int i = 0; i < untilY; i++)
            {
                for (int j = 0; j < untilX; j++)
                {
                    byte b = sourceBytes[(i + diffY) * sourceW + (j + diffX)];
                    bytes[i * dimX + j] = b;
                    min = Math.Min(b, min);
                }
            }
            for (int i = 0; i < bytes.Length; i++)
            {
                bytes[i] = (byte)Math.Max(0, ((int)bytes[i] - (int)min));                
            }
            return bytemap;
        }

        /// <summary>
        /// Copy a portion of an existing Bytemap into a new Bytemap. The rectangles
        /// that describe the position of the source and the selected area are all
        /// relative to the original page bitmap that the OCR is processing.
        /// </summary>
        public unsafe static Bytemap CopyRectangle(Bytemap source, Rectangle area)
        {
            area.Intersect(source.Frame);
            if (area.IsEmpty)
            {
                return new Bytemap(new Rectangle(area.X, area.Y, Properties.Settings.Default.DimensionX, Properties.Settings.Default.DimensionY));
            }
            byte[] sourceBytes = source.Bytes;
            int sourceW = source.Frame.Width;
            Bytemap dest = new Bytemap(area);
            byte[] destBytes = dest.Bytes;

            int diffX = area.X - source.Frame.X;
            int diffY = area.Y - source.Frame.Y;
            int untilY = Math.Min(source.Frame.Height - diffY, area.Height);
            int untilX = Math.Min(source.Frame.Width - diffX, area.Width);

            for (int i = 0; i < untilY; i++)
            {
                for (int j = 0; j < untilX; j++)
                {
                    byte b = sourceBytes[(i + diffY) * sourceW + (j + diffX)];
                    destBytes[i * area.Width + j] = b;
                }
            }
            return dest;
        }

        internal static PageSections RefinePartition(PageSections pageSections, Bitmap binary)
        {
            Bytemap imageBinary = new Bytemap(binary);
            List<Line> descriptionLines = new List<Line>();
            if (pageSections.DescriptiveText == null)
            {
                return pageSections;
            }
            foreach (Line line in pageSections.DescriptiveText)
            {
                List<Rectangle> accumulate = new List<Rectangle>();
                foreach (Rectangle letter in line)
                {
                    Bytemap letterMask = ImageLetters.CopyRectangle(imageBinary, letter);
                    accumulate.AddRange(ImageLetters.CleanupKerning(letterMask, false));
                }
                descriptionLines.Add(new Line(line.Bounds, accumulate));
            }
            TextSection descriptiveText = new TextSection(descriptionLines);

            // Fix kerning for all text lines - hoping for terraforming and mining resources lines.
            List<TextLineSection> textLines = new List<TextLineSection>();
            foreach (TextLineSection tls in pageSections.TextLines)
            {
                List<Rectangle> accumulate = new List<Rectangle>();
                foreach (Rectangle letter in tls.Line)
                {
                    Bytemap letterMask = ImageLetters.CopyRectangle(imageBinary, letter);
                    accumulate.AddRange(ImageLetters.CleanupKerning(letterMask, false));
                }
                textLines.Add(new TextLineSection(new Line(tls.Line.Bounds, accumulate)));
            }

            return new PageSections(pageSections.Tables, descriptiveText, textLines, pageSections.Excluded, pageSections.Headlines);
        }
    }
}
