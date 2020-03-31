using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using RimWorld;
using TwitchToolkit.Utilities;
using UnityEngine;
using Verse;

namespace SirRandoo.ToolkitUtils.Utils
{
    public static class ShopExpansionHelper
    {
        public static readonly string ExpansionFile = Path.Combine(SaveHelper.dataPath, "ShopExt.json");

        public static void SaveData(object json, string filePath)
        {
            var result = JsonUtility.ToJson(json);
            Logger.Info(result);

            SaveData(result, filePath);
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

        public static void SaveShopExtension()
        {
            var builder = new StringBuilder("{");

            builder.Append("\"traits\":[");
            builder.Append(string.Join(",", TkUtils.ShopExpansion.traits.Select(JsonUtility.ToJson).ToArray()));
            builder.Append("],");
            builder.Append("\"races\":[");
            builder.Append(string.Join(",", TkUtils.ShopExpansion.races.Select(JsonUtility.ToJson).ToArray()));
            builder.Append("]}");

            SaveData(builder.ToString(), ExpansionFile);
        }

        public static T LoadData<T>(string filePath)
        {
            var directory = Path.GetDirectoryName(filePath);

            if (!Directory.Exists(directory))
            {
                throw new IOException($"Directory {directory} does not exist!");
            }

            return JsonUtility.FromJson<T>(File.ReadAllText(filePath));
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
                for (var i = TkUtils.ShopExpansion.traits.Count - 1; i >= 0; i--)
                {
                    if (loadedTraits.Any(t => t.defName.Equals(TkUtils.ShopExpansion.traits[i].defName)))
                    {
                        continue;
                    }

                    TkUtils.ShopExpansion.traits.RemoveAt(i);
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
                .Where(t => !TkUtils.ShopExpansion.traits.Any(p => t.defName.EqualsIgnoreCase(t.defName)))
                .ToList();

            foreach (var trait in missingTraits)
            {
                foreach (var t in TraitHelper.GetEffectiveTraits(trait))
                {
                    t.name = Unrichify.StripTags(t.name);
                    t.bypassLimit = TraitHelper.IsSexualityTrait(trait);

                    TkUtils.ShopExpansion.traits.Add(t);
                }
            }

            try
            {
                for (var i = TkUtils.ShopExpansion.races.Count - 1; i >= 0; i--)
                {
                    if (loadedRaces.Any(r => r.Equals(TkUtils.ShopExpansion.races[i].defName)))
                    {
                        continue;
                    }

                    TkUtils.ShopExpansion.races.RemoveAt(i);
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
                .Where(t => !TkUtils.ShopExpansion.races.Any(p => t.EqualsIgnoreCase(p.defName)))
                .ToList();

            foreach (var race in missingRaces)
            {
                TkUtils.ShopExpansion.races.Add(
                    new ShopExpansion.Race {defName = race, name = race, price = 2500, enabled = true}
                );
            }

            if (removedRaces <= 0 && removedTraits <= 0 && missingRaces.Count <= 0 && missingTraits.Count <= 0)
            {
                return;
            }

            Logger.Info("Trait/Race data changed between instances; saving new data...");
            SaveShopExtension();
        }
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
            public bool enabled;
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
