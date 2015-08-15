using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Text;
using System.Xml.Serialization;

namespace ExplOCR
{
    public class TableItem
    {
        public string Name = "";
        public string Description = "";
        public string MinimalMatch = "";
        public string Unit = "";
        public bool AllText;
        public bool NoText;
        public bool Signed;
        public bool BeltOnly;
        public bool Percentage;
        public int ExcludeUnit;
        public int InitialSkip;

        public static TableItem[] Test()
        {
            TableItem item = new TableItem();
            item.Name = "Test";
            item.AllText = true;
            item.NoText = false;
            item.ExcludeUnit = 2;
            item.Signed = true;
            item.BeltOnly = false;
            item.Unit = "UNITLESS";
            return new TableItem[] { item };

        }

        public static TableItem[] Load(string file)
        {
            XmlSerializer ser = new XmlSerializer(typeof(TableItem[]));
            using (FileStream stream = new FileStream(file, FileMode.Open, FileAccess.Read))
            {
                return (TableItem[])ser.Deserialize(stream);
            }
        }

        public static void Save(string file, TableItem[] items)
        {
            XmlSerializer ser = new XmlSerializer(typeof(TableItem[]));
            using (FileStream stream = new FileStream(file, FileMode.Create, FileAccess.Write))
            {
                ser.Serialize(stream, items);
            }
        }
    }
}
