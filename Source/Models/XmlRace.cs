using System.Xml.Serialization;

namespace SirRandoo.ToolkitUtils.Models
{
    public class XmlRace
    {
        [XmlAttribute] public string DefName;

        public bool Enabled;
        public string Name;
        public int Price;
    }
}
