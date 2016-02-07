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

            try
            {
                MainMethod(args);
            }
            catch (Exception exception)
            {
                HandleException(exception);
            }
        }

        private static void MainMethod(string[] args)
        {
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;

            if (args.Length == 0)
            {
                Application.Run(new FrmUser());
            }
            else if (args[0] == "private")
            {
                Application.Run(new FrmMain());
            }
            else if (args[0] == "multiread")
            {
                Application.Run(new FrmUser());
            }
            else if (args.Length == 2)
            {
                using (OcrReader ocrReader = LibExplOCR.CreateOcrReader())
                {
                    LibExplOCR.ProcessImageFile(ocrReader, args[0]);
                    if (Path.GetExtension(args[1]).ToLower() == ".xml")
                    {
                        File.WriteAllText(args[1], OutputConverter.GetDataXML(ocrReader.Items));
                    }
                    else
                    {
                        File.WriteAllText(args[1], OutputConverter.GetDataText(ocrReader.Items));
                    }
                }
            }
        }

        private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            HandleException(e.ExceptionObject as Exception);
        }

        private static void HandleException(Exception exception)
        {
            using (FrmCrash form = new FrmCrash())
            {
                form.SetMessage(exception);
                form.ShowDialog();
            }
        }
    }
}
