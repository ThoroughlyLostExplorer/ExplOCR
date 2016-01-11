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

namespace ExplOCR
{
    static class PathHelpers
    {
        public static string BuildConfigDirectory()
        {
            return Path.Combine(Path.GetDirectoryName(typeof(FrmMain).Assembly.Location),"Configuration");
        }

        public static string BuildScreenDirectory()
        {
            if (string.IsNullOrEmpty(Properties.Settings.Default.UserScreenshotDirectory))
            {
                return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "ExplOCR_Screenshots");
            }
            else
            {
                return Properties.Settings.Default.UserScreenshotDirectory;
            }
        }

        public static string BuildKnowledgeDirectory(string type)
        {
            return Path.Combine(Path.Combine(PathBase, "knowledge"), type); ;
        }

        public static string BuildSaveDirectory()
        {
            return Path.Combine(PathBase, "save");
        }

        public static string BuildTeachDirectory()
        {
            return Path.Combine(PathBase, "teach");
        }

        public static string BuildWordsDirectory()
        {
            return Path.Combine(PathBase, "words");
        }

        public static string BuildAutoTestDirectory()
        {
            return Path.Combine(PathBase, "autotest\\screenshots");
        }

        public static string BuildConfigFilename(string name)
        {
            return Path.Combine(BuildConfigDirectory(), name + ".xml");
        }

        public static string BuildScreenFilename(int screen)
        {
            if (screen < 10000)
            {
                return Path.Combine(BuildScreenDirectory(), "sysmap" + screen.ToString("D4") + ".png");
            }
            else
            {
                return Path.Combine(BuildScreenDirectory(), "sysmap" + screen.ToString("D8") + ".png");
            }
        }

        public static int GetFileNumber(string file)
        {
            int num;
            if (!file.StartsWith("sysmap") || !file.EndsWith(".png"))
            {
                return -1;
            }
            if (!int.TryParse(file.Substring("sysmap".Length, file.Length - "sysmap".Length - ".png".Length), out num))
            {
                return -1;
            }
            return num;
        }

        public static string BuildTeachBaseFilename()
        {
            return Path.Combine(BuildTeachDirectory(), "letters.txt");
        }

        public static string BuildTeachFilename(string name)
        {
            return Path.Combine(BuildTeachDirectory(), name+".txt");
        }

        public static string BuildNetworkFilename(string name)
        {
            return Path.Combine(BuildSaveDirectory(), name+".xml");
        }

        public static string BuildKnowledgeFilename(string type, string name)
        {
            return Path.Combine(BuildKnowledgeDirectory(type), name + ".txt");
        }

        public static string BuildWordFilename(string name)
        {
            string dir = Path.Combine(PathBase, "words");
            return Path.Combine(BuildWordsDirectory(), name + ".txt");
        }

        public static string BuildAutoTestFilename(int screen)
        {
            return Path.Combine(BuildAutoTestDirectory(), "sysmap" + screen.ToString("D4") + ".png");
        }
        
        static string PathBase
        {
            get 
            {
                return Path.Combine(Path.GetDirectoryName(typeof(PathHelpers).Assembly.Location), Properties.Settings.Default.PathBase);
            }
        }

        internal static string BuildUserSaveFilename()
        {
            return Path.Combine(BuildUserSaveDirectory(), "systems.xml");
        }

        internal static string BuildUserSaveDirectory()
        {
            if (string.IsNullOrEmpty(Properties.Settings.Default.UserSaveDirectory))
            {
                return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "ExplOCR");
            }
            else
            {
                return Properties.Settings.Default.UserSaveDirectory;
            }
        }

        internal static void ConfigureScreenDirectory(string p)
        {
            Properties.Settings.Default.UserScreenshotDirectory = p;
            Properties.Settings.Default.Save();
        }

        internal static void ConfigureUserSaveDirectory(string p)
        {
            Properties.Settings.Default.UserSaveDirectory = p;
            Properties.Settings.Default.Save();
        }
    }
}
