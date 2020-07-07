using System.Xml.Serialization;

namespace SirRandoo.ToolkitUtils.Models
{
    public class PawnKindProduct
    {
        [XmlAttribute] public int Cost;
        [XmlAttribute] public string DefName;
        [XmlAttribute] public bool Enabled;
        [XmlAttribute] public bool HasCustomName;
        [XmlAttribute] public string Name;
    }
}
