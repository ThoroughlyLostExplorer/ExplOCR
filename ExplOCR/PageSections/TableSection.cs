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
using System.Text;

namespace ExplOCR
{
    class TableSection : IEnumerable<Line>,  IPageSection
    {
        public TableSection(IEnumerable<Line> lines)
        {
            this.lines = new List<Line>(lines);
            this.items = new List<int>();

            bounds = new Rectangle();
            foreach (Line line in lines)
            {
                if (bounds.IsEmpty) bounds = line.Bounds;
                bounds = Rectangle.Union(bounds, line.Bounds);
            }

            gap = bounds;
            foreach (Line line in lines)
            {
                if (line.Count == 0) continue;

                bool intersect = false;
                foreach (Rectangle letter in line)
                {
                    if (letter.IntersectsWith(gap))
                    {
                        intersect = true;
                    }
                }
                if (!intersect) continue;

                Rectangle lineGap = GetLineGap(line);
                int begin = line[0].Left;
                int end = line[line.Count - 1].Right;
                if (LineHasGap(gap, lineGap))
                {
                    if (lineGap.Width > 10)
                    {
                        gap = Rectangle.Intersect(gap, lineGap);
                    }
                    else if (begin - bounds.Left > bounds.Right - end && begin > 50)
                    {
                        gap.Width = Math.Max(begin - gap.X, 0);
                    }
                    else if (bounds.Right - end > 50)
                    {
                        gap.Width = Math.Max(gap.Right - end, 0);
                        gap.X = end;
                    }
                    else
                    {
                        gap = Rectangle.Intersect(gap, lineGap);
                    }
                }
                else
                {

                }
            }

            int itemCount = 0;
            if (Count > 0) items.Add(itemCount);
            for (int i = 1 /*sic!*/; i < Count; i++)
            {
                int diff = this.lines[i].Bounds.Top - this.lines[i-1].Bounds.Bottom;
                bool newSection = diff > 17;
                if (newSection)
                {
                    itemCount++;
                }
                items.Add(itemCount);
            }
        }

        private Rectangle GetLineGap(Line line)
        {
            int max = 0;
            Rectangle lineGap = bounds;
            int begin = line[0].Left;
            int end = line[line.Count - 1].Right;
            for (int i = 0; i < line.Count; i++)
            {
                if (i == 0)
                {
                    begin = line[i].Left;
                    if (line[i].Left - bounds.Left > bounds.Right - line[i].Right)
                    {
                        lineGap = new Rectangle(bounds.X, bounds.Y, line[i].Left - bounds.Left, bounds.Height);
                    }
                    else
                    {
                        lineGap = new Rectangle(line[i].Right, bounds.Y, bounds.Right - line[i].Right, bounds.Height);
                    }
                }
                else
                {
                    if (line[i].Left - line[i - 1].Right > max)
                    {
                        max = line[i].Left - line[i - 1].Right;
                        lineGap = new Rectangle(line[i - 1].Right, bounds.Y, max, bounds.Height);
                    }
                }
            }
            return lineGap;
        }

        private bool LineHasGap(Rectangle gap, Rectangle lineGap)
        {
            Rectangle nextGap = Rectangle.Intersect(gap, lineGap);
            return nextGap.Width > 10;
        }

        public bool LineHasGap(int i)
        {
            Rectangle lineGap = GetLineGap(lines[i]);
            return LineHasGap(Gap, lineGap);
        }

        public int Count
        {
            get { return lines.Count; }
        }

        public Rectangle Bounds
        {
            get { return bounds; }
        }

        public Rectangle Gap
        {
            get { return gap; }
        }

        public Line this[int n]
        {
            get { return lines[n]; }
        }

        public bool FirstLineValid
        {
            get
            {
                if (GetLineLeft(0).Count == 0) return false;
                if (GetLineRight(0).Count == 0) return false;
                return LineHasGap(Gap, GetLineGap(this[0]));
            }
        }

        public bool LastLineValid
        {
            get
            {
                if (Count == 0)
                {
                    return false;
                }
                if (Count < 2)
                {
                    if (GetLineLeft(0).Count == 0) return false;
                    if (GetLineRight(0).Count == 0) return false;
                    return true;
                }
                else
                {
                    // Only single-line items can be invalid.
                    if (items[Count - 1] == items[Count - 2]) return true;
                    // Items with mission left/right are invalid.
                    if (GetLineLeft(Count - 1).Count == 0) return false;
                    if (GetLineRight(Count - 1).Count == 0) return false;
                    // Better not consider wether a gap actually exists.
                    return true;
                }
            }
        }

        public int GetLineItem(int n)
        {
            return items[n];
        }

        public Line GetLineLeft(int n)
        {
            List<Rectangle> letters = new List<Rectangle>();
            foreach (Rectangle r in lines[n])
            {
                if (r.Right < gap.Right) letters.Add(r);
            }
            return new Line(bounds, letters);
        }

        public Line GetLineRight(int n)
        {
            List<Rectangle> letters = new List<Rectangle>();
            foreach (Rectangle r in lines[n])
            {
                if (r.Left > gap.Left) letters.Add(r);
            }
            return new Line(bounds, letters);
        }

        public IEnumerator<Line> GetEnumerator()
        {
            return lines.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return lines.GetEnumerator();
        }

        List<Line> lines;
        List<int> items;
        Rectangle bounds;
        Rectangle gap;
    }
}
