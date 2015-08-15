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
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExplOCR
{
    class ContextAnalysis
    {
        public static PageSections PartitionScreen(Bitmap bmp)
        {
            List<Rectangle> lines = ImageLetters.GetPrimitiveLines(bmp);
            List<Line> lineObj = new List<Line>();
            foreach (Rectangle line in lines)
            {
                List<Rectangle> letters = ImageLetters.GetPrimitiveLetters(bmp, line);
                lineObj.Add(new Line(line, letters));
            }
            return ContextAnalysis.PartitionScreen(bmp, lineObj);
        }

        public static PageSections PartitionScreen(Bitmap bmp, List<Line> lines)
        {
            if (lines.Count <= 1)
            {
                return null;
            }

            List<Rectangle> exclude = DetectExclusionAreas(lines, bmp.Height);

            List<TableSection> table = new List<TableSection>();
            List<TextLineSection> textLines = new List<TextLineSection>();
            //List<Line> separator = new List<TextLineSection>();
            TextSection description = null;

            lines = CopyExcluded(lines, exclude);
            List<Line> lineQueue = new List<Line>(lines);

            while (lineQueue.Count > 0)
            {
                if (IsSeparator(lineQueue[0]))
                {
                    //separator.Add(lineQueue[0]);
                    lineQueue.RemoveAt(0);
                    continue;
                }
                TextSection textSection = ReadTextSection(lineQueue);
                if (textSection != null)
                {
                    if (description == null) description = textSection;
                    lineQueue.RemoveRange(0, textSection.Count);
                    continue;
                }
                TableSection tableSection = ReadTableSection(lineQueue);
                if (tableSection != null)
                {
                    table.Add(tableSection);
                    lineQueue.RemoveRange(0, tableSection.Count);
                    continue;
                }
                TextLineSection textLineSection = ReadTextLineSection(lineQueue);
                if (textLineSection != null)
                {
                    textLines.Add(textLineSection);
                    lineQueue.RemoveAt(0);
                    continue;
                }
                lineQueue.RemoveAt(0);
            }

            // Lines in text sections have a tendency to stick together because
            // the gap separating them is not very pronounced.
            if (description != null)
            {
                description = ImageLetters.ImproveDescriptionLines(bmp, description);
            }
            return new PageSections(table, description, textLines, exclude);
        }

        private static List<Line> CopyExcluded(List<Line> lines, List<Rectangle> exclude)
        {
            if(exclude.Count == 0)
            {
                return new List<Line>(lines);
            }
            List<Line> reduced = new List<Line>(lines.Count);
            foreach (Line line in lines)
            {
                bool use = true;
                foreach (Rectangle r in exclude)
                {
                    if (Rectangle.Intersect(line.Bounds, r).Height > 0)
                    {
                        use = false;
                        break;
                    }
                }
                if (use)
                {
                    reduced.Add(line);
                }
            }
            return reduced;
        }

        private static List<Rectangle> DetectExclusionAreas(List<Line> lines, int height)
        {
            List<Rectangle> exclude = new List<Rectangle>();
            Line infoline = null;
            foreach (Line line in lines)
            {
                // Match the "info" tab line, which typically shows up as a 
                // single, very large letter.
                if (line.Count > 10)
                {
                    continue;
                }
                // Infoline is never in lower half of screen.
                if (line.Bounds.Bottom > height / 2)
                {
                    continue;
                }
                foreach (Rectangle letter in line)
                {
                    if (letter.Width > 100 && letter.Height > 20)
                    {
                        infoline = line;
                    }
                }
            }

            if (infoline != null)
            {
                exclude.Add(new Rectangle(infoline.Bounds.X, 0, infoline.Bounds.Width, infoline.Bounds.Bottom));
            }
            return exclude;
        }

        private static TextLineSection ReadTextLineSection(List<Line> lines)
        {
            if (lines.Count == 0)
            {
                return null;
            }
            return new TextLineSection(lines[0]);
        }

        private static TextSection ReadTextSection(List<Line> lines)
        {
            List<Line> section = new List<Line>();
            foreach (Line line in lines)
            {
                if (section.Count > 1)
                {
                    List<Line> tmp = new List<Line>(section);
                    tmp.Add(line);

                    if (IsTextSection(section) && !IsTextSection(tmp))
                    {
                        return new TextSection(section);
                    }

                }
                section.Add(line);
            }
            if (IsTextSection(section))
            {
                return new TextSection(section);
            }
            else
            {
                return null;
            }
        }

        private static TableSection ReadTableSection(List<Line> lines)
        {
            List<Line> section = new List<Line>();
            foreach (Line line in lines)
            {
                if (section.Count > 1)
                {
                    List<Line> tmp = new List<Line>(section);
                    tmp.Add(line);

                    if (IsTableSection(section) && !IsTableSection(tmp))
                    {
                        return new TableSection(section);
                    }

                }
                section.Add(line);
            }
            if (IsTableSection(section))
            {
                return new TableSection(section);
            }
            else
            {
                return null;
            }
        }

        private static bool IsTextSection(List<Line> section)
        {
            if (section.Count < 2) return false;
            foreach (Line line in section)
            {
                if (IsSeparator(line))
                {
                    return false;
                }
            }

            for (int i = 1 /*sic!*/; i < section.Count; i++)
            {
                if (section[i].Bounds.Top - section[i - 1].Bounds.Bottom >= 8)
                {
                    return false;
                }
            }

            int rightEdge = 0;
            int rightEdgeSection = 0;
            foreach (Line line in section)
            {
                rightEdgeSection = Math.Max(rightEdgeSection, line.Bounds.Right);
                foreach (Rectangle r in line)
                {
                    rightEdge = Math.Max(rightEdge, r.Right);
                }
            }

            // This will skip sections that are not word-wrapped text.
            // TODO: Improve. This will keep us from mistaking discovery tags for description text.
            if (rightEdge < 2 * rightEdgeSection / 3)
            {
                return false;
            }
            return true;
        }

        private static bool IsTableSection(List<Line> section)
        {
            if (section.Count < 2) return false;

            Rectangle previous = section[0].Bounds;
            foreach (Line line in section)
            {
                if (IsSeparator(line))
                {
                    return false;
                }
                if (line.Bounds.Top - previous.Bottom > line.Bounds.Height + 5)
                {
                    return false;
                }
                previous = line.Bounds;
            }

            TableSection t = new TableSection(section);

            return t.Gap.Width > 10 && t.Gap.Right < section[0].Bounds.Right - 5;
        }

        private static bool IsSeparator(Line line)
        {
            if (line.Bounds.Height > 6)
            {
                return false;
            }
            foreach (Rectangle letter in line)
            {
                if (letter.Width > 50) return true;
            }
            return false;
        }
    }
}
