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
    public static class ImageFiles
    {
        public static Bitmap LoadImageFile(string file)
        {
            if (!File.Exists(file))
            {
                return null;
            }
            Bitmap fromFile = new Bitmap(file);
            int scrX = ExplOCR.Properties.Settings.Default.ScreenshotX;
            int scrY = ExplOCR.Properties.Settings.Default.ScreenshotY;
            int scrW = ExplOCR.Properties.Settings.Default.ScreenshotW;
            int scrH = ExplOCR.Properties.Settings.Default.ScreenshotH;
            Rectangle screenshot = new Rectangle(scrX, scrY, scrW, scrH);
            if (fromFile.Width < scrW + 10)
            {
                // My pre-formatted testing screenshots, debugging only
                return fromFile;
            }

            if (Rectangle.Intersect(new Rectangle(0, 0, fromFile.Width, fromFile.Height), screenshot) == screenshot)
            {
                return fromFile.Clone(new Rectangle(scrX, scrY, scrW, scrH), fromFile.PixelFormat);
            }
            else
            {
                scrW = Math.Min(scrW, fromFile.Width - scrX);
                scrH = Math.Min(scrH, fromFile.Height - scrY);
                if (scrW < 0 || scrH < 0)
                {
                    return fromFile;
                }
                else
                {
                    return fromFile.Clone(new Rectangle(scrX, scrY, scrW, scrH), fromFile.PixelFormat);
                }
            }
        }

    }
}
