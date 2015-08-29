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
    static class TeachLetters
    {
        internal static string ProcessLetter(Bitmap source, int screen, char c, Rectangle r)
        {
            if (c == ',') c = '#';
            byte[] letterBytes = ImageLetters.GetLetterImage(source, r, new Size(Properties.Settings.Default.DimensionX, Properties.Settings.Default.DimensionY));
            string s = LetterInfo.WriteLetterInfoLine(c, r, screen, Convert.ToBase64String(letterBytes));

            string name;
            if (char.IsLetter(c))
            {
                name = c + (char.IsUpper(c) ? "_upper" : "_lower");
            }
            else if (char.IsNumber(c))
            {
                name = c.ToString();
            }
            else
            {
                name = "delimiter";
            }
            if (!File.Exists(PathHelpers.BuildTeachFilename(name)))
            {
                Directory.CreateDirectory(PathHelpers.BuildTeachDirectory());
                File.WriteAllText(PathHelpers.BuildTeachFilename(name), "");
            }
            if (!File.Exists(PathHelpers.BuildTeachBaseFilename()))
            {
                Directory.CreateDirectory(PathHelpers.BuildTeachDirectory());
                File.WriteAllText(PathHelpers.BuildTeachBaseFilename(), "");
            }
            File.AppendAllText(PathHelpers.BuildTeachFilename(name), s + Environment.NewLine);
            File.AppendAllText(PathHelpers.BuildTeachBaseFilename(), s + Environment.NewLine);
            return s + Environment.NewLine;
        }

        internal static void ProcessWords(Bitmap source, int screen, string text, Point location, PageSections pageSections)
        {
            List<List<Rectangle>> imageWords = new List<List<Rectangle>>();
            string[] textWords = text.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);


            List<Rectangle> letters = new List<Rectangle>();
            bool active = false;
            bool inDescription = pageSections.DescriptiveText.Bounds.Contains(location);
            for (int j = 0, i = 0; i < pageSections.AllLetters.Count; i++)
            {
                if (pageSections.AllLetters[i].Contains(location))
                {
                    active = true;
                }

                if (active)
                {
                    if (j > 0 && ImageLetters.IsNewWord(pageSections.AllLetters, i, inDescription))
                    {
                        imageWords.Add(letters);
                        letters = new List<Rectangle>();
                    }
                    letters.Add(pageSections.AllLetters[i]);
                    j++;
                }
            }
            imageWords.Add(letters);


            for (int i = 0; i < Math.Min(imageWords.Count, textWords.Length); i++)
            {
                // Test if words in text and gaps in bitmap text match up.
                // We need this to reject any text where letters were not isolated properly.
                if (imageWords[i].Count != textWords[i].Length)
                {
                    continue;
                }

                for (int j = 0; j < textWords[i].Length; j++)
                {
                    TeachLetters.ProcessLetter(source, screen, textWords[i][j], imageWords[i][j]);
                }
            }

            return;
        }

    }
}
