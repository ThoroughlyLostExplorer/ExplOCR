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
    public partial class FrmConfigureKeys : Form
    {
        public FrmConfigureKeys()
        {
            InitializeComponent();
        }

        public bool UseCustom
        {
            get
            {
                return radioCustom.Checked;
            }
            set
            {
                if (value)
                {
                    radioCustom.Checked = true;
                }
                else
                {
                    radioStandard.Checked = value;
                }
            }
        }

        public string KeyCombination
        {
            get
            {
                return MakeKeyString(combination);
            }
            set
            {
                combination = ParseKeyString(value);
                textBox1.Text = PrintKeyCombo(combination);
            }
        }

        private string PrintKeyCombo(List<int> c)
        {
            List<string> strings = new List<string>();
            for (int i = 0; i < c.Count; i++)
            {
                strings.Add(((Keys)c[i]).ToString());
            }
            return string.Join(",", strings.ToArray());
        }

        public static List<int> ParseKeyString(string combo)
        {
            List<int> output = new List<int>();
            string[] strings = combo.Split(new char[] { ',' });
            for (int i = 0; i < strings.Length; i++)
            {
                int value;
                if (int.TryParse(strings[i], out value))
                {
                    output.Add(value);
                }
            }
            return output;
        }

        public static string MakeKeyString(List<int> c)
        {
            List<string> strings = new List<string>();
            for (int i = 0; i < c.Count; i++)
            {
                strings.Add(c[i].ToString());
            }
            return string.Join(",", strings.ToArray());
        }

        private void RadioButton_CheckedChanged(object sender, EventArgs e)
        {
            recording = false;
            buttonStart.Enabled = radioCustom.Checked;
            buttonDone.Enabled = false;
        }

        private void FrmConfigureKeys_KeyDown(object sender, KeyEventArgs e)
        {
            if (recording)
            {
                combination.Add((int)e.KeyCode);
                textBox1.Text = PrintKeyCombo(combination);
            }
        }

        private void buttonStart_Click(object sender, EventArgs e)
        {
            combination.Clear();
            textBox1.Text = PrintKeyCombo(combination);
            recording = true;
            buttonStart.Enabled = false;
            buttonDone.Enabled = true;
        }

        private void buttonDone_Click(object sender, EventArgs e)
        {
            recording = false;
            buttonStart.Enabled = true;
            buttonDone.Enabled = false;
        }

        private void buttonOK_Click(object sender, EventArgs e)
        {
            if (UseCustom && combination.Count < 2)
            {
                MessageBox.Show("Sorry, key combination mus have length of at least two.");
                return;
            }
            DialogResult = System.Windows.Forms.DialogResult.OK;
            Close();
        }

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            DialogResult = System.Windows.Forms.DialogResult.Cancel;
            Close();
        }

        bool recording = false;
        List<int> combination = new List<int>();
    }
}
