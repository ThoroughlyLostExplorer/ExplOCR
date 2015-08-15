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
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExplOCR
{
    static class SimilarityMatch
    {
        public static bool LettersSimilar(char a, char b)
        {
            return a == b;
        }

        public static bool WordsSimilar(string a, string b)
        {
            int i = 0;
            int j = 0;
            int distance = 0;

            a = a.Trim(ListDelimiter);
            b = b.Trim(ListDelimiter);

            for (; i < a.Length && j < b.Length; i++, j++)
            {
                if(LettersSimilar(a[i], b[j]))
                {
                    continue;
                }
                distance++;

                // Try to skip letter in both strings.
                if (i + 1 < a.Length && j + 1 < b.Length)
                {
                    if (LettersSimilar(a[i + 1], b[j + 1]))
                    {
                        i++;
                        j++;
                        continue;
                    }
                }
                // Try to skip a letter in first string.
                if (i + 1 < a.Length)
                {
                    if (LettersSimilar(a[i + 1], b[j]))
                    {
                        i++;
                        continue;
                    }
                }
                // Try to skip a letter in second string.
                if (j + 1 < b.Length)
                {
                    if (LettersSimilar(a[i], b[j+1]))
                    {
                        j++;
                        continue;
                    }
                }
            }

            distance += Math.Max(a.Length - i, 0);
            distance += Math.Max(b.Length - j, 0);

            if (Math.Min(a.Length, b.Length) < Properties.Settings.Default.LongWordThreshold)
            {
                return distance <= 1;
            }
            else
            {
                return distance <= 2;
            }
        }

        public static bool SentencesSimilar(string a, string b)
        {
            return SentencesSimilar(a.Split(ListSpace), b.Split(ListSpace));
        }

        public static bool SentencesSimilar(string[] a, string[] b)
        {
            int allowed = 1 + (Math.Min(a.Length, b.Length) / Properties.Settings.Default.LongWordThreshold);
            return SentenceDistance(a,b) <= allowed;
        }

        public static int SentenceDistance(string[] a, string[] b)
        {
            int i = 0;
            int j = 0;
            int distance = 0;
            for (; i < a.Length && j < b.Length; i++, j++)
            {
                if (WordsSimilar(a[i], b[j]))
                {
                    continue;
                }
                distance++;
                // Try to skip letter in both strings.
                if (i + 1 < a.Length && j + 1 < b.Length)
                {
                    if (WordsSimilar(a[i + 1], b[j + 1]))
                    {
                        i++;
                        j++;
                        continue;
                    }
                }
                // Try to skip a word in first string.
                if (i + 1 < a.Length)
                {
                    if (WordsSimilar(a[i + 1], b[j]))
                    {
                        i++;
                        continue;
                    }
                }
                // Try to skip a word in second string.
                if (j + 1 < b.Length)
                {
                    if (WordsSimilar(a[i], b[j + 1]))
                    {
                        j++;
                        continue;
                    }
                }
            }

            distance += a.Length - i;
            distance += b.Length - j;
            return distance;
        }

        public static bool SentencesSimilarPerWord(string a, string b)
        {
            return SentencesSimilarPerWord(a.Split(ListSpace), b.Split(ListSpace));
        }

        public static bool SentencesSimilarPerWord(string[] a, string[] b)
        {
            if (a.Length != b.Length)
            {
                return false;
            }
            for (int i = 0; i < a.Length; i++)
            {
                if (!WordsSimilar(a[i], b[i])) return false;
            }
            return true;
        }

        internal static bool HasSimilarWord(string a, string b)
        {
            return HasSimilarWord(a.Split(ListSpace), b);
        }

        private static bool HasSimilarWord(string[] a, string b)
        {
            for (int i = 0; i < a.Length; i++)
            {
                if (WordsSimilar(a[i], b)) return true;
            }
            return false;
        }

        public static string GuessWord(string a, string[] words)
        {
            for (int i = 0; i < words.Length; i++)
            {
                if (a == words[i]) return a;
            }
            for (int i = 0; i < words.Length; i++)
            {
                if (WordsSimilar(a, words[i]))
                {
                    return words[i];
                }
            }
            return a;
        }

        public static string GuessWords(string a, string[] words)
        {
            return string.Join(" ", GuessWords(a.Split(ListSpace, StringSplitOptions.RemoveEmptyEntries), words));
        }

        public static string[] GuessWords(string[] a, string[] words)
        {
            string[] b = new string[a.Length];
            for (int i = 0; i < a.Length; i++)
            {
                b[i] = GuessWord(a[i], words);
            }
            return b;
        }

        static char[] ListSpace = new char[] { ' ' };
        static char[] ListDelimiter = new char[] { '.', ',', ':', ';' };
    }
}
