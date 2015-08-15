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
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExplOCR
{
    static class ImageProcessing
    {
        public unsafe static void GrayscaleImage(Bitmap bmp)
        {
            BitmapData data = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height), ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb);
            double gamma = 1.0;
            try
            {
                byte* d = (byte*)data.Scan0;
                byte gray = 0;
                int diff1, diff2, diff3;
                for (int i = 0; i < data.Height * data.Width; i++)
                {
                    gray = (byte)((d[4 * i + 0] + d[4 * i + 1] + d[4 * i + 2]) / 3);
                    diff1 = d[4 * i + 0] - d[4 * i + 1];
                    diff2 = d[4 * i + 1] - d[4 * i + 2];
                    diff3 = d[4 * i + 2] - d[4 * i + 0];
                    gray = (byte)(255.0 * Math.Pow(gray / 255.0, gamma));
                    d[4 * i + 0] = gray;
                    d[4 * i + 1] = gray;
                    d[4 * i + 2] = gray;
                    d[4 * i + 3] = 255;
                }
            }
            finally
            {
                bmp.UnlockBits(data);
            }
        }

        // Uses averaging to remove low-frequency artifacts from the image. Used to
        // exploit the fact that system map background is blurred, which reduces its frequency.
        public unsafe static void HighPassImage(Bitmap bmp, int depth, int blank)
        {
            byte[] bytes = new byte[bmp.Width * bmp.Height];
            BitmapData data = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height), ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb);
            try
            {
                byte* d = (byte*)data.Scan0;
                byte gray = 0;

                // Filter is applied to grayscale.
                for (int i = 0; i < data.Height * data.Width; i++)
                {
                    bytes[i] = (byte)((d[4 * i + 0] + d[4 * i + 1] + d[4 * i + 2]) / 3);
                }

                for (int i = 0; i < data.Height * data.Width; i++)
                {
                    gray = bytes[i];
                    int c = 0;
                    int g = 0;
                    for (int j = -depth; j <= depth; j++)
                    {
                        for (int k = -depth; k <= depth; k++)
                        {
                            int idx = i + (j * data.Width) + k;
                            if (idx >= 0 && idx < bytes.Length)
                            {
                                c++;
                                g += bytes[idx];
                            }
                        }
                    }

                    // The amount of pixels specified in blank around the image edge
                    // are set to 0 because the filter produces junk near image edges.
                    if (i < blank * data.Width) gray = 0;
                    if (i >= data.Height * data.Width - blank * data.Width) gray = 0;
                    if (i % data.Width < blank) gray = 0;
                    if ((i + blank) % data.Width < blank) gray = 0;

                    gray = (byte)Math.Max(gray - (g / c), 0);
                    d[4 * i + 0] = gray;
                    d[4 * i + 1] = gray;
                    d[4 * i + 2] = gray;
                    d[4 * i + 3] = 255;
                }
            }
            finally
            {
                bmp.UnlockBits(data);
            }
        }

        public unsafe static void BinarizeImage(Bitmap bmp, double gamma)
        {
            BitmapData data = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height), ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb);
            try
            {
                byte* d = (byte*)data.Scan0;
                byte gray = 0;
                for (int i = 0; i < data.Height * data.Width; i++)
                {
                    gray = (byte)((d[4 * i + 0] + d[4 * i + 1] + d[4 * i + 2]) / 3);
                    if (Math.Abs(d[4 * i + 0] - d[4 * i + 1]) > 50 || Math.Abs(d[4 * i + 1] - d[4 * i + 2]) > 50 || Math.Abs(d[4 * i + 0] - d[4 * i + 2]) > 50)
                    {
                        gray = 0;
                    }
                    gray = (byte)(255.0 * Math.Pow(gray / 255.0, gamma));
                    if (gray < 30) gray = 0;
                    else gray = 255;
                    d[4 * i + 0] = gray;
                    d[4 * i + 1] = gray;
                    d[4 * i + 2] = gray;
                    d[4 * i + 3] = 255;
                }
            }
            finally
            {
                bmp.UnlockBits(data);
            }
        }
    }
}
