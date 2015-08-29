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
    class TextSection : IEnumerable<Line>, IPageSection
    {
        public TextSection(IEnumerable<Line> lines)
        {
            this.lines = new List<Line>(lines);

            bounds = new Rectangle();
            foreach (Line line in lines)
            {
                if (bounds.IsEmpty) bounds = line.Bounds;
                bounds = Rectangle.Union(bounds, line.Bounds);
            }
        }

        public int Count
        {
            get { return lines.Count; }
        }

        public Rectangle Bounds
        {
            get { return bounds; }
        }

        public Line this[int n]
        {
            get { return lines[n]; }
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
        Rectangle bounds;
    }

}
