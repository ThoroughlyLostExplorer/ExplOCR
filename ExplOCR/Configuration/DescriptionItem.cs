using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Text;
using System.Xml.Serialization;

namespace ExplOCR
{
    public class DescriptionItem
    {
        public string Name;
        public string Short;
        public string Description;
        public string MinimalMatch;
        public bool Planet;
        public bool Star;

        public static DescriptionItem[] Load(string file)
        {
            XmlSerializer ser = new XmlSerializer(typeof(DescriptionItem[]));
            using (FileStream stream = new FileStream(file, FileMode.Open, FileAccess.Read))
            {
                return (DescriptionItem[])ser.Deserialize(stream);
            }
        }

        public static void Save(string file, DescriptionItem[] items)
        {
            XmlSerializer ser = new XmlSerializer(typeof(DescriptionItem[]));
            using (FileStream stream = new FileStream(file, FileMode.Create, FileAccess.Write))
            {
                ser.Serialize(stream, items);
            }
        }
    }
}
