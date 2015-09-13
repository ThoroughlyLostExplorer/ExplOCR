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
using System.Xml.Serialization;

namespace ExplOCR
{
    struct LetterInfo
    {
        public bool Invalid;
        public int Screen;
        public int X;
        public int Y;
        public string Base64;
        public char Char;

        internal static LetterInfo ReadLetterInfoLine(string line)
        {
            LetterInfo info = new LetterInfo();
            string[] sub = line.Split(new char[] { ',' });

            if (sub.Length < LetterLength || sub[indexChar].Length == 0)
            {
                info.Invalid = true;
            }
            else
            {
                int.TryParse(sub[indexPage], out info.Screen);
                int.TryParse(sub[indexCoordX], out info.X);
                int.TryParse(sub[indexCoordY], out info.Y);
                info.Char = sub[indexChar][0];
                info.Base64 = sub[indexBits];
            }
            return info;
        }

        const int LetterLength = 5;

        const int indexBits = 0;
        const int indexPage = 1;
        const int indexCoordX = 2;
        const int indexCoordY = 3;
        const int indexChar = 4;

        internal static string WriteLetterInfoLine(char c, Rectangle frame, int screen, string base64)
        {
            Point p = new Point((frame.Left + frame.Right) / 2, (frame.Top + frame.Bottom) / 2);
            return base64 + "," + screen.ToString() + "," + p.X.ToString() + "," + p.Y.ToString() + "," + c;
        }
    }

    public struct QualityData
    {
        public QualityData(double q, Rectangle f)
        {
            Quality = q;
            Frame = f;
        }

        public Rectangle Frame;
        public double Quality;
    }

    public class Temporary
    {
        public static Dictionary<string, string> UnitNames = new Dictionary<string, string>
        { 
            { "MY","Million Years" },
            { "K", "K"},
            { "UNITLESS", "" },
            { "AT" , "ATMOSPHERES" },
            { "KM","KM" },
            { "PERCENT", "%"},
            { "DAYS", "D" },
            { "AU", "AU" },
            { "DEGREES", "DEG" },
            { "LITERAL",""},
            { "MT","MT"}
        };

        public static string[] VolcanismTypes = new string[] {
            "NO VOLCANISM",
            "IRON MAGMA",
            "SILICATE MAGMA",
            "WATER MAGMA",
            "SILICATE VAPOUR GEYSERS",
            "CARBON DIOXIDE GEYSERS",
            "WATER GEYSERS",
            "METHANE MAGMA",
            "NITROGEN MAGMA"
        };

        public static string[] AtmosphereTypes = new string[] {
            "NO ATMOSPHERE",
            "SUITABLE FOR WATER BASED LIFE",
            "NITROGEN",
            "CARBON DIOXIDE",
            "SULPHUR DIOXIDE",
            "ARGON",
            "NEON",
            "NEON-RICH",
            "ARGON-RICH",
            "NITROGEN-RICH",
            "WATER-RICH",
            "CARBON DIOXIDE-RICH",
            "METHANE-RICH",
            "SILICATE VAPOUR",
            "METHANE",
            "HELIUM",
            "AMMONIA",
            "AMMONIA AND OXYGEN",
            "WATER",
        };

        public static string[] AtmosphereComponents = new string[] {
            "NITROGEN",
            "OXYGEN",
            "WATER",
            "NEON",
            "CARBON DIOXIDE",
            "AMMONIA",
            "METHANE",
            "SULPHUR DIOXIDE",
            "HYDROGEN",
            "HELIUM",
            "ARGON",
            "IRON",
            "SILICATES"
        };

        public static string[] SolidComponents = new string[] {
            "METAL",
            "ROCK",
            "ICE",
        };

        public static string[] RingTypes = new string[] {
            "METALLIC",
            "METAL RICH",
            "ROCKY",
            "ICY"
        };

        public static string[] MiningReserves = new string[] {
            "Pristine reserves",
            "Major reserves",
            "Common reserves",
            "Low reserves",
            "Depleted reserves"
        };

        public static string[] Terraforming = new string[] {
            "This planet is a candidate for terraforming.",
            "This planet has been terraformed."
        };
    }

    public class TransferItem
    {
        public TransferItem()
        {
            Name = "";
        }

        public TransferItem(string name)
        {
            Name = name;
        }

        public static TransferItem FindItem(string name, TransferItem[] items)
        {
            foreach (TransferItem item in items)
            {
                if (item.Name == name) return item;
            }
            return null;
        }

        internal static TransferItem FindItem(string name, TransferItem[] items, int num)
        {
            int count = 0;
            foreach (TransferItem item in items)
            {
                if (item.Name == name)
                {
                    count++;
                }
                if (count == num)
                {
                    return item;
                }
            }
            return null;
        }

        public string Name = "";
        public List<TransferItemValue> Values = new List<TransferItemValue>();
    }

    public class TransferItemValue
    {
        public TransferItemValue()
        {
        }

        public TransferItemValue(string text, double value, string unit)
        {
            Text = text;
            Value = value;
            Unit = unit;
        }

        public TransferItemValue(string text)
        {
            Text = text;
            Value = double.NaN;
            Unit = "";
        }

        public string Text = "";
        public string Unit = "";
        public double Value = 0;
    }

    public class WellKnownItems
    {
        public const string System = "SYSTEM";
        public const string BodyCode = "BODY";
        public const string Description = "DESCRIPTION";
        public const string CustomDescription = "CUSTOM_DESCRIPTION";
        public const string CustomCategory = "CUSTOM_CATEGORY";
        public const string ArchiveName = "ARCHIVE_NAME";
        public const string Headline = "HEADLINE";
        public const string ScanDate = "META_DATE";
    }

    public class Bytemap
    {
        public Bytemap(Rectangle frame)
        {
            this.frame = frame;
            bytes = new byte[frame.Width * frame.Height];
        }

        public Bytemap(Bitmap bmp)
        {
            Bytemap bytemap = ImageLetters.ExtractBytes(bmp);
            frame = bytemap.Frame;
            bytes = bytemap.Bytes;
        }

        public Rectangle Frame
        {
            get { return frame; }
        }

        public byte[] Bytes
        {
            get { return bytes; }
        }

        byte[] bytes;
        Rectangle frame;
    }
}
