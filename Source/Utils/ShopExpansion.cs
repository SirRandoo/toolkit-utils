using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using RimWorld;
using TwitchToolkit.Utilities;
using UnityEngine;
using Verse;

namespace SirRandoo.ToolkitUtils.Utils
{
    public static class ShopExpansionHelper
    {
        public static readonly string ExpansionFile = Path.Combine(SaveHelper.dataPath, "ShopExt.xml");
        public static readonly string JsonExpansionFile = Path.Combine(SaveHelper.dataPath, "ShopExt.json");

        public static void SaveData<T>(T xml, string filePath)
        {
            var directory = Path.GetDirectoryName(filePath);

            if (directory == null)
            {
                Logger.Warn($"File path @ {filePath} is invalid!");
                return;
            }

            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            var serializer = new XmlSerializer(typeof(T));

            using (var writer = File.OpenWrite(filePath))
            {
                serializer.Serialize(writer, xml);
            }
        }

        public static void SaveData(string data, string filePath)
        {
            var directory = Path.GetDirectoryName(filePath);

            if (directory == null)
            {
                Logger.Warn($"File path @ {filePath} is invalid!");
                return;
            }

            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            File.WriteAllText(filePath, data);
        }

        public static T LoadData<T>(string filePath)
        {
            var directory = Path.GetDirectoryName(filePath);

            if (!Directory.Exists(directory))
            {
                throw new IOException($"Directory {directory} does not exist!");
            }

            var serializer = new XmlSerializer(typeof(T));

            using (var reader = File.OpenText(filePath))
            {
                return (T) serializer.Deserialize(reader);
            }
        }

        public static void DumpShopExtension()
        {
            var jsonTraits = TkUtils.ShopExpansion.Traits.Select(
                    t =>
                        new ShopExpansion.Trait
                        {
                            addPrice = t.AddPrice,
                            bypassLimit = t.BypassLimit,
                            degree = t.Degree,
                            canAdd = t.CanAdd,
                            canRemove = t.CanRemove,
                            name = t.Name,
                            defName = t.DefName,
                            removePrice = t.RemovePrice
                        }
                )
                .Select(JsonUtility.ToJson)
                .ToArray();

            var jsonRaces = TkUtils.ShopExpansion.Races.Select(
                    r => new ShopExpansion.Race
                    {
                        defName = r.DefName, enabled = r.Enabled, name = r.Name, price = r.Price
                    }
                )
                .Select(JsonUtility.ToJson)
                .ToArray();

            var builder = new StringBuilder("{");

            builder.Append("\"traits\":[");
            builder.Append(string.Join(",", jsonTraits.ToArray()));
            builder.Append("],");
            builder.Append("\"races\":[");
            builder.Append(string.Join(",", jsonRaces.ToArray()));
            builder.Append("]}");

            SaveData(builder.ToString(), JsonExpansionFile);
        }

        public static void ValidateExpansionData()
        {
            Logger.Info("Validating shop expansion data...");
            var loadedTraits = DefDatabase<TraitDef>.AllDefsListForReading.ToHashSet();
            var loadedRaces = DefDatabase<PawnKindDef>.AllDefsListForReading.Where(i => i.RaceProps.Humanlike)
                .GroupBy(i => i.race.defName)
                .Select(i => i.Key)
                .ToHashSet();
            var removedTraits = 0;
            var removedRaces = 0;

            try
            {
                for (var i = TkUtils.ShopExpansion.Traits.Count - 1; i >= 0; i--)
                {
                    if (loadedTraits.Any(t => t.defName.Equals(TkUtils.ShopExpansion.Traits[i].DefName)))
                    {
                        continue;
                    }

                    TkUtils.ShopExpansion.Traits.RemoveAt(i);
                    removedTraits += 1;
                }

                if (removedTraits > 0)
                {
                    Logger.Info($"Removed {removedTraits} traits from the shop.");
                }
            }
            catch (Exception e)
            {
                Logger.Error("Could not validate trait data!", e);
            }

            var missingTraits = loadedTraits
                .Where(t => !TkUtils.ShopExpansion.Traits.Any(p => t.defName.EqualsIgnoreCase(p.DefName)))
                .ToList();

            foreach (var trait in missingTraits)
            {
                foreach (var t in TraitHelper.GetEffectiveTraits(trait))
                {
                    t.Name = Unrichify.StripTags(t.Name);
                    t.BypassLimit = TraitHelper.IsSexualityTrait(trait);

                    TkUtils.ShopExpansion.Traits.Add(t);
                }
            }

            try
            {
                for (var i = TkUtils.ShopExpansion.Races.Count - 1; i >= 0; i--)
                {
                    if (loadedRaces.Any(r => r.Equals(TkUtils.ShopExpansion.Races[i].DefName)))
                    {
                        continue;
                    }

                    TkUtils.ShopExpansion.Races.RemoveAt(i);
                    removedRaces += 1;
                }

                if (removedRaces > 0)
                {
                    Logger.Info($"Removed {removedRaces} races from the shop.");
                }
            }
            catch (Exception e)
            {
                Logger.Error("Could not validate race data!", e);
            }

            var missingRaces = loadedRaces
                .Where(t => !TkUtils.ShopExpansion.Races.Any(p => t.EqualsIgnoreCase(p.DefName)))
                .ToList();

            foreach (var race in missingRaces)
            {
                TkUtils.ShopExpansion.Races.Add(
                    new XmlRace {DefName = race, Name = race, Price = 2500, Enabled = true}
                );
            }

            if (removedRaces <= 0 && removedTraits <= 0 && missingRaces.Count <= 0 && missingTraits.Count <= 0)
            {
                return;
            }

            Logger.Info("Trait/Race data changed between instances; saving new data...");
            SaveData(TkUtils.ShopExpansion, ExpansionFile);
            
            if(TkSettings.JsonShop)
            {
                DumpShopExtension();
            }
        }
    }

    [XmlRoot("ShopExpansion", IsNullable = false)]
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
        public string DefName;
        public bool BypassLimit;
        public int Degree;
        public bool CanAdd;
        public bool CanRemove;
        public string Name;
        public int AddPrice;
        public int RemovePrice;
    }

    [Serializable]
    public class ShopExpansion
    {
        public List<Race> races;
        public List<Trait> traits;

        [Serializable]
        public class Trait
        {
            public int addPrice;
            public bool bypassLimit;
            public string defName;
            public int degree;
            public bool canAdd;
            public bool canRemove;
            public string name;
            public int removePrice;
        }

        [Serializable]
        public class Race
        {
            public string defName;
            public bool enabled;
            public string name;
            public int price;
        }
    }
}
