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
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace ExplOCR
{
    public static class OutputConverter
    {
        static OutputConverter()
        {
            descriptionConfig = DescriptionItem.Load(PathHelpers.BuildConfigFilename("Descriptions"));
        }

        public static string GetDataXML(TransferItem[] array)
        {
            XmlSerializer ser = new XmlSerializer(typeof(TransferItem[]));
            StringWriter writer = new StringWriter();
            ser.Serialize(writer, array);
            return writer.ToString();
        }

        internal static string GetDataBodyCode(string systemName, TransferItem[] array)
        {
            string[] systemParts = systemName.Split(new char[] { ' ' });
            foreach (TransferItem ti in array)
            {
                if (ti.Name != WellKnownItems.Headline)
                {
                    continue;
                }

                string[] headlineParts = ti.Values[0].Text.Split(new char[] { ' ' });
                string value = "";
                for (int i = systemParts.Length; i < headlineParts.Length; i++)
                {
                    value += headlineParts[i] + " ";
                }
                return value.Trim();
            }
            return "";
        }

        public static string GetDataText(TransferItem[] array)
        {
            string output = "";
            foreach (TransferItem ti in array)
            {
                if (ti == null)
                {
                    output += "??????" + Environment.NewLine;
                    continue;
                }
                if (ti.Name == "DELIMITER" || ti.Name == WellKnownItems.ArchiveName)
                {
                    continue;
                }
                if (ti.Name == WellKnownItems.CustomDescription || ti.Name == WellKnownItems.CustomCategory)
                {
                    continue;
                }
                if (ti.Name == WellKnownItems.ScanDate)
                {
                    continue;
                }
                output += ti.Name;

                bool first = true;
                foreach (TransferItemValue tv in ti.Values)
                {
                    string offset = first ? ti.Name : "";
                    first = false;
                    if (ti.Name == WellKnownItems.Description)
                    {
                        output += GetSpaces(offset) + LookupShortDescription(tv.Text) + Environment.NewLine;
                        continue;
                    }
                    if (double.IsNaN(tv.Value))
                    {
                        output += GetSpaces(offset) + tv.Text.ToString() + Environment.NewLine;
                    }
                    else
                    {
                        string format = tv.Value > 1e10 ? tv.Value.ToString("e") : tv.Value.ToString();
                        output += GetSpaces(offset) + "[" + format + "]";
                        if (!string.IsNullOrEmpty(tv.Unit) && Temporary.UnitNames.ContainsKey(tv.Unit))
                        {
                            output += " " + Temporary.UnitNames[tv.Unit];
                        }
                        if (!string.IsNullOrEmpty(tv.Text))
                        {
                            output += " " + tv.Text;
                        }
                        output += Environment.NewLine;
                    }

                }
            }
            return output;
        }

        public static string GetDataTextClassic(TransferItem[] array)
        {
            string output = "";
            foreach (TransferItem ti in array)
            {
                if (ti == null)
                {
                    continue;
                }
                if (ti.Name == WellKnownItems.Description && ti.Values.Count > 0)
                {
                    output += ti.Values[0].Text + Environment.NewLine;
                }
            }
            foreach (TransferItem ti in array)
            {
                if (ti == null)
                {
                    output += "??????" + Environment.NewLine;
                    continue;
                }
                if (ti.Name == WellKnownItems.Description)
                {
                    continue;
                }
                if (ti.Name == "DELIMITER")
                {
                    output += "DELIMITER" + Environment.NewLine;
                    continue;
                }
                if (ti.Name == WellKnownItems.ScanDate)
                {
                    continue;
                }
                output += ti.Name;
                foreach (TransferItemValue tv in ti.Values)
                {
                    if (double.IsNaN(tv.Value))
                    {
                        output += "        " + tv.Text.ToString() + Environment.NewLine;
                    }
                    else
                    {
                        string format = tv.Value > 1e10 ? tv.Value.ToString("e") : tv.Value.ToString();
                        output += "        " + "[" + format + "]";
                        if (!string.IsNullOrEmpty(tv.Unit) && Temporary.UnitNames.ContainsKey(tv.Unit))
                        {
                            output += " " + Temporary.UnitNames[tv.Unit];
                        }
                        if (!string.IsNullOrEmpty(tv.Text))
                        {
                            output += " " + tv.Text;
                        }
                        output += Environment.NewLine;
                    }
                }
            }
            return output;
        }

        private static string GetSpaces(string name)
        {
            int count = 20 - name.Length;
            count = Math.Min(20, Math.Max(0, count));
            return new string(' ', count);
        }

        private static string LookupShortDescription(string code)
        {
            foreach (DescriptionItem di in descriptionConfig)
            {
                if (code == di.Name)
                {
                    return di.Short;
                }
            }
            return "?";
        }

        static DescriptionItem[] descriptionConfig;
    }
}
