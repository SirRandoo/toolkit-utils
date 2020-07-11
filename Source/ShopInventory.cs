using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using JetBrains.Annotations;
using Newtonsoft.Json;
using RimWorld;
using SirRandoo.ToolkitUtils.Helpers;
using SirRandoo.ToolkitUtils.Models;
using TwitchToolkit;
using Verse;

namespace SirRandoo.ToolkitUtils
{
    [StaticConstructorOnStartup]
    public static class ShopInventory
    {
        private static readonly JsonSerializer JsonSerializer = new JsonSerializer
        {
            NullValueHandling = NullValueHandling.Ignore, MissingMemberHandling = MissingMemberHandling.Ignore,
        };

        static ShopInventory()
        {
        }

        public static List<TraitItem> Traits { get; set; }
        public static List<PawnKindItem> PawnKinds { get; set; }
        public static Dictionary<string, ItemData> ItemData { get; set; }

        [CanBeNull]
        private static T LoadJson<T>(string path) where T : class
        {
            if (!File.Exists(path))
            {
                TkLogger.Warn($"Could not load file at {path} -- Does not exist!");
                return null;
            }

            try
            {
                using (FileStream file = File.Open(path, FileMode.Open, FileAccess.Read))
                {
                    using (var reader = new StreamReader(file))
                    {
                        using (JsonReader jReader = new JsonTextReader(reader))
                        {
                            return (T) JsonSerializer.Deserialize(jReader, typeof(T));
                        }
                    }
                }
            }
            catch (Exception e)
            {
                TkLogger.Error($"Could not load file at {path}", e);
                return null;
            }
        }

        private static void SaveJson<T>(T obj, string path)
        {
            string directory = Path.GetDirectoryName(path);

            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory!);
            }

            string tempPath = Path.GetTempFileName();

            try
            {
                using (FileStream writer = File.Open(tempPath, FileMode.Create, FileAccess.Write))
                {
                    using (StreamWriter streamWriter = new StreamWriter(writer))
                    {
                        using (JsonWriter jWriter = new JsonTextWriter(streamWriter))
                        {
                            JsonSerializer.Serialize(jWriter, obj);
                        }
                    }
                }

                if (File.Exists(path))
                {
                    File.Replace(tempPath, path, null);
                }
                else
                {
                    File.Move(tempPath, path);
                }
            }
            catch (IOException e)
            {
                TkLogger.Error($"Could not save json to path {path}", e);
            }
        }

        public static bool LoadTraits(string path)
        {
            Traits = LoadJson<List<TraitItem>>(path);

            return Traits != null;
        }

        public static bool LoadPawnKinds(string path)
        {
            PawnKinds = LoadJson<List<PawnKindItem>>(path);

            return PawnKinds != null;
        }

        public static bool LoadItemData(string path)
        {
            ItemData = LoadJson<Dictionary<string, ItemData>>(path);

            return ItemData != null;
        }

        private static void ValidateTraits()
        {
            List<TraitDef> traitDefs = DefDatabase<TraitDef>.AllDefsListForReading;
            Traits.RemoveAll(t => traitDefs.Find(d => d.defName.Equals(t.DefName)) == null);

            foreach (TraitDef def in traitDefs)
            {
                List<TraitItem> item = Traits.FindAll(t => t.DefName.Equals(def.defName));

                if (item.NullOrEmpty())
                {
                    Traits.AddRange(def.ToTraitItems());
                    continue;
                }

                TraitItem[] traitItems = def.ToTraitItems().Except(item).ToArray();

                if (traitItems.Length > 0)
                {
                    Traits.AddRange(traitItems);
                }
            }
        }

        private static void ValidatePawnKinds()
        {
            List<PawnKindDef> kindDefs = DefDatabase<PawnKindDef>.AllDefsListForReading;
            PawnKinds.RemoveAll(k => kindDefs.Find(d => d.defName.Equals(k.DefName)) == null);

            foreach (PawnKindDef def in kindDefs)
            {
                List<PawnKindItem> item = PawnKinds.FindAll(k => k.DefName.Equals(def.defName));

                if (item.Count > 0)
                {
                    continue;
                }

                PawnKinds.Add(
                    new PawnKindItem
                    {
                        DefName = def.race.defName,
                        Enabled = true,
                        Name = def.race.label ?? def.race.defName,
                        Price = def.race.CalculateStorePrice(),
                        Data = new PawnKindData()
                    }
                );
            }
        }

        private static void ValidateItemData()
        {
            List<ThingDef> thingDefs = DefDatabase<ThingDef>.AllDefs
                .Where(t => t.race == null)
                .ToList();
            ItemData.RemoveAll(p => thingDefs.Find(d => d.defName.Equals(p.Key)) == null);

            foreach (ThingDef thingDef in thingDefs.Where(thingDef => !ItemData.ContainsKey(thingDef.defName)))
            {
                ItemData[thingDef.defName] = new ItemData
                {
                    IsMelee = thingDef.IsMeleeWeapon,
                    IsRanged = thingDef.IsRangedWeapon,
                    IsWeapon = thingDef.IsWeapon,
                    Mod = thingDef.modContentPack.Name,
                    KarmaType = KarmaType.Neutral,
                    QuantityLimit = -1
                };
            }
        }
    }
}
