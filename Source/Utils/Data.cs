using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Xml.Serialization;
using Verse;

namespace SirRandoo.ToolkitUtils.Utils
{
    public class ItemData
    {
        public string Mod { get; set; }
        public bool IsWeapon { get; set; }
        public bool IsRanged { get; set; }
    }

    [XmlRoot("PawnKindShop")]
    public class PawnKindShop
    {
        [XmlIgnore] public string FilePath;

        public List<PawnKindProduct> Products;
        [XmlIgnore] public XmlSerializer Serializer;

        public static PawnKindShop Load(string filePath)
        {
            var serializer = new XmlSerializer(typeof(PawnKindShop));

            using StreamReader reader = File.OpenText(filePath);
            var result = (PawnKindShop) serializer.Deserialize(reader);
            result.FilePath = filePath;
            result.Serializer = serializer;

            return result;
        }

        public void Save()
        {
            if (FilePath.NullOrEmpty())
            {
                return;
            }

            using FileStream writer = File.Open(FilePath, FileMode.Create, FileAccess.Write);
            Serializer.Serialize(writer, this);
        }

        public void Save(string filePath)
        {
            if (filePath.NullOrEmpty())
            {
                return;
            }

            using FileStream writer = File.Open(filePath, FileMode.Create, FileAccess.Write);
            Serializer.Serialize(writer, this);
        }
    }

    public class PawnKindProduct
    {
        [XmlAttribute] public int Cost;
        [XmlAttribute] public string DefName;
        [XmlAttribute] public bool Enabled;
        [XmlAttribute] public bool HasCustomName;
        [XmlAttribute] public string Name;
    }

    [XmlRoot("TraitShop")]
    public class TraitShop
    {
        [XmlIgnore] public string FilePath;

        public List<TraitProduct> Products;
        [XmlIgnore] public XmlSerializer Serializer;

        public static TraitShop Load(string filePath)
        {
            var serializer = new XmlSerializer(typeof(TraitShop));

            using StreamReader reader = File.OpenText(filePath);
            var result = (TraitShop) serializer.Deserialize(reader);
            result.FilePath = filePath;
            result.Serializer = serializer;

            return result;
        }

        public void Save()
        {
            if (FilePath.NullOrEmpty())
            {
                return;
            }

            using FileStream writer = File.Open(FilePath, FileMode.Create, FileAccess.Write);
            Serializer.Serialize(writer, this);
        }

        public void Save(string filePath)
        {
            if (filePath.NullOrEmpty())
            {
                return;
            }

            using FileStream writer = File.Open(filePath, FileMode.Create, FileAccess.Write);
            Serializer.Serialize(writer, this);
        }
    }

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

    [XmlRoot("ShopExpansion", IsNullable = false, Namespace = null)]
    public class XmlShop
    {
        public List<XmlRace> Races;
        public List<XmlTrait> Traits;
    }

    public class XmlRace
    {
        [XmlAttribute] public string DefName;

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
