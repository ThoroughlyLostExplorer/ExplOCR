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
    class Line : IEnumerable<Rectangle>
    {
        public Line(Rectangle bounds, IEnumerable<Rectangle> letters)
        {
            this.letters = new List<Rectangle>(letters);
            this.bounds = bounds;
        }

        public int Count
        {
            get { return letters.Count; }
        }

        public Rectangle Bounds
        {
            get { return bounds; }
        }

        public Rectangle this[int n]
        {
            get { return letters[n]; }
        }

        public IEnumerator<Rectangle> GetEnumerator()
        {
            return letters.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return letters.GetEnumerator();
        }

        List<Rectangle> letters;
        Rectangle bounds;
    }
}
