using System.Xml.Serialization;

namespace SirRandoo.ToolkitUtils.Models
{
    public class TraitProduct
    {
        [XmlAttribute] public bool CanAdd;
        [XmlAttribute] public bool CanBypassLimit;
        [XmlAttribute] public bool CanRemove;
        [XmlAttribute] public int CostToAdd;
        [XmlAttribute] public int CostToRemove;
        [XmlAttribute] public string DefName;
        [XmlAttribute] public int Degree;
        [XmlAttribute] public bool HasCustomName;
        [XmlAttribute] public string Name;
    }
}
