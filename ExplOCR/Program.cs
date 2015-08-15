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
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ExplOCR
{
    static class Program
    {
        /// <summary>
        /// Der Haupteinstiegspunkt für die Anwendung.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            if (args.Length == 0)
            {
                Application.Run(new FrmUser());
            }
            else if (args[0] == "private")
            {
                Application.Run(new FrmMain());
            }
            else if( args.Length == 2)
            {
                using (OcrReader ocrReader = LibExplOCR.CreateOcrReader())
                {
                    LibExplOCR.ProcessImageFile(ocrReader, args[0], false);
                    if (Path.GetExtension(args[1]).ToLower() == ".xml")
                    {
                        File.WriteAllText(args[1], ocrReader.GetDataXML());
                    }
                    else
                    {
                        File.WriteAllText(args[1], ocrReader.GetDataText());
                    }
                }
            }
        }
    }
}
