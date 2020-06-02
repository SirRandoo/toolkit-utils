using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Xml.Serialization;

namespace SirRandoo.ToolkitUtils.Utils
{
    [XmlRoot("ShopExpansion", IsNullable = false, Namespace = null)]
    public class XmlShop
    {
        public List<XmlRace> Races;
        public List<XmlTrait> Traits;
    }

    public class XmlRace
    {
        [XmlAttribute]
        public string DefName;

        public bool Enabled;
        public string Name;
        public int Price;
    }

    public class XmlTrait
    {
        public int AddPrice;
        public bool BypassLimit;
        public bool CanAdd;
        public bool CanRemove;
        public string DefName;
        public int Degree;
        public string Name;
        public int RemovePrice;
    }

    [SuppressMessage("ReSharper", "InconsistentNaming")]
    [SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global")]
    public class TraitDump
    {
        public int addPrice { get; set; }
        public bool bypassLimit { get; set; }
        public bool canAdd { get; set; }
        public bool canRemove { get; set; }
        public string[] conflicts { get; set; }
        public string defName { get; set; }
        public int degree { get; set; }
        public string description { get; set; }
        public string name { get; set; }
        public int removePrice { get; set; }
        public string[] stats { get; set; }
    }

    [SuppressMessage("ReSharper", "InconsistentNaming")]
    [SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global")]
    public class RaceDump
    {
        public string defName { get; set; }
        public bool enabled { get; set; }
        public string name { get; set; }
        public int price { get; set; }
    }

    [SuppressMessage("ReSharper", "InconsistentNaming")]
    [SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global")]
    public class ModDump
    {
        public string author { get; set; }
        public string name { get; set; }
        public string steamId { get; set; }
        public string version { get; set; }
    }

    [SuppressMessage("ReSharper", "InconsistentNaming")]
    [SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global")]
    public class CommandDump
    {
        public string description { get; set; }
        public string name { get; set; }
        public bool shortcut { get; set; }
        public string usage { get; set; }
        public string userLevel { get; set; }
    }

    [SuppressMessage("ReSharper", "InconsistentNaming")]
    [SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global")]
    public class ShopDump
    {
        public List<TraitDump> traits { get; set; }
        public List<RaceDump> races { get; set; }
    }
}
