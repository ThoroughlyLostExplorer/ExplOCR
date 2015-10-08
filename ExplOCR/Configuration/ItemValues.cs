using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace ExplOCR
{
    public class ItemValues
    {
        public static ItemValues Load(string file)
        {
            XmlSerializer ser = new XmlSerializer(typeof(ItemValues));
            using (FileStream stream = new FileStream(file, FileMode.Open, FileAccess.Read))
            {
                return (ItemValues)ser.Deserialize(stream);
            }
        }

        public static void Save(string file, ItemValues items)
        {
            XmlSerializer ser = new XmlSerializer(typeof(ItemValues));
            using (FileStream stream = new FileStream(file, FileMode.Create, FileAccess.Write))
            {
                ser.Serialize(stream, items);
            }
        }

        public string LookupUnit(string name)
        {
            foreach (Unit test in UnitNames)
            {
                if (test.Short == name) return test.Long;
            }
            return null;
        }

        [XmlArrayItem("Value")]
        public string[] VolcanismTypes;
        [XmlArrayItem("Value")]
        public string[] AtmosphereTypes;
        [XmlArrayItem("Value")]
        public string[] AtmosphereComponents;
        [XmlArrayItem("Value")]
        public string[] SolidComponents;
        [XmlArrayItem("Value")]
        public string[] RingTypes;
        [XmlArrayItem("Value")]
        public string[] MiningReserves;
        [XmlArrayItem("Value")]
        public string[] Terraforming;
        public Unit[] UnitNames;

        public struct Unit
        {
            public string Long;
            public string Short;
        }
    }
}
