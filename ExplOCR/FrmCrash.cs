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
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ExplOCR
{
    public partial class FrmCrash : Form
    {
        public FrmCrash()
        {
            InitializeComponent();
        }

        public void SetMessage(Exception ex)
        {
            if (ex == null) return;

            textBox.Text = "";

            do
            {
                textBox.Text += "Exception " + ex.GetType().ToString() + Environment.NewLine +
                   ex.Message + Environment.NewLine + ex.StackTrace + Environment.NewLine;
                ex = ex.InnerException;
            } while (ex != null);
        }
    }
}
