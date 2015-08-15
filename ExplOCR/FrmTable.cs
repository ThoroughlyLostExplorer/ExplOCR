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
    public partial class FrmTable : Form
    {
        public FrmTable()
        {
            InitializeComponent();

            dataSet.Tables.Add(dataTable);
            dataView.Table = dataTable;
            dataGrid.DataSource = dataView;

            UpdateUI();
        }

        public void SetValues(TransferItem[][] items)
        {
            foreach (TransferItem[] body in items)
            {
                foreach (TransferItem item in body)
                {
                    if (item == null) continue;
                    if (item.Values.Count != 1)
                    {
                        if (item.Name == "ATMOSPHERE" || item.Name == "COMPOSITION")
                        {
                            for (int i = 0; i < 3; i++)
                            {
                                if (dataTable.Columns.Contains(item.Name + "_" + (i + 1).ToString() + "_NAME"))
                                {
                                    continue;
                                }
                                dataTable.Columns.Add(new DataColumn(item.Name + "_" + (i + 1).ToString() + "_NAME", typeof(string)));
                                dataTable.Columns.Add(new DataColumn(item.Name + "_" + (i + 1).ToString() + "_PERCENT", typeof(double)));
                            }
                        }
                        else
                        {
                            continue;
                        }
                        continue;
                    }
                    if (dataTable.Columns.Contains(item.Name))
                    {
                        continue;
                    }

                    Type dataType;
                    if (double.IsNaN(item.Values[0].Value))
                    {
                        dataType = typeof(string);
                    }
                    else
                    {
                        dataType = typeof(double);
                    }
                    dataTable.Columns.Add(new DataColumn(item.Name, dataType));
                }
            }
            foreach (TransferItem[] body in items)
            {
                DataRow row = dataTable.NewRow();
                dataTable.Rows.Add(row);
                foreach (TransferItem item in body)
                {
                    if (item == null) continue;
                    if (item.Name == "ATMOSPHERE" || item.Name == "COMPOSITION")
                    {
                        for (int i = 0; i < item.Values.Count; i++)
                        {
                            if (!dataTable.Columns.Contains(item.Name + "_" + (i + 1).ToString() + "_NAME"))
                            {
                                continue;
                            }
                            row[item.Name + "_" + (i + 1).ToString() + "_NAME"] = item.Values[i].Text;
                            row[item.Name + "_" + (i + 1).ToString() + "_PERCENT"] = item.Values[i].Value;
                        }
                    }
                    else if (dataTable.Columns.Contains(item.Name) && item.Values.Count == 1)
                    {
                        int index = dataGrid.Columns[item.Name].Index;
                        if (double.IsNaN(item.Values[0].Value) && dataTable.Columns[index].DataType == typeof(double))
                        {
                            continue;
                        }
                        if (double.IsNaN(item.Values[0].Value))
                        {
                            row[index] = item.Values[0].Text;
                        }
                        else
                        {
                            row[index] = item.Values[0].Value;
                        }
                    }
                }
            }
        }

        private void buttonApply_Click(object sender, EventArgs e)
        {
            ApplyRowFilter(textRowFilter.Text);
        }

        private void checkRowFilter_CheckedChanged(object sender, EventArgs e)
        {
            UpdateUI();
        }

        private void UpdateUI()
        {
            textRowFilter.Enabled = checkRowFilter.Checked;
            buttonApply.Enabled = checkRowFilter.Checked;
            if (checkRowFilter.Checked)
            {
                ApplyRowFilter(textRowFilter.Text);
            }
            else
            {
                ApplyRowFilter("");
            }
        }

        private void ApplyRowFilter(string filter)
        {
            string old = dataView.RowFilter;
            try
            {
                dataView.RowFilter = filter;
            }
            catch
            {
                dataView.RowFilter = old;
            }
        }

        private void buttonColumns_Click(object sender, EventArgs e)
        {
            using (FrmSelectColumnsDlg dlg = new FrmSelectColumnsDlg())
            {
                dlg.Grid = dataGrid;
                dlg.ShowDialog();
            }
        }

        DataSet dataSet = new DataSet();
        DataTable dataTable = new DataTable("info");
        DataView dataView = new DataView();

    }
}
