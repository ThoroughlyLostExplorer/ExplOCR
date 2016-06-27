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
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ExplOCR
{
    public partial class EditAstroBody : UserControl
    {
        public EditAstroBody()
        {
            InitializeComponent();

            try
            {
                tableConfig = TableItem.Load(PathHelpers.BuildConfigFilename("TableItems"));
                descriptionConfig = DescriptionItem.Load(PathHelpers.BuildConfigFilename("Descriptions"));
                for (int i = 0; i < descriptionConfig.Length; i++)
                {
                    comboType.Items.Add(descriptionConfig[i].Short);
                }
            }
            catch
            {
            }
            HasChanges = false;
        }

        public event EventHandler StateChanged;

        public void ChangeLayout(bool vertical)
        {
            if (vertical)
            {
                splitContainer.Orientation = Orientation.Horizontal;
            }
            else
            {
                splitContainer.Orientation = Orientation.Vertical;
            }
        }

        public bool HasChanges
        {
            get { return hasChanges; }
            set
            {
                bool changed = hasChanges != value;
                hasChanges = value;
                if (changed && StateChanged != null)
                {
                    StateChanged(this, new EventArgs());
                }
            }
        }

        public bool ReadOnly
        {
            get { return textSystem.ReadOnly; }
            set
            {
                textSystem.ReadOnly = value;
                textBody.ReadOnly = value;
                textCoords.ReadOnly = value;
                textCategories.ReadOnly = value;
                textDescription.ReadOnly = value;
                comboType.Enabled = !value;
            }
        }

        internal void SetData(TransferItem[][] dataContainer, int index)
        {
            dataGridEdit.Rows.Clear();

            if (dataContainer == null)
            {
                textSystem.Text = "";
                textBody.Text = "";
                textCoords.Text = "";
                textCategories.Text = "";
                textDescription.Text = "";
            }
            else
            {
                ResetControls(dataContainer[index]);
            }
            editData = dataContainer;
            editIndex = index;
            HasChanges = false;
            UpdateUI();
        }

        internal void ResetControls(TransferItem[] data)
        {
            SystemCoordinates sc = new SystemCoordinates();
            dataGridEdit.Rows.Clear();
            bool skippedHeadline = false;
            for (int i = 0; i < data.Length; i++)
            {
                if (IsStandardProperty(data[i].Name))
                {
                    AddItemToGrid(data[i]);
                }
                else if (data[i].Name == WellKnownItems.Headline && skippedHeadline)
                {
                    AddItemToGrid(data[i]);
                }
                else if (data[i].Name == WellKnownItems.Headline)
                {
                    skippedHeadline = true;
                }
                else if (data[i].Name == WellKnownItems.System && data[i].Values.Count > 0)
                {
                    textSystem.Text = data[i].Values[0].Text;
                }
                else if (data[i].Name == WellKnownItems.BodyCode && data[i].Values.Count > 0)
                {
                    textBody.Text = data[i].Values[0].Text;
                }
                else if (data[i].Name == WellKnownItems.GalCoordX && data[i].Values.Count > 0)
                {
                    sc.X = data[i].Values[0].Value;
                }
                else if (data[i].Name == WellKnownItems.GalCoordY && data[i].Values.Count > 0)
                {
                    sc.Y = data[i].Values[0].Value;
                }
                else if (data[i].Name == WellKnownItems.GalCoordZ && data[i].Values.Count > 0)
                {
                    sc.Z = data[i].Values[0].Value;
                }
                else if (data[i].Name == WellKnownItems.CustomCategory && data[i].Values.Count > 0)
                {
                    textCategories.Text = data[i].Values[0].Text.Replace(";", Environment.NewLine);
                }
                else if (data[i].Name == WellKnownItems.CustomDescription && data[i].Values.Count > 0)
                {
                    textDescription.Text = data[i].Values[0].Text;
                }
                else if (data[i].Name == WellKnownItems.Description && data[i].Values.Count > 0)
                {
                    for (int j = 0; j < comboType.Items.Count; j++)
                    {
                        if (descriptionConfig[j].Name.ToLower() == data[i].Values[0].Text.ToLower())
                        {
                            comboType.SelectedIndex = j;
                            break;
                        }
                    }
                }
            }

            textCoords.Text = sc.ToString();
        }

        private void AddItemToGrid(TransferItem item)
        {
            if (item.Name == "ATMOSPHERE" || item.Name == "COMPOSITION")
            {
                for (int i = 0; i < item.Values.Count; i++)
                {
                    dataGridEdit.Rows.Add(new object[] { item.Name + "_" + (i + 1).ToString() + "_NAME", 0, item.Values[i].Text });
                    dataGridEdit.Rows.Add(new object[] { item.Name + "_" + (i + 1).ToString() + "_PERCENT", 0, item.Values[i].Value.ToString() });
                }
            }
            else if(item.Values.Count == 1)
            {
                if (IsAllTextProperty(item.Name))
                {
                    dataGridEdit.Rows.Add(new object[] { item.Name, 0, item.Values[0].Text });
                }
                else
                {
                    dataGridEdit.Rows.Add(new object[] { item.Name, 0, item.Values[0].Value.ToString() });
                }
            }
        }

        internal void SaveEditState()
        {
            string[] compositionNames = new string[3];
            double[] compositionValues = new double[3] { double.NaN, double.NaN, double.NaN };
            string[] atmosphereNames = new string[3];
            double[] atmosphereValues = new double[3] { double.NaN, double.NaN, double.NaN };
            SystemCoordinates sc = LibExplOCR.ParseCoordinateValues(textCoords.Text);

            bool error = false; ;
            List<TransferItem> items = new List<TransferItem>();
            TransferItem ti;

            ti = new TransferItem(WellKnownItems.System);
            ti.Values.Add(new TransferItemValue(textSystem.Text));
            items.Add(ti);

            ti = new TransferItem(WellKnownItems.BodyCode);
            ti.Values.Add(new TransferItemValue(textBody.Text));
            items.Add(ti);

            ti = new TransferItem(WellKnownItems.GalCoordX);
            ti.Values.Add(new TransferItemValue(""));
            ti.Values[0].Value = sc.X;
            items.Add(ti);
            ti = new TransferItem(WellKnownItems.GalCoordY);
            ti.Values.Add(new TransferItemValue(""));
            ti.Values[0].Value = sc.Y;
            items.Add(ti);
            ti = new TransferItem(WellKnownItems.GalCoordZ);
            ti.Values.Add(new TransferItemValue(""));
            ti.Values[0].Value = sc.Z;
            items.Add(ti);

            ti = new TransferItem(WellKnownItems.CustomCategory);
            ti.Values.Add(new TransferItemValue(textCategories.Text.Replace(Environment.NewLine, ";")));
            items.Add(ti);

            ti = new TransferItem(WellKnownItems.CustomDescription);
            ti.Values.Add(new TransferItemValue(textDescription.Text));
            items.Add(ti);

            ti = new TransferItem(WellKnownItems.Headline);
            ti.Values.Add(new TransferItemValue(textSystem.Text+" "+textBody.Text));
            items.Add(ti);

            ti = new TransferItem(WellKnownItems.Description);
            ti.Values.Add(new TransferItemValue(descriptionConfig[comboType.SelectedIndex].Name));
            items.Add(ti);

            foreach (DataGridViewRow row in dataGridEdit.Rows)
            {
                if (row.IsNewRow)
                {
                    continue;
                }
                foreach (DataGridViewCell cell in row.Cells)
                {
                    cell.ErrorText = "";
                }

                if (TryReadAtmosphere(0, atmosphereNames, atmosphereValues, row, ref error)
                    || TryReadAtmosphere(1, atmosphereNames, atmosphereValues, row, ref error)
                    || TryReadAtmosphere(2, atmosphereNames, atmosphereValues, row, ref error))
                {
                    continue;
                }
                if (TryReadComposition(0, compositionNames, compositionValues, row, ref error)
                    || TryReadComposition(1, compositionNames, compositionValues, row, ref error)
                    || TryReadComposition(2, compositionNames, compositionValues, row, ref error))
                {
                    continue;
                }

                string name = row.Cells[columnEditName.Index].Value as string;
                string value = row.Cells[columnEditValue.Index].Value as string;
                if (IsStandardProperty(name))
                {
                    if (IsAllTextProperty(name))
                    {
                        ti = new TransferItem(name);
                        ti.Values.Add(new TransferItemValue(value, double.NaN, GetDataUnit(name)));
                        items.Add(ti);
                    }
                    else
                    {
                        double d;
                        if (!double.TryParse(value, out d))
                        {
                            error = true;
                            row.Cells[columnEditValue.Index].ErrorText = "Not a number!";
                            continue;
                        }

                        ti = new TransferItem(name);
                        ti.Values.Add(new TransferItemValue("", d, GetDataUnit(name)));
                        items.Add(ti);
                    }
                }

                if (name == WellKnownItems.Headline)
                {
                    ti = new TransferItem(name);
                    ti.Name = name;
                    ti.Values.Add(new TransferItemValue(value));
                    items.Add(ti);
                }
            }

            ti = GetMultiValueItem("ATMOSPHERE", atmosphereNames, atmosphereValues);
            if (ti != null)
            {
                items.Add(ti);
            }
            ti = GetMultiValueItem("COMPOSITION", compositionNames, compositionValues);
            if (ti != null)
            {
                items.Add(ti);
            }

            foreach (TransferItem other in editData[editIndex])
            {
                if (IsStandardProperty(other.Name))
                {
                    continue;
                }

                bool exists = false;
                foreach (TransferItem present in items)
                {
                    if (present.Name == other.Name)
                    {
                        exists = true;
                    }
                }

                if (exists)
                {
                    continue;
                }

                items.Add(other);
            }
            
            if (!error)
            {
                SortAlike(editData[editIndex], items);
                editData[editIndex] = items.ToArray();
                HasChanges = false;
            }
        }

        private TransferItem GetMultiValueItem(string name, string[] nameContent, double[] valueContent)
        {
            TransferItem ti = new TransferItem();
            ti.Name = name;
            for (int i = 0; i < nameContent.Length; i++)
            {
                if (nameContent[i] != null && !double.IsNaN(valueContent[i]))
                {
                    ti.Values.Add(new TransferItemValue(nameContent[i], valueContent[i], "PERCENT"));
                }
            }
            if (ti.Values.Count > 0)
            {
                return ti;
            }
            else
            {
                return null;
            }
        }

        private bool TryReadComposition(int i, string[] compNames, double[] compValues, DataGridViewRow row, ref bool error)
        {
            string name = row.Cells[columnEditName.Index].Value as string;
            string value = row.Cells[columnEditValue.Index].Value as string;
            if (name == "COMPOSITION_" + (i + 1).ToString() + "_NAME")
            {
                compNames[i] = value;
                return true;
            }
            else if (name == "COMPOSITION_" + (i + 1).ToString() + "_PERCENT")
            {
                double d;
                if (!double.TryParse(value, out d))
                {
                    row.Cells[columnEditValue.Index].ErrorText = "Not a number!";
                    error = true;
                    return false;
                }
                compValues[i] = d;
                return true;
            }
            else
            {
                return false;
            }
        }

        private bool TryReadAtmosphere(int i, string[] atmosphereNames, double[] atmosphereValues, DataGridViewRow row, ref bool error)
        {
            string name = row.Cells[columnEditName.Index].Value as string;
            string value = row.Cells[columnEditValue.Index].Value as string;
            if (name == "ATMOSPHERE_" + (i + 1).ToString() + "_NAME")
            {
                atmosphereNames[i] = value;
                return true;
            }
            else if (name == "ATMOSPHERE_" + (i + 1).ToString() + "_PERCENT")
            {
                double d;
                if (!double.TryParse(value, out d))
                {
                    row.Cells[columnEditValue.Index].ErrorText = "Not a number!";
                    error = true;
                    return false;
                }
                atmosphereValues[i] = d;
                return true;
            }
            else
            {
                return false;
            }
        }

        void UpdateUI()
        {
            if (editData == null)
            {
                panelContainer.Enabled = false;
                dataGridEdit.Enabled = false;
            }
            else
            {
                panelContainer.Enabled = true;
                dataGridEdit.Enabled = true;
            }
        }

        bool IsStandardProperty(string name)
        {
            if (name == WellKnownItems.Description)
            {
                return false;
            }
            if (name == WellKnownItems.ScanDate)
            {
                return false;
            }
            foreach (TableItem ti in tableConfig)
            {
                if (ti.Name == name) return true;
            }
            return false;
        }

        bool IsAllTextProperty(string name)
        {
            if (name == WellKnownItems.Headline)
            {
                return true;
            }
            foreach (TableItem ti in tableConfig)
            {
                if (ti.Name == name) return ti.AllText;
            }
            return false;
        }

        private string GetDataUnit(string name)
        {
            foreach (TableItem ti in tableConfig)
            {
                if (ti.Name == name) return ti.Unit;
            }
            return "";
        }

        // For easier testing.
        private void SortAlike(TransferItem[] transferItem, List<TransferItem> items)
        {
            List<TransferItem> sorted = new List<TransferItem>();
            for (int i = 0; i < transferItem.Length; i++)
            {
                for (int j = 0; j < items.Count; j++)
                {
                    if (transferItem[i].Name == items[j].Name)
                    {
                        sorted.Add(items[j]);
                        items.RemoveAt(j);
                        break;
                    }
                }
            }
            sorted.AddRange(items);
            items.Clear();
            items.AddRange(sorted);
        }

        private void dataGridEdit_RowsAdded(object sender, DataGridViewRowsAddedEventArgs e)
        {
            HasChanges = true;

        }

        private void dataGridEdit_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            HasChanges = true;
        }

        private void dataGridEdit_RowsRemoved(object sender, DataGridViewRowsRemovedEventArgs e)
        {
            HasChanges = true;
        }

        private void EditField_TextChanged(object sender, EventArgs e)
        {
            HasChanges = true;
        }

        private void comboType_SelectedIndexChanged(object sender, EventArgs e)
        {
            HasChanges = true;
        }

        #region Private Variables

        TableItem[] tableConfig;
        DescriptionItem[] descriptionConfig;
        TransferItem[][] editData;
        int editIndex;
        bool hasChanges;

        #endregion
    }
}
