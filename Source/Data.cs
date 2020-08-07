using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;
using RimWorld;
using SirRandoo.ToolkitUtils.Helpers;
using SirRandoo.ToolkitUtils.Models;
using SirRandoo.ToolkitUtils.Utils;
using TwitchToolkit;
using Verse;

namespace SirRandoo.ToolkitUtils
{
    [StaticConstructorOnStartup]
    public static class Data
    {
        private static readonly JsonSerializer JsonSerializer = new JsonSerializer
        {
            NullValueHandling = NullValueHandling.Ignore,
            MissingMemberHandling = MissingMemberHandling.Ignore,
            ContractResolver = new CamelCasePropertyNamesContractResolver(),
            Converters = {new StringEnumConverter()}
        };

        static Data()
        {
            if (Traits.NullOrEmpty())
            {
                LoadTraits(Paths.TraitFilePath, true);
            }

            if (PawnKinds.NullOrEmpty())
            {
                LoadPawnKinds(Paths.PawnKindFilePath, true);
            }

            if (File.Exists(Paths.LegacyShopFilePath) && (PawnKinds.NullOrEmpty() || Traits.NullOrEmpty()))
            {
                MigrateFromLegacy(Paths.LegacyShopFilePath);
                File.Move(Paths.LegacyShopFilePath, Path.ChangeExtension(Paths.LegacyShopFilePath, ".bak")!);
            }

            if (ItemData == null || ItemData.Count <= 0)
            {
                LoadItemData(Paths.ItemDataFilePath);
            }

            ValidateItemData();
            ValidatePawnKinds();
            ValidateTraits();
            TkUtils.BuildModList();

            if (TkSettings.Offload)
            {
                Task.Run(DumpAllData);
            }
            else
            {
                DumpAllData();
            }
        }

        public static List<TraitItem> Traits { get; set; }
        public static List<PawnKindItem> PawnKinds { get; set; }
        public static Dictionary<string, ItemData> ItemData { get; set; }
        public static ModItem[] Mods { get; set; }
        public static List<ThingItem> Items { get; set; }

        private static void MigrateFromLegacy(string path)
        {
            XmlShop data;
            try
            {
                data = ShopExpansion.LoadData<XmlShop>(path);
            }
            catch (IOException e)
            {
                TkLogger.Error("Could not read legacy old data!", e);
                return;
            }

            if (data == null)
            {
                return;
            }

            Traits = data.Traits.Select(TraitItem.MigrateFrom).ToList();
            PawnKinds = data.Races.Select(PawnKindItem.MigrateFrom).ToList();
        }

        [CanBeNull]
        internal static T LoadJson<T>(string path, bool ignoreErrors = false) where T : class
        {
            if (!File.Exists(path) && !ignoreErrors)
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
                if (!ignoreErrors)
                {
                    TkLogger.Error($"Could not load file at {path}", e);
                }

                return null;
            }
        }

        internal static void SaveJson<T>(T obj, string path)
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
                    using (var streamWriter = new StreamWriter(writer))
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

        public static void LoadTraits(string path, bool ignoreErrors = false)
        {
            Traits = LoadJson<List<TraitItem>>(path, ignoreErrors) ?? new List<TraitItem>();
        }

        public static void SaveTraits(string path)
        {
            SaveJson(Traits, path);
        }

        public static void LoadPawnKinds(string path, bool ignoreErrors = false)
        {
            PawnKinds = LoadJson<List<PawnKindItem>>(path, ignoreErrors) ?? new List<PawnKindItem>();
        }

        public static void SavePawnKinds(string path)
        {
            SaveJson(PawnKinds, path);
        }

        public static void LoadItemData(string path)
        {
            ItemData = LoadJson<Dictionary<string, ItemData>>(path, true) ?? new Dictionary<string, ItemData>();
        }

        public static void SaveItemData(string path)
        {
            SaveJson(ItemData, path);
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
            List<PawnKindDef> kindDefs = DefDatabase<PawnKindDef>.AllDefs.Where(k => k.RaceProps.Humanlike).ToList();
            PawnKinds.RemoveAll(k => kindDefs.Find(d => d.race.defName.Equals(k.DefName)) == null);

            foreach (PawnKindDef def in kindDefs)
            {
                List<PawnKindItem> item = PawnKinds.FindAll(k => k.DefName.Equals(def.race.defName));

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
                        Cost = def.race.CalculateStorePrice(),
                        Data = new PawnKindData()
                    }
                );
            }
        }

        private static void ValidateItemData()
        {
            List<ThingDef> thingDefs = DefDatabase<ThingDef>.AllDefs.Where(t => t.race == null).ToList();
            List<string> toCull = thingDefs.Where(thing => !ItemData.ContainsKey(thing.defName))
               .Select(thing => thing.defName)
               .ToList();
            foreach (string defName in toCull)
            {
                ItemData.Remove(defName);
            }

            foreach (ThingDef thingDef in thingDefs.Where(thingDef => !ItemData.ContainsKey(thingDef.defName)))
            {
                ItemData[thingDef.defName] = new ItemData
                {
                    IsMelee = thingDef.IsMeleeWeapon,
                    IsRanged = thingDef.IsRangedWeapon,
                    IsWeapon = thingDef.IsWeapon,
                    Mod = thingDef.modContentPack?.Name ?? "Unknown",
                    KarmaType = KarmaType.Neutral,
                    QuantityLimit = -1,
                    IsStuffAllowed = true
                };
            }
        }

        public static void SaveLegacyShop(string path)
        {
            SaveJson(new ShopLegacy {Races = PawnKinds, Traits = Traits}, path);
        }

        public static void DumpAllData()
        {
            SaveItemData(Paths.ItemDataFilePath);
            ShopExpansion.DumpModList();
            ShopExpansion.DumpCommands();

            if (TkSettings.DumpStyle.Equals("SingleFile"))
            {
                SaveLegacyShop(Paths.LegacyShopDumpFilePath);
            }
            else
            {
                SaveTraits(Paths.TraitFilePath);
                SavePawnKinds(Paths.PawnKindFilePath);
            }
        }
    }
}
