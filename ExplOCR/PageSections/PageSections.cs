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
        public PageSections(List<TableSection> tables, TextSection text, List<TextLineSection> textLines, List<ExcludeSection> exclude, List<HeadlineSection> headlines)
        {
            this.tables = new List<TableSection>(tables);
            this.headlines = new List<HeadlineSection>(headlines);
            this.textLines = new List<TextLineSection>(textLines);
            this.descriptiveText = text;
            this.excluded =  new List<ExcludeSection>(exclude);
            this.allLines = new List<Line>();
            this.allLetters = new List<Rectangle>();
            this.allSections = new List<IPageSection>();

            if (text != null)
            {
                allLines.AddRange(text);
                allSections.Add(text);
            }
            foreach (TableSection t in tables)
            {
                allLines.AddRange(t);
            }
            foreach (TextLineSection t in textLines)
            {
                allLines.Add(t.Line);
            }
            foreach (HeadlineSection t in headlines)
            {
                allLines.Add(t.Line);
            }
            foreach (Line line in AllLines)
            {
                allLetters.AddRange(line);
            }

            allSections.AddRange(tables);
            allSections.AddRange(headlines);
            allSections.AddRange(textLines);
            allSections.AddRange(excluded);
            allSections.Sort(CompareSections);
        }

        public List<TableSection> Tables
        {
            get { return tables; }
        }

        public List<TextLineSection> TextLines
        {
            get { return textLines; }
        }

        public List<HeadlineSection> Headlines
        {
            get { return headlines; }
        }

        public TextSection DescriptiveText
        {
            get { return descriptiveText; }
        }

        public List<ExcludeSection> Excluded
        {
            get { return excluded; }
        }

        public List<IPageSection> AllSections
        {
            get { return allSections; }
        }

        public List<Rectangle> AllLetters
        {
            get { return allLetters; }
        }

        public List<Line> AllLines
        {
            get { return allLines; }
        }

        int CompareSections(IPageSection a, IPageSection b)
        {
            return a.Bounds.Top.CompareTo(b.Bounds.Top);
        }

        public List<TableSection> tables;
        public List<TextLineSection> textLines;
        public List<HeadlineSection> headlines;
        public TextSection descriptiveText;
        public List<IPageSection> allSections;
        public List<Rectangle> allLetters;
        public List<Line> allLines;
        public List<ExcludeSection> excluded;
    }
}
