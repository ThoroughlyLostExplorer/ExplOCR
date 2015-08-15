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
using System.Text;

namespace ExplOCR
{
    class PageSections
    {
        public PageSections(List<TableSection> tables, TextSection text, List<TextLineSection> textLines, List<Rectangle> exclude)
        {
            Tables = new List<TableSection>(tables);
            TextLines = textLines;
            DescriptiveText = text;
            Excluded = exclude;
            AllLines = new List<Line>();
            if (text != null)
            {
                AllLines.AddRange(text);
            }
            foreach (TableSection t in tables)
            {
                AllLines.AddRange(t);
            }
            foreach (TextLineSection t in textLines)
            {
                AllLines.Add(t.Line);
            }
            AllLetters = new List<Rectangle>();
            foreach (Line line in AllLines)
            {
                AllLetters.AddRange(line);
            }
        }

        public List<TableSection> Tables;
        public List<TextLineSection> TextLines;
        public TextSection DescriptiveText;
        public List<Rectangle> AllLetters;
        public List<Line> AllLines;
        public List<Rectangle> Excluded;
    }
}
