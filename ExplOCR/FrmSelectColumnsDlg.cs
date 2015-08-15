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
    public partial class FrmSelectColumnsDlg : Form
    {
        public FrmSelectColumnsDlg()
        {
            InitializeComponent();

        }

        public DataGridView Grid
        {
            get
            {
                return grid;
            }
            set
            {
                grid = value;
                checkedList.Items.Clear();
                foreach (DataGridViewColumn column in grid.Columns)
                {
                    checkedList.Items.Add(column.Name, column.Visible);
                }
            }
        }

        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            base.OnFormClosed(e);

            if (DialogResult != System.Windows.Forms.DialogResult.OK)
            {
                return;
            }

            for (int i = 0; i < checkedList.Items.Count; i++)
            {
                if (!grid.Columns.Contains(checkedList.Items[i] as string))
                {
                    continue;
                }
                grid.Columns[checkedList.Items[i]as string].Visible = checkedList.GetItemChecked(i);
            }
        }

        DataGridView grid;

        private void buttonCheckAll_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < checkedList.Items.Count; i++)
            {
                checkedList.SetItemChecked(i, true);
            }
        }

        private void buttonUncheckAll_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < checkedList.Items.Count; i++)
            {
                checkedList.SetItemChecked(i, false);
            }
        }

        private void buttonInvertAll_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < checkedList.Items.Count; i++)
            {
                checkedList.SetItemChecked(i, !checkedList.GetItemChecked(i));
            }
        }
    }
}
