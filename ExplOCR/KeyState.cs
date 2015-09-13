using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ExplOCR
{
    static class KeyState
    {
        public static bool IsKeyDown(Keys key)
        {
            short keyValue = GetKeyState((int)key);
            return (keyValue & 0x8000) != 0;
        }

        [DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
        private static extern short GetKeyState(int keyCode);
    }
}
