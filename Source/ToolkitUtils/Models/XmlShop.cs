using System.Collections.Generic;
using System.Xml.Serialization;

namespace SirRandoo.ToolkitUtils.Models
{
    [XmlRoot("ShopExpansion", IsNullable = false, Namespace = null)]
    public class XmlShop
    {
        public List<XmlRace> Races;
        public List<XmlTrait> Traits;
    }
}
