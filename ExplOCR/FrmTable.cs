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
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Serialization;

namespace ExplOCR
{
    public partial class FrmTable : Form
    {
        public FrmTable()
        {
            InitializeComponent();

            dataTable.Columns.Add(HiddenIndexName, typeof(int));
            dataSet.Tables.Add(dataTable);
            dataView.Table = dataTable;
            dataGrid.DataSource = dataView;

            UpdateUI();

            LoadData();
            dataGrid.Columns[HiddenIndexName].Visible = false;
            editAstroBody.ChangeLayout(true);
            editAstroBody.Height = (buttonReRead.Top - 8) - editAstroBody.Top;
            initialized = true;
            checkReadOnly.Checked = true;

            splitContainer.Panel2MinSize = 300 + 450;
            tabControl.Width = 450;
        }

        private void UpdateUI()
        {
            buttonSave.Enabled = editAstroBody.HasChanges && !checkReadOnly.Checked;
            buttonReRead.Enabled = !checkReadOnly.Checked;
            buttonCancel.Enabled = editAstroBody.HasChanges;
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

        private void UpdateEditFields(int rowIndex)
        {
            if (rowIndex < 0)
            {
                editAstroBody.SetData(null, -1);
            }
            else
            {
                DataRowView rv = dataView[rowIndex];
                currentRow = (int)rv.Row[HiddenIndexName];
                editAstroBody.SetData(dataItems, currentRow);
                try
                {
                    string file = (string)rv.Row[WellKnownItems.ArchiveName];
                    imageDisplay.Image = new Bitmap(Path.Combine(PathHelpers.BuildScreenDirectory(), file));
                    imageDisplay.Invalidate();
                    imageDisplay.Update();
                }
                catch
                {

                }
                textOverview.Text = OutputConverter.GetDataText(dataItems[currentRow]);
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

        #region Data File Access

        private void SaveData()
        {
            XmlSerializer ser = new XmlSerializer(typeof(TransferItem[][]));

            using (FileStream fs = new FileStream(PathHelpers.BuildSaveFilename(), FileMode.Create, FileAccess.Write))
            {
                ser.Serialize(fs, dataItems);
            }
        }

        private void LoadData()
        {
            TransferItem[][] array;
            XmlSerializer ser = new XmlSerializer(typeof(TransferItem[][]));
            try
            {
                using (FileStream fs = File.OpenRead(PathHelpers.BuildSaveFilename()))
                {
                    array = ser.Deserialize(fs) as TransferItem[][];
                }
            }
            catch
            {
                array = new TransferItem[0][];
            }
            dataItems = array;
            dataTable.Rows.Clear();
            PopulateTable(array);
        }

        #endregion

        #region Populate Data Set

        public void PopulateTable(TransferItem[][] items)
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
            for (int index = 0; index < items.Length; index++)
            {
                TransferItem[] body = items[index];
                DataRow row = dataTable.NewRow();
                dataTable.Rows.Add(row);
                row[HiddenIndexName] = index;
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
                    else if (item.Name == WellKnownItems.ArchiveName)
                    {
                        row[item.Name] = item.Values[0].Text;
                    }
                    else if (dataTable.Columns.Contains(item.Name) && item.Values.Count == 1)
                    {
                        if (double.IsNaN(item.Values[0].Value) && dataTable.Columns[item.Name].DataType == typeof(double))
                        {
                            continue;
                        }
                        if (double.IsNaN(item.Values[0].Value))
                        {
                            row[item.Name] = item.Values[0].Text;
                        }
                        else
                        {
                            row[item.Name] = item.Values[0].Value;
                        }
                    }
                }
            }

            foreach (DataGridViewColumn col in dataGrid.Columns)
            {
                col.ReadOnly = true;
            }
        }


        #endregion

        string[] GetArchiveFiles(TransferItem[] data)
        {
            List<string> files = new List<string>();

            foreach (TransferItem item in data)
            {
                if (item.Name == WellKnownItems.ArchiveName)
                {
                    foreach (TransferItemValue value in item.Values)
                    {
                        files.Add(value.Text);
                    }
                }
            }
            return files.ToArray();
        }

        private void ShowImagePanel()
        {
            int controlsWidth = 300 + 450;
            int oldSplitterDistance = splitContainer.SplitterDistance;
            int newMinWidth = controlsWidth + splitContainer.Panel1MinSize;
            MinimumSize = new Size(newMinWidth, MinimumSize.Width);
            if (Width < MinimumSize.Width)
            {
                Width = MinimumSize.Width;
            }
            splitContainer.Panel2MinSize = controlsWidth;
            if (oldSplitterDistance > 450 + splitContainer.Panel1MinSize)
            {
                splitContainer.SplitterDistance = oldSplitterDistance - 450;
            }
            tabControl.Width = 450;
            buttonHide.Text = "Hide >";
        }

        private void HideImagePanel()
        {
            tabControl.Width = 0;
            buttonHide.Text = "< Show";
            splitContainer.Panel2MinSize = 300;
            splitContainer.SplitterDistance += 450;
            this.MinimumSize = new Size(splitContainer.Panel1MinSize + splitContainer.Panel2MinSize, this.MinimumSize.Height);
        }


        #region Events Related to the Grid

        private void buttonColumns_Click(object sender, EventArgs e)
        {
            using (FrmSelectColumnsDlg dlg = new FrmSelectColumnsDlg())
            {
                dlg.Grid = dataGrid;
                dlg.ShowDialog();
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

        private void dataGrid_SelectionChanged(object sender, EventArgs e)
        {
            if (editAstroBody.HasChanges || !initialized)
            {
                return;
            }

            if (dataGrid.SelectedCells.Count > 0)
            {
                UpdateEditFields(dataGrid.SelectedCells[0].RowIndex);
            }
            else
            {
                UpdateEditFields(-1);
            }
        }

        #endregion

        #region Events Realted to Editing

        private void editAstroBody_StateChanged(object sender, EventArgs e)
        {
            UpdateUI();
        }

        private void buttonSave_Click(object sender, EventArgs e)
        {
            editAstroBody.SaveEditState();
            SaveData();
            // This is a waste of resources, but we are still in beta.
            initialized = false;
            LoadData();
            initialized = true;

            if (currentRow >= 0 && currentRow < dataGrid.Rows.Count - 1)
            {
                for (int i = 0; i < dataGrid.Columns.Count; i++)
                {
                    if (dataGrid.Columns[i].Visible)
                    {
                        dataGrid.CurrentCell = dataGrid.Rows[currentRow].Cells[i];
                        break;
                    }
                }
            }
        }

        private void checkReadOnly_CheckedChanged(object sender, EventArgs e)
        {
            editAstroBody.ReadOnly = checkReadOnly.Checked;
            UpdateUI();
        }

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            if (dataGrid.SelectedCells.Count > 0)
            {
                UpdateEditFields(dataGrid.SelectedCells[0].RowIndex);
            }
            else
            {
                UpdateEditFields(-1);
            }
        }

        private void buttonHide_Click(object sender, EventArgs e)
        {
            if (tabControl.Width > 100)
            {
                HideImagePanel();
            }
            else
            {
                ShowImagePanel();
            }

        }

        private void buttonReRead_Click(object sender, EventArgs e)
        {            
            try
            {
                bool stitch = false;
                OcrReader ocrReader = LibExplOCR.CreateOcrReader();
                Bitmap bmpStructure, bmpHeatmap;

                string[] files = GetArchiveFiles(dataItems[currentRow]);
                if (files.Length > 0)
                {
                    foreach (string file in files)
                    {
                        using (Bitmap bmp = new Bitmap(Path.Combine(PathHelpers.BuildScreenDirectory(), file)))
                        {
                            ocrReader.StitchPrevious = stitch;
                            LibExplOCR.ProcessImage(ocrReader, bmp, out bmpStructure, out bmpHeatmap);
                            ocrReader.StitchPrevious = false;
                            textOverview.Text = OutputConverter.GetDataText(ocrReader.Items);
                            if (!stitch)
                            {
                                stitch = true;
                                imageDisplay.Image = bmpHeatmap;
                                imageDisplay.Invalidate();
                                imageDisplay.Update();
                            }
                        }
                    }
                    editAstroBody.ResetControls(ocrReader.Items);
                    editAstroBody.HasChanges = true;
                }
            }
            catch
            {
            }
        }

        #endregion

        #region Private Variables

        DataSet dataSet = new DataSet();
        DataTable dataTable = new DataTable("info");
        DataView dataView = new DataView();
        TransferItem[][] dataItems;
        bool initialized = false;
        int currentRow = -1;

        const string HiddenIndexName = "_hiddenIndex";

        #endregion
    }
}
